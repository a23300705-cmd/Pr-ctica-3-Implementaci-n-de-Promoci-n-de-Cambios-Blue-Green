namespace MicroscopioBackend.Models
{
    public class Response<T>
    {
        public bool Success { get; set; }
        public int Tipo { get; set; }
        public string Mensaje { get; set; }
        public T Data { get; set; }

        public override string ToString()
        {
            return
                $"Success={Success} | " +
                $"Tipo={Tipo} | " +
                $"Mensaje=\"{Mensaje}\" | " +
                $"Data={Data}";
        }
    }
}
