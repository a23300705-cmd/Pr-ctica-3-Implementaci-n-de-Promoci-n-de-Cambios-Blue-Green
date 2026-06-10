using MicroscopioBackend.Services._Interfaces.Muestras;

namespace MicroscopioBackend.Services.Muestras
{
    public class ImageService : IImageService
    {
        private readonly string _uploadPath;

        public ImageService()
        {
            _uploadPath = Environment.GetEnvironmentVariable("UPLOAD_PATH")
                          ?? Path.Combine(Directory.GetCurrentDirectory(), "uploads");
        }
        public async Task<string> UploadImageAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
                throw new Exception("Archivo vacío");

            var allowedTypes = new[] { "image/jpeg", "image/png", "image/webp" };

            if (!allowedTypes.Contains(file.ContentType))
                throw new Exception("Formato no permitido");

            if (file.Length > 5 * 1024 * 1024)
                throw new Exception("Archivo demasiado grande");

            if (!Directory.Exists(_uploadPath))
                Directory.CreateDirectory(_uploadPath);

            var extension = Path.GetExtension(file.FileName);
            var fileName = $"{Guid.NewGuid()}{extension}";

            var filePath = Path.Combine(_uploadPath, fileName);
            Console.WriteLine(filePath);

            using var stream = new FileStream(filePath, FileMode.Create);

            await file.CopyToAsync(stream);

            return fileName;
        }
    }
}