using System.Text.Json.Serialization;

namespace OfDungeonsDeep.Storage;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum Status : uint {
    Bind = 215003,
    Heavy = 215002,
    Sleep = 215013,
    Slow = 215009,
    Stun = 215004,
    Resolution = 27990,
}
