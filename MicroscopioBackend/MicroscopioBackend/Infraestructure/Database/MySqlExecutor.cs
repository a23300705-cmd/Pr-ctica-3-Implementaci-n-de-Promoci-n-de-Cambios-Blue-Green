using MicroscopioBackend.Infraestructure.Serialization;
using MicroscopioBackend.Models;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using System.Data;
using System.Diagnostics;
using System.Text.Json;

namespace MicroscopioBackend.Infraestructure.Database
{
    public class MySqlExecutor : IMySqlExecutor
    {
        private readonly IConfiguration _config;
        private readonly ILogger<MySqlExecutor> _logger;

        public MySqlExecutor(IConfiguration config, ILogger<MySqlExecutor> logger)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }


        public async Task<Response<T>?> EjecutarProcedure<T>(
            string nombre,
            Dictionary<string, object> parametros)
        {
            var connStr = _config.GetConnectionString("MySqlConnection");

            await using var conn = new MySqlConnection(connStr);
            await conn.OpenAsync();

            string sql = $"CALL {nombre}({string.Join(", ", parametros.Keys.Select(k => "@" + k))});";
            await using var cmd = new MySqlCommand(sql, conn);

            foreach (var p in parametros)
                cmd.Parameters.AddWithValue("@" + p.Key, p.Value ?? DBNull.Value);

            await using var reader = await cmd.ExecuteReaderAsync(CommandBehavior.CloseConnection);

            if (!await reader.ReadAsync())
                return null;

            string jsonResponse = reader.GetString("response");

            while (await reader.NextResultAsync()) { }

            try
            {
                var respuesta = JsonHelper.Deserialize<Response<T>>(jsonResponse);

                if (respuesta == null)
                    return null;

                if (respuesta.Tipo == 1)
                {
                    _logger.LogError(
                        "Error MySQL en procedure {Procedure}: {Mensaje}",
                        nombre,
                        respuesta.Mensaje
                    );
                }

                return respuesta;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error deserializando resultado del procedure {Procedure}: {Json}",
                    nombre,
                    jsonResponse
                );
                return null;
            }
        }

        public async Task<Response<T>?> EjecutarFunction<T>(
            string nombre,
            Dictionary<string, object> parametros)
        {
            var connStr = _config.GetConnectionString("MySqlConnection");

            await using var conn = new MySqlConnection(connStr);
            await conn.OpenAsync();

            string sql = $"SELECT {nombre}({string.Join(", ", parametros.Keys.Select(k => "@" + k))}) AS resultado;";
            await using var cmd = new MySqlCommand(sql, conn);

            foreach (var p in parametros)
                cmd.Parameters.AddWithValue("@" + p.Key, p.Value ?? DBNull.Value);

            var result = await cmd.ExecuteScalarAsync();

            if (result == null || result == DBNull.Value)
                return null;

            try
            {
                var respuesta = JsonHelper.Deserialize<Response<T>>(result.ToString()!);

                if (respuesta == null)
                    return null;

                if (respuesta.Tipo == 1)
                {
                    _logger.LogError(
                        "Error MySQL en function {Function}: {Mensaje}",
                        nombre,
                        respuesta.Mensaje
                    );
                }

                return respuesta;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error deserializando resultado de la function {Function}: {Json}",
                    nombre,
                    result.ToString()
                );
                return null;
            }
        }

    }
}
