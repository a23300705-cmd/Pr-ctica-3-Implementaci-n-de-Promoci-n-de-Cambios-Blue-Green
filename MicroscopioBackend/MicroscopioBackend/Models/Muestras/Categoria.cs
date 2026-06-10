namespace MicroscopioBackend.Models.Muestras
{
    public class Categoria
    {
        public int Id { get; set; }

        public string Nombre { get; set; } = null!;

        public string Descripcion { get; set; } = null!;
    }

    public class CatalogoCategorias
    {
        public int Total { get; set; } 
        public List<Categoria> Categorias { get; set; } = new List<Categoria>();
    }
}
