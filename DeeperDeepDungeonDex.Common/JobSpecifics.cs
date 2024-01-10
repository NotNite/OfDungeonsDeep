using VYaml.Annotations;

namespace DeeperDeepDungeonDex.Common;

[YamlObject(NamingConvention.SnakeCase)]
public partial class JobSpecifics {
    public string Difficulty = "Unknown";
    public List<string> Notes = new();
}
