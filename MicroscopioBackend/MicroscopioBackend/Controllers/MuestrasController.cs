using MicroscopioBackend.Models.Requests;
using MicroscopioBackend.Services.Interfaces.Muestras;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MicroscopioBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MuestrasController : ControllerBase
    {
        private readonly IMuestraService _service;

        public MuestrasController(IMuestraService service)
        {
            _service = service;
        }

        [HttpGet("obtener_muestras")]
        public async Task<IActionResult> GetCatalogo([FromQuery] PageRequest request)
        {
            var result = await _service.ObtenerMuestras(request);
            return Ok(result);
        }

        [HttpGet("obtener_muestra")]
        public async Task<IActionResult> GetMuestra(int id)
        {
            var result = await _service.ObtenerMuestra(id);
            return Ok(result);
        }

        [Authorize]
        [HttpGet("obtener_favoritos")]
        public async Task<IActionResult> GetFavoritos([FromQuery] PageRequest request)
        {
            var userIdClaim = User.FindFirst("id_usuario");
            if (userIdClaim == null) return Unauthorized();

            int userId = int.Parse(userIdClaim.Value);

            var result = await _service.ObtenerFavoritos(userId, request);
            return Ok(result);
        }

        [Authorize]
        [HttpPost("subir_muestra")]
        public async Task<IActionResult> SubirMuestra([FromForm] SubirMuestraRequest request)
        {
            var userIdClaim = User.FindFirst("id_usuario");
            if (userIdClaim == null) return Unauthorized();

            int userId = int.Parse(userIdClaim.Value);

            var result = await _service.SubirMuestra(userId, request);
            return Ok(result);
        }

        [Authorize]
        [HttpPost("subir_imagen")]
        public async Task<IActionResult> SubirImagen([FromForm] ImagenRequest request)
        {
            var userIdClaim = User.FindFirst("id_usuario");
            if (userIdClaim == null) return Unauthorized();

            int userId = int.Parse(userIdClaim.Value);

            var result = await _service.SubirImagen(userId, request);
            return Ok(result);
        }

        [Authorize]
        [HttpPut("editar_muestra")]
        public async Task<IActionResult> EditarMuestra([FromBody] EditarMuestraRequest request)
        {
            var userIdClaim = User.FindFirst("id_usuario");
            if (userIdClaim == null) return Unauthorized();

            int userId = int.Parse(userIdClaim.Value);

            var result = await _service.EditarMuestra(userId, request);
            return Ok(result);
        }

        [Authorize]
        [HttpDelete("eliminar_muestra")]
        public async Task<IActionResult> EliminarMuestra([FromBody] IdMuestraRequest request)
        {
            var userIdClaim = User.FindFirst("id_usuario");
            if (userIdClaim == null) return Unauthorized();

            int userId = int.Parse(userIdClaim.Value);

            var result = await _service.EliminarMuestra(userId, request);
            return Ok(result);
        }

        [Authorize]
        [HttpPost("agregar_favorito")]
        public async Task<IActionResult> AgregarFavorito([FromBody] FavoritoRequest request)
        {
            var userIdClaim = User.FindFirst("id_usuario");
            if (userIdClaim == null) return Unauthorized();

            int userId = int.Parse(userIdClaim.Value);

            var result = await _service.AgregarFavorito(userId, request);
            return Ok(result);
        }

        [Authorize]
        [HttpDelete("eliminar_favorito")]
        public async Task<IActionResult> EliminarFavorito([FromBody] FavoritoRequest request)
        {
            var userIdClaim = User.FindFirst("id_usuario");
            if (userIdClaim == null) return Unauthorized();

            int userId = int.Parse(userIdClaim.Value);

            var result = await _service.EliminarFavorito(userId, request);
            return Ok(result);
        }

        [HttpGet("obtener_categorias")]
        public async Task<IActionResult> GetCategorias()
        {
            var result = await _service.ObtenerCategorias();
            return Ok(result);
        }

        [HttpGet("obtener_catalogo_muestras_filtrado")]
        public async Task<IActionResult> GetCatalogoFiltrado([FromQuery] PageRequest pageRequest, [FromQuery] List<int> categorias)
        {
            var result = await _service.ObtenerCatalogoMuestrasFiltradas(pageRequest, categorias);
            return Ok(result);
        }
    }
}