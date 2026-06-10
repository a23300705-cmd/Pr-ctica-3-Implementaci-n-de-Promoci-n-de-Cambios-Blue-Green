using MicroscopioBackend.Models.Muestras;

namespace MicroscopioBackend.Models
{
    public class Muestra
    {
        public long Id { get; set; }

        public int UsuarioId { get; set; }

        public string Nombre { get; set; } = null!;

        public string? Descripcion { get; set; }

        public DateTime? FechaCreacion { get; set; }

        public DateTime? FechaActualizacion { get; set; }

        public long? IdImagenPrincipal { get; set; }

        public List<ImagenMuestra>? Imagenes { get; set; }

        public List<Categoria>? Categorias { get; set; }

        public override string ToString()
        {

            return $"Muestra {{ " +
                   $"Id: {Id}, " +
                   $"UsuarioId: {UsuarioId}, " +
                   $"Nombre: '{Nombre}', " +
                   $"Descripcion: '{Descripcion}', " +
                   $"FechaCreacion: {FechaCreacion}, " +
                   $"FechaActualizacion: {FechaActualizacion}, " +
                   $"IdImagenPrincipal: {IdImagenPrincipal}, " +
                   $"}}";
        }

    }


}
