namespace MicroscopioBackend.Services._Interfaces.Muestras
{
    public interface IImageService
    {
        Task<string> UploadImageAsync(IFormFile file);
    }
}
