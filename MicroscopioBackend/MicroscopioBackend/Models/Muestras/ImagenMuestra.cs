namespace MicroscopioBackend.Models.Muestras
{
    public class ImagenMuestra
    {
        public long IdImagen { get; set; }

        public long IdMuestra { get; set; }

        public string Url { get; set; } = null!;

        public int Objetivo { get; set; }

        public DateTime? FechaCreacion { get; set; }

        public Muestra? Muestra { get; set; }
    }

    public class  ImagenMuestraRaw
    {
        public int Objetivo { get; set; }
        public IFormFile File { get; set; } = null!;
    }
}
