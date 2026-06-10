namespace MicroscopioBackend.Models.Requests
{
    public class LoginUsuarioRequest
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;

    }
}
