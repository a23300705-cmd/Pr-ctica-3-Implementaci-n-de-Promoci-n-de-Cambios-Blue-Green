using MicroscopioBackend.Models.Muestras;

namespace MicroscopioBackend.Models.Requests
{
    public class SubirMuestraRequest
    {
        public string Nombre { get; set; } = null!;
        public string? Descripcion { get; set; }

        public List<int> Categorias { get; set; } = new();

    }
}
