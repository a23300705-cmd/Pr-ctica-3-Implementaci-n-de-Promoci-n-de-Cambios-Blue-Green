using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace MicroscopioBackend.Infraestructure.Serialization
{
    public static class JsonHelper
    {
        private static readonly JsonSerializerSettings _settings = new JsonSerializerSettings
        {
            MissingMemberHandling = MissingMemberHandling.Ignore,
            ContractResolver = new DefaultContractResolver
            {
                NamingStrategy = new SnakeCaseNamingStrategy()
            }
        };

        public static T? Deserialize<T>(string json)
        {
            if (string.IsNullOrWhiteSpace(json))
                return default;

            try
            {
                return JsonConvert.DeserializeObject<T>(json, _settings);
            }
            catch (JsonException ex)
            {
                Console.WriteLine($"[JsonHelper] Error JSON ({typeof(T).Name}): {ex.Message}");
                return default;
            }
        }

        public static string Serialize(object obj)
        {
            return JsonConvert.SerializeObject(obj, _settings);
        }
    }
}