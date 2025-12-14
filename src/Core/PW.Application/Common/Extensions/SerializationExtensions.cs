using Newtonsoft.Json;
using System.Text;

namespace PW.Application.Extensions
{
    public static class SerializationExtensions
    {
        public static byte[] ToByteArray(this object obj)
        {
            if (obj == null) return null;

            var jsonString = JsonConvert.SerializeObject(obj, new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                NullValueHandling = NullValueHandling.Ignore
            });

            return Encoding.UTF8.GetBytes(jsonString);
        }

        public static T FromByteArray<T>(this byte[] byteArray)
        {
            if (byteArray == null || byteArray.Length == 0) return default;

            var jsonString = Encoding.UTF8.GetString(byteArray);
            return JsonConvert.DeserializeObject<T>(jsonString);
        }

        public static string ToJson(this object obj)
        {
            if (obj == null) return string.Empty;
            return JsonConvert.SerializeObject(obj, new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });
        }
    }
}
