using MicroscopioBackend.Models;

namespace MicroscopioBackend.Services.Interfaces.Auth
{
    public interface IGoogleAuthService
    {
        Task<Response<Usuario>> LoginWithGoogleAsync(string idToken);
    }
}
