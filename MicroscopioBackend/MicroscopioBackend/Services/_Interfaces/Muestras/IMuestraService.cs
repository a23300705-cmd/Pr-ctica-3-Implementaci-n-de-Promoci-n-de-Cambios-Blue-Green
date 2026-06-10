using MicroscopioBackend.Models;
using MicroscopioBackend.Models.Muestras;
using MicroscopioBackend.Models.Requests;

namespace MicroscopioBackend.Services.Interfaces.Muestras
{
    public interface IMuestraService
    {
        Task<Response<CatalogoMuestras>?> ObtenerMuestras(PageRequest request);

        Task<Response<Muestra>?> ObtenerMuestra(int id);

        Task<Response<object>?> EditarMuestra(int userId, EditarMuestraRequest request);

        Task<Response<Muestra>?> SubirMuestra(int userId, SubirMuestraRequest request);
        Task<Response<object>?> SubirImagen(int userId, ImagenRequest request);

        Task<Response<object>?> EliminarMuestra(int userId, IdMuestraRequest request);

        Task<Response<CatalogoMuestras>?> ObtenerFavoritos(int userId, PageRequest request);

        Task<Response<object>?> AgregarFavorito(int userId, FavoritoRequest request);

        Task<Response<object>?> EliminarFavorito(int userId, FavoritoRequest request);

        Task<Response<CatalogoCategorias>> ObtenerCategorias();
        Task<Response<CatalogoMuestras>> ObtenerCatalogoMuestrasFiltradas(PageRequest pageRequest, List<int> categorias);
    }
}