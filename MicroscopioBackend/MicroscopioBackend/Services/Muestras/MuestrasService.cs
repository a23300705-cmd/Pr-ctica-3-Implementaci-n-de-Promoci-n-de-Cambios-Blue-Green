using MicroscopioBackend.Infraestructure.Database;
using MicroscopioBackend.Models;
using MicroscopioBackend.Models.Muestras;
using MicroscopioBackend.Models.Requests;
using MicroscopioBackend.Services._Interfaces.Muestras;
using MicroscopioBackend.Services.Interfaces.Muestras;
using System.Diagnostics;
using System.Text.Json;
using static System.Net.WebRequestMethods;

namespace MicroscopioBackend.Services.Muestras
{
    public class MuestraService : IMuestraService
    {
        private readonly IMySqlExecutor _executor;
        private readonly IImageService _imageService;
        private readonly IHttpContextAccessor _httpContextAccessor;


        public MuestraService(IMySqlExecutor executor, IImageService imageService, IHttpContextAccessor httpContextAccessor)
        {
            _imageService = imageService;
            _executor = executor;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<Response<CatalogoMuestras>?> ObtenerMuestras(PageRequest request)
        {
            var parametros = new Dictionary<string, object>
            {
                { "p_page", request.Page },
                { "p_size", request.Size }
            };

            return await _executor.EjecutarProcedure<CatalogoMuestras>(
                "obtener_catalogo_muestras",
                parametros
            );
        }

        public async Task<Response<Muestra>?> ObtenerMuestra(int id)
        {
            var parametros = new Dictionary<string, object>
            {
                { "p_id", id }
            };
            return await _executor.EjecutarProcedure<Muestra>(
                "obtener_muestra_por_id",
                parametros
            );
        }

        public async Task<Response<Muestra>?> SubirMuestra(int userId, SubirMuestraRequest request)
        {
            if (request == null)
            {
                return new Response<Muestra>
                {
                    Success = false,
                    Tipo = 2,
                    Mensaje = "Request inválido",
                    Data = null
                };
            }

            string categoriasJson = JsonSerializer.Serialize(request.Categorias);

            var parametros = new Dictionary<string, object>
            {
                { "p_id_usuario", userId },
                { "p_titulo", request.Nombre },
                { "p_descripcion", request.Descripcion },
                { "p_categorias", categoriasJson }
            };

            return await _executor.EjecutarProcedure<Muestra>(
                "subir_muestra",
                parametros
            );
        }

        public async Task<Response<object>?> EditarMuestra(int userId, EditarMuestraRequest request)
        {
            var parametros = new Dictionary<string, object>
            {
                { "p_id_muestra", request.IdMuestra },
                { "p_id_usuario", userId },
                { "p_titulo", request.Nombre },
                { "p_descripcion", request.Descripcion }
            };

            return await _executor.EjecutarProcedure<object>(
                "editar_muestra",
                parametros
            );
        }



        public async Task<Response<object>?> SubirImagen(int userId, ImagenRequest request)
        {
            try
            {
                if (request == null || request.File == null || request.File.Length == 0)
                {
                    return new Response<object>
                    {
                        Success = false,
                        Tipo = 2,
                        Mensaje = "Archivo inválido",
                        Data = null
                    };
                }

                // Subir archivo y obtener nombre generado
                var fileName = await _imageService.UploadImageAsync(request.File);

                if (string.IsNullOrEmpty(fileName))
                {
                    return new Response<object>
                    {
                        Success = false,
                        Tipo = 2,
                        Mensaje = "Error al subir la imagen",
                        Data = null
                    };
                }

                // Construir URL pública
                var httpRequest = _httpContextAccessor.HttpContext.Request;
                var baseUrl = $"{httpRequest.Scheme}://{httpRequest.Host}";
                string url = $"{baseUrl}/images/{fileName}";

                // Parámetros para procedure
                var parametros = new Dictionary<string, object>
        {
            { "p_id_usuario", userId },
            { "p_id_muestra", request.IdMuestra },
            { "p_objetivo", request.Objetivo },
            { "p_url", url }
        };

                // Guardar en BD
                return await _executor.EjecutarProcedure<object>(
                    "subir_imagen_muestra",
                    parametros
                );
            }
            catch (Exception ex)
            {
                return new Response<object>
                {
                    Success = false,
                    Tipo = 2,
                    Mensaje = ex.Message,
                    Data = null
                };
            }
        }


        public async Task<Response<object>?> EliminarMuestra(int userId, IdMuestraRequest request)
        {
            var parametros = new Dictionary<string, object>
            {
                { "p_id_usuario", userId },
                { "p_id_muestra", request.IdMuestra }
            };

            return await _executor.EjecutarProcedure<object>(
                "eliminar_muestra",
                parametros
            );
        }


        public async Task<Response<CatalogoMuestras>?> ObtenerFavoritos(int userId, PageRequest request)
        {
            var parametros = new Dictionary<string, object>
            {
                { "p_id_usuario", userId },
                { "p_page", request.Page },
                { "p_size", request.Size }
            };

            return await _executor.EjecutarProcedure<CatalogoMuestras>(
                "obtener_favoritos",
                parametros
            );
        }

        public async Task<Response<object>?> AgregarFavorito(int userId, FavoritoRequest request)
        {
            var parametros = new Dictionary<string, object>
            {
                { "p_id_usuario", userId },
                { "p_id_muestra", request.IdMuestra }
            };

            return await _executor.EjecutarProcedure<object>(
                "agregar_favorito",
                parametros
            );
        }

        public async Task<Response<object>?> EliminarFavorito(int userId, FavoritoRequest request)
        {
            var parametros = new Dictionary<string, object>
            {
                { "p_id_usuario", userId },
                { "p_id_muestra", request.IdMuestra }
            };

            return await _executor.EjecutarProcedure<object>(
                "eliminar_favorito",
                parametros
            );
        }


        public async Task<Response<CatalogoCategorias>> ObtenerCategorias()
        {
            var response = await _executor.EjecutarProcedure<CatalogoCategorias>(
                "obtener_categorias",
                new Dictionary<string, object>()
            );

            Debug.WriteLine(response.ToString());

            return response;
        }

        public async Task<Response<CatalogoMuestras>> ObtenerCatalogoMuestrasFiltradas(PageRequest pageRequest, List<int> categorias)
        {
            string categoriasJson = JsonSerializer.Serialize(categorias);
            Debug.WriteLine(categoriasJson);
            var parametros = new Dictionary<string, object>
            {
                { "p_page", pageRequest.Page },
                { "p_size", pageRequest.Size },
                { "p_categorias", categoriasJson }
            };
            var response = await _executor.EjecutarProcedure<CatalogoMuestras>(
                "obtener_catalogo_muestras_filtrado",
                parametros
            );
            return response;
        }
    



    }
}