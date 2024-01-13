using System.Text.Json.Serialization;

namespace DeeperDeepDungeonDex.Storage;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum Status : uint {
    Bind = 15003,
    Heavy = 15002,
    Sleep = 15013,
    Slow = 15009,
    Stun = 15004,
    Resolution = 0,
}
