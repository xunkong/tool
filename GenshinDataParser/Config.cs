using MySqlConnector;
using System.Text.Encodings.Web;
using System.Text.Json.Serialization;

namespace GenshinDataParser;

internal abstract class Config
{

    public static Dictionary<long, string> TextMap;

    public static string GenshinDataPath = @"C:\Users\Scighost\OneDrive\Code\GenshinData\";

    public static string MySqlConnectionString = "Server=localhost;Database=xunkong;Uid=root;Pwd=mysql;";

    public static JsonSerializerOptions JsonOptions = new JsonSerializerOptions { DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull, PropertyNameCaseInsensitive = true, WriteIndented = true, Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping };

    public static JsonSerializerOptions DefultJsonOptions = new JsonSerializerOptions();

    static Config()
    {
        var textMapFile = Path.Combine(GenshinDataPath, @"TextMap\TextMapCHS.json");
        var textMap = File.ReadAllText(textMapFile);
        var node = (JsonObject)JsonNode.Parse(textMap)!;
        TextMap = new Dictionary<long, string>(node.Count);
        foreach (var item in node)
        {
            TextMap.Add(long.Parse(item.Key), item.Value!.ToString());
        }
    }


    public static MySqlConnection CreateConnection()
    {
        return new MySqlConnection(MySqlConnectionString);
    }


    public static string GetFileText(string releativePath)
    {
        var file = Path.Combine(GenshinDataPath, releativePath);
        return File.ReadAllText(file);
    }


}
