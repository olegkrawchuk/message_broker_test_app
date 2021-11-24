using System.IO;
using System.Reflection;
using Newtonsoft.Json;

namespace MessageBrokerTestApp.Helpers
{
    public static class JsonConfiguration
    {
        public static T Read<T>(string path)
        {
            if (!Path.IsPathRooted(path))
            {
                path = Path.Combine(
                    Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase).Replace("file:\\", string.Empty),
                    path);
            }

            if (!File.Exists(path))
                throw new FileNotFoundException($"{path} is not found", Path.GetFileName(path));

            return JsonConvert.DeserializeObject<T>(File.ReadAllText(path), new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
        }
    }
}
