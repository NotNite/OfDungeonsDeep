using System.Text.Json.Serialization;
using Lumina.Excel.GeneratedSheets2;

namespace OfDungeonsDeep.Storage;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum DeepDungeonType {
    Unknown,
    PalaceOfTheDead,
    HeavenOnHigh,
    EurekaOrthos,
}

public static class DeepDungeonTypeExtensions {
    public static string Localized(this DeepDungeonType type) => type switch {
        DeepDungeonType.PalaceOfTheDead => Services.DataManager.GetExcelSheet<JournalGenre>()?.GetRow(90)?.Name ?? string.Empty,
        DeepDungeonType.HeavenOnHigh => Services.DataManager.GetExcelSheet<JournalGenre>()?.GetRow(91)?.Name ?? string.Empty,
        DeepDungeonType.EurekaOrthos => Services.DataManager.GetExcelSheet<JournalGenre>()?.GetRow(92)?.Name ?? string.Empty,
        _ => string.Empty
    };
}
