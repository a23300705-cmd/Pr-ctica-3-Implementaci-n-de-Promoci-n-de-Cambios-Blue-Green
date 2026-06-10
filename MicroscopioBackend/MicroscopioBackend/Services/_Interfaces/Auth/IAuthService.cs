using MicroscopioBackend.Models;
using MicroscopioBackend.Models.Muestras;
using MicroscopioBackend.Models.Requests;

namespace MicroscopioBackend.Services.Interfaces.Auth
{
    public interface IAuthService
    {
        Task<Response<object>?> RegistrarUsuarioCorreo(RegistrarUsuarioRequest request);
        Task<Response<object>?> LoginUsuarioCorreo(LoginUsuarioRequest request);
        Task<Response<object>> LoginWithGoogleAsync(string idToken);

    }
}
