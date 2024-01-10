using VYaml.Annotations;

namespace DeeperDeepDungeonDex.Common;

[YamlObject(NamingConvention.SnakeCase)]
public partial class Ability {
    public required string Name;
    public string? Potency; // "n/a" or a number
    public AttackType Type;
    public string? Description;
}
