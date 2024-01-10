using VYaml.Annotations;

namespace DeeperDeepDungeonDex.Common;

[YamlObject(NamingConvention.UpperCamelCase)]
public enum AttackType {
    Physical,
    Magic,
    Unique
}
