using System.Text.Json;
using System.Text.RegularExpressions;

public static class Utils
{
    public static T Dump<T>(this T self)
    {
        Console.WriteLine(JsonSerializer.Serialize(self, new JsonSerializerOptions()
        {
            WriteIndented = true,
            IncludeFields = true
        }));
        return self;
    }
}