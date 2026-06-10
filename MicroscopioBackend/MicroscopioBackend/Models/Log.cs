namespace MicroscopioBackend.Models
{
    public class Log
    {
        public long Id { get; set; }

        public int? UsuarioId { get; set; }

        public string Accion { get; set; }
        public string? Detalle { get; set; }

        public DateTime Fecha { get; set; }
    }
}
