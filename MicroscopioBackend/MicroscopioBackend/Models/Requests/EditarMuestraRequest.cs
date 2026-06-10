using MicroscopioBackend.Models.Muestras;

public class EditarMuestraRequest
{
    public int IdMuestra { get; set; }

    public string Nombre { get; set; } = null!;
    public string? Descripcion { get; set; }

    public List<int> Categorias { get; set; } = new();

    public List<ImagenMuestraRaw> Imagenes { get; set; } = new();
}