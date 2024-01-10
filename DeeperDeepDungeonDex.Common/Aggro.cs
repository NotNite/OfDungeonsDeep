using VYaml.Annotations;

namespace DeeperDeepDungeonDex.Common;

[YamlObject(NamingConvention.UpperCamelCase)]
public enum Aggro {
    Sight,
    Sound,
    Proximity
}
