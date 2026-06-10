using MicroscopioBackend.Models.Requests;
using MicroscopioBackend.Services.Interfaces;
using MicroscopioBackend.Services.Interfaces.Auth;
using Microsoft.AspNetCore.Mvc;


namespace MicroscopioBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _service;

        public AuthController(IAuthService service)
        {
            _service = service;
        }

        [HttpPost("register/email")]
        public async Task<IActionResult> RegistrarUsuarioCorreo([FromBody] RegistrarUsuarioRequest request)
        {
            var result = await _service.RegistrarUsuarioCorreo(request);

            if (result == null)
            {
                return StatusCode(500, new
                {
                    Success = false,
                    Message = "Error interno al registrar usuario"
                });
            }

            return Ok(result);
        }

        [HttpPost("login/email")]
        public async Task<IActionResult> LoginUsuarioCorreo([FromBody] LoginUsuarioRequest request)
        {


            var result = await _service.LoginUsuarioCorreo(request);

            if (result == null)
            {
                return StatusCode(500, new
                {
                    Success = false,
                    Message = "Error interno al iniciar sesión"
                });
            }

            return Ok(result);
        }

        [HttpPost("login/google")]
        public async Task<IActionResult> LoginGoogle([FromBody] GoogleLoginRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.IdToken))
            {
                return BadRequest(new
                {
                    Success = false,
                    Tipo = 5,
                    Mensaje = "El idToken es obligatorio"
                });
            }

            var result = await _service.LoginWithGoogleAsync(request.IdToken);

            if (result == null)
            {
                return StatusCode(500, new
                {
                    Success = false,
                    Message = "Error interno al iniciar sesión con Google"
                });
            }

            return Ok(result);
        }
    }
}
