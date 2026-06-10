using BCrypt.Net;
using BCrypt.Net;
using Google.Apis.Auth;
using MicroscopioBackend.Infraestructure.Database;
using MicroscopioBackend.Models;
using MicroscopioBackend.Models.Muestras;
using MicroscopioBackend.Models.Requests;
using MicroscopioBackend.Services._Interfaces.Auth;
using MicroscopioBackend.Services.Interfaces;
using MicroscopioBackend.Services.Interfaces.Auth;
using Org.BouncyCastle.Crypto.Generators;
using System.Diagnostics;

namespace MicroscopioBackend.Services.Auth
{
    public class AuthService : IAuthService
    {
        private readonly IMySqlExecutor _executor;
        private readonly ILogger<AuthService> _logger;
        private readonly IJwtService _jwtService;


        public AuthService(IMySqlExecutor executor, IJwtService jwtService, ILogger<AuthService> logger)
        {
            _executor = executor;
            _jwtService = jwtService;
            _logger = logger;
        }

        public async Task<Response<object>?> RegistrarUsuarioCorreo(RegistrarUsuarioRequest request)
        {

            if (string.IsNullOrWhiteSpace(request.Email))
            {
                return new Response<object>
                {
                    Success = false,
                    Tipo = 5,
                    Mensaje = "El correo es obligatorio",
                    Data = null
                };
            }

            if (string.IsNullOrWhiteSpace(request.Password))
            {
                return new Response<object>
                {
                    Success = false,
                    Tipo = 5,
                    Mensaje = $"La contraseña es obligatoria",
                    Data = null
                };
            }


            var emailValidator = new System.ComponentModel.DataAnnotations.EmailAddressAttribute();

            if (!emailValidator.IsValid(request.Email))
            {
                return new Response<object>
                {
                    Success = false,
                    Mensaje = "Formato de correo inválido",
                    Data = null
                };
            }



            if (request.Password.Length < 8)
            {
                return new Response<object>
                {
                    Success = false,
                    Tipo = 3,
                    Mensaje = "La contraseña debe tener al menos 8 caracteres",
                    Data = null
                };
            }

            if (!request.Password.Any(char.IsDigit))
            {
                return new Response<object>
                {
                    Success = false,
                    Tipo = 3,
                    Mensaje = "La contraseña debe contener al menos un número",
                    Data = null
                };
            }


            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.Password);


            var parametros = new Dictionary<string, object>
            {
                { "p_name", request.Name },
                { "p_email", request.Email },
                { "p_password", hashedPassword }
            };

            var response = await _executor.EjecutarProcedure<Usuario>(
                "registrar_usuario_correo",
                parametros
            );

            if (response == null || !response.Success || response.Data == null)
            {
                return new Response<object>
                {
                    Success = false,
                    Tipo = 2,
                    Mensaje = response != null ? response.Mensaje : "Error al registrar usuario",
                    Data = null
                };
            }

            var usuario = response?.Data;
            usuario.PasswordHash = null;

            var token = _jwtService.GenerateToken(usuario);
            var expiracion = DateTime.UtcNow.AddHours(2);

            await GuardarToken(usuario.Id, token, expiracion);

            return new Response<object>
            {
                Success = response != null && response.Success,
                Tipo = response != null && response.Success ? 1 : 2,
                Mensaje = response != null ? response.Mensaje : "Error al registrar usuario",
                Data = new
                {
                    token,
                    usuario
                }
            };
        }

        public async Task<Response<object>?> LoginUsuarioCorreo(LoginUsuarioRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Email))
            {
                return new Response<object>
                {
                    Success = false,
                    Tipo = 5,
                    Mensaje = "El correo es obligatorio",
                    Data = null
                };
            }

            if (string.IsNullOrWhiteSpace(request.Password))
            {
                return new Response<object>
                {
                    Success = false,
                    Tipo = 5,
                    Mensaje = "La contraseña es obligatoria",
                    Data = null
                };
            }

            var parametros = new Dictionary<string, object>
            {
                { "p_email", request.Email }
            };

            var response = await _executor.EjecutarProcedure<Usuario>(
                "login_usuario_correo",
                parametros
            );

            Debug.WriteLine(response.ToString());

            if (response == null || response.Data == null)
            {
                return new Response<object>
                {
                    Success = false,
                    Tipo = 3,
                    Mensaje = "Error al iniciar sesion",
                    Data = null
                };
            }

            if (!response.Success || response.Data == null)
            {
                return new Response<object>
                {
                    Success = false,
                    Tipo = response.Tipo,
                    Mensaje = response.Mensaje,
                    Data = null
                };
            }

            var usuario = response.Data;

            bool passwordValido = BCrypt.Net.BCrypt.Verify(
                request.Password,
                usuario.PasswordHash
            );

            if (!passwordValido)
            {
                return new Response<object>
                {
                    Success = false,
                    Tipo = 2,
                    Mensaje = "Credenciales inválidas",
                    Data = null
                };
            }

            var token = _jwtService.GenerateToken(usuario);
            await GuardarToken(usuario.Id, token, DateTime.UtcNow.AddHours(2));

            usuario.PasswordHash = null;

            return new Response<object>
            {
                Success = true,
                Tipo = 0,
                Mensaje = "Login exitoso",
                Data = new
                {
                    token,
                    usuario
                }
            };
        }

        public async Task<Response<object>> LoginWithGoogleAsync(string idToken)
        {
            _logger.LogInformation("Iniciando login con Google");

            var payload = await GoogleJsonWebSignature.ValidateAsync(
                idToken,
                new GoogleJsonWebSignature.ValidationSettings()
                {
                    Audience = new[] { Environment.GetEnvironmentVariable("GOOGLE_CLIENT_ID") }
                });

            _logger.LogInformation("Token validado para {Email}", payload.Email);

            var email = payload.Email;
            var name = payload.Name;
            var googleId = payload.Subject;

            var parametros = new Dictionary<string, object>
    {
        { "p_email", email }
    };

            _logger.LogInformation("Buscando usuario por email");

            var response = await _executor.EjecutarProcedure<Usuario>(
                "obtener_usuario_por_email",
                parametros
            );

            var usuario = response.Data;

            if (usuario == null)
            {
                _logger.LogInformation("Usuario no existe, registrando...");

                var parametrosRegistro = new Dictionary<string, object>
        {
            { "p_email", email },
            { "p_nombre", name },
            { "p_google_id", googleId }
        };

                var responseRegistro = await _executor.EjecutarProcedure<Usuario>(
                    "registrar_usuario_google",
                    parametrosRegistro
                );

                usuario = responseRegistro.Data;

                _logger.LogInformation("Usuario registrado con ID {UserId}", usuario?.Id);
            }
            else
            {
                _logger.LogInformation("Usuario encontrado con ID {UserId}", usuario.Id);

                if (usuario.AuthProvider == AuthProvider.LOCAL)
                {
                    _logger.LogWarning("Intento de login Google en cuenta LOCAL: {Email}", email);

                    return new Response<object>
                    {
                        Success = false,
                        Tipo = 2,
                        Mensaje = "Este correo ya está registrado con email y contraseña",
                        Data = null
                    };
                }
            }

            var token = _jwtService.GenerateToken(usuario);
            var expiracion = DateTime.UtcNow.AddHours(2);

            _logger.LogInformation("Token generado para {UserId}", usuario.Id);

            await GuardarToken(usuario.Id, token, expiracion);

            _logger.LogInformation("Token guardado correctamente");

            usuario.PasswordHash = null;

            return new Response<object>
            {
                Success = true,
                Tipo = 0,
                Mensaje = "Login con Google exitoso",
                Data = new
                {
                    usuario,
                    token
                }
            };
        }

        private async Task GuardarToken(int idUsuario, string token, DateTime expiracion)
        {
            var parametros = new Dictionary<string, object>
            {
                { "p_token", token },
                { "p_id_usuario", idUsuario },
                { "p_fecha_expiracion", expiracion }
            };

            var res = await _executor.EjecutarProcedure<object>(
                "guardar_access_token",
                parametros
            );

            if (res == null || !res.Success)
            {
                throw new Exception("Error al guardar el token en BD");
            }
        }

        public async Task<bool> RevocarToken(string token)
        {
            var parametros = new Dictionary<string, object>
        {
            { "p_token", token }
        };

            var res = await _executor.EjecutarProcedure<object>(
                "revocar_token",
                parametros
            );

            return res != null && res.Success;
        }

    }
}
