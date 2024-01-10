using System.Text.Json.Serialization;

namespace DeeperDeepDungeonDex.Storage;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum Status {
    Bind,
    Heavy,
    Sleep,
    Slow,
    Stun,
    Resolution
}
