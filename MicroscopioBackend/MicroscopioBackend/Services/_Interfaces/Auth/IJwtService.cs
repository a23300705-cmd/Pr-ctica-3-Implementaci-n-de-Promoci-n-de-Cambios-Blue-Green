using MicroscopioBackend.Models;

namespace MicroscopioBackend.Services._Interfaces.Auth
{
    public interface IJwtService
    {
        string GenerateToken(Usuario usuario);
    }
}
