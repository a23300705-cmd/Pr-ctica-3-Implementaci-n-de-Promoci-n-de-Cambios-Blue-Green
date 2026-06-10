using MicroscopioBackend.Models;
using System.Text.Json;

namespace MicroscopioBackend.Infraestructure.Database
{
    public interface IMySqlExecutor
    {
        Task<Response<T>?> EjecutarProcedure<T>(
            string nombre,
            Dictionary<string, object> parametros);

        Task<Response<T>?> EjecutarFunction<T>(
            string nombre,
            Dictionary<string, object> parametros);
    }
}
