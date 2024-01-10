using System.Text.Json.Serialization;

namespace DeeperDeepDungeonDex.Storage;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum AttackType {
    Physical,
    Magic,
    Unique
}
