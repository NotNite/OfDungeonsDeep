using System.Text.Json.Serialization;

namespace DeeperDeepDungeonDex.Storage;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum Difficulty {
    Easy,
    Medium,
    Hard,
    Extreme,
    Unrated
}
