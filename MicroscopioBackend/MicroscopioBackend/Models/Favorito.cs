using MicroscopioBackend.Models.Muestras;

namespace MicroscopioBackend.Models
{
    public class Favorito
    {
        public int UsuarioId { get; set; }
        public Usuario Usuario { get; set; } = null!;

        public long MuestraId { get; set; }
        public Muestra Muestra { get; set; } = null!;

        public DateTime FechaAgregado { get; set; } = DateTime.UtcNow;
    }
}
