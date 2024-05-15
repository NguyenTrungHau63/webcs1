using System.Text.Json;
using System.Text.Json.Serialization;
namespace WebCosmeticsStore.Models
{
    public static class SessionExtensions
    {
        public static void SetObjectAsJson(this ISession session, string key, object value)
        {
            JsonSerializerOptions options = new JsonSerializerOptions
            {
                ReferenceHandler = ReferenceHandler.IgnoreCycles
            };
            session.SetString(key, JsonSerializer.Serialize(value, options));
        }
        public static T GetObjectFromJson<T>(this ISession session, string key)
        {
            var value = session.GetString(key);
            return value == null ? default : JsonSerializer.Deserialize<T>(value);
        }
    }
}
