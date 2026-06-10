namespace MicroscopioBackend.Models.Muestras
{
    public class CatalogoMuestras
    {
        public int Total { get; set; }
        public List<Muestra>? Muestras { get; set; }

        public override string ToString()
        {
            var count = Muestras?.Count ?? 0;

            var preview = Muestras == null
                ? "null"
                : string.Join(", ", Muestras.Take(3).Select(m => m.Id));

            return $"CatalogoMuestras {{ " +
                   $"Total: {Total}, " +
                   $"MuestrasCount: {count}, " +
                   $"PreviewIds: [{preview}] " +
                   $"}}";
        }
    }
}
