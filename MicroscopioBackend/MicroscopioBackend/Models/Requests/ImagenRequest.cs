namespace MicroscopioBackend.Models.Requests
{
    public class ImagenRequest
    {
        public int IdMuestra { get; set; }
        public int Objetivo { get; set; }
        public IFormFile File { get; set; } = null!;
    }
}
