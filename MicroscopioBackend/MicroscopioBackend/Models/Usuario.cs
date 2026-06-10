using MicroscopioBackend.Models.Muestras;
using System.Text;

namespace MicroscopioBackend.Models
{
    public class Usuario
    {
        public int Id { get; set; }

        public string Email { get; set; } = null!;
        public string? PasswordHash { get; set; }

        public string Nombre { get; set; } = null!;

        public string? GoogleId { get; set; }

        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

        public AuthProvider AuthProvider { get; set; }

        public ICollection<Muestra> Muestras { get; set; } = new List<Muestra>();
        public ICollection<Favorito> Favoritos { get; set; } = new List<Favorito>();

        public override string ToString()
        {
            var sb = new StringBuilder();

            sb.AppendLine("=== Usuario ===");
            sb.AppendLine($"Id: {Id}");
            sb.AppendLine($"Email: {Email}");
            sb.AppendLine($"Nombre: {Nombre}");
            sb.AppendLine($"AuthProvider: {AuthProvider}");
            sb.AppendLine($"GoogleId: {GoogleId ?? "NULL"}");
            sb.AppendLine($"PasswordHash: {(string.IsNullOrEmpty(PasswordHash) ? "NULL" : "[HIDDEN]")}");
            sb.AppendLine($"FechaCreacion: {FechaCreacion:yyyy-MM-dd HH:mm:ss}");

            return sb.ToString();
        }
    }

    public enum AuthProvider
    {
        LOCAL,
        GOOGLE
    }
}