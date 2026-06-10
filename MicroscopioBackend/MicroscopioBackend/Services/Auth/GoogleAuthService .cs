using Google.Apis.Auth;
using MicroscopioBackend.Infraestructure.Database;
using MicroscopioBackend.Models;
using MicroscopioBackend.Services._Interfaces.Auth;
using MicroscopioBackend.Services.Interfaces.Auth;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Drawing;

namespace MicroscopioBackend.Services.Auth
{
    public class GoogleAuthService : IGoogleAuthService
    {
        private readonly IConfiguration _config;
        private readonly IMySqlExecutor _executor;
        private readonly IJwtService _jwtService;


        public GoogleAuthService(IConfiguration config, IMySqlExecutor executor, IJwtService jwtService)
        {
            _config = config;
            _executor = executor;
            _jwtService = jwtService;
        }

        public async Task<Response<Usuario>> LoginWithGoogleAsync(string idToken)
        {
            var payload = await GoogleJsonWebSignature.ValidateAsync(idToken, new GoogleJsonWebSignature.ValidationSettings()
            {
                Audience = new[] { Environment.GetEnvironmentVariable("GOOGLE_CLIENT_ID") }
            });

            var email = payload.Email;
            var name = payload.Name;

            var parametros = new Dictionary<string, object>
            {
                { "p_email", email }
            };

            var response = await _executor.EjecutarProcedure<Usuario>("obtener_usuario_por_email", parametros);
            var usuario = response.Data;

            if (usuario == null)
            {
                var parametrosRegistro = new Dictionary<string, object>
                {
                    { "p_email", email },
                    { "p_nombre", name },
                    { "p_google_id", payload.Subject }
                };

                var responseRegistro = await _executor.EjecutarProcedure<Usuario>(
                    "registrar_usuario_google",
                    parametrosRegistro
                );

                usuario = responseRegistro.Data;
            }
            else
            {
                return new Response<Usuario>
                {
                    Success = true,
                    Tipo = 2,
                    Mensaje = "Ya existe una cuenta con ese correo",
                    Data = null
                };
            }

            var token = _jwtService.GenerateToken(usuario);

            // 4. Retornar respuesta
            return new Response<Usuario>
            {
                Success = true,
                Tipo = 1,
                Mensaje = "Login con Google exitoso",
                Data = usuario
            };
        }
    }
}
