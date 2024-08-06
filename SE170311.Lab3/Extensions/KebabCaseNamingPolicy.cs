using System.Text.Json;

namespace SE170311.Lab3.Extensions;

public class KebabCaseNamingPolicy : JsonNamingPolicy
{
    public override string ConvertName(string name)
    {
        return string.Concat(name.Select((x, i) => i > 0 && char.IsUpper(x) ? "-" + x.ToString() : x.ToString())).ToLower();
    }
}