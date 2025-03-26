using System.Text.Json.Serialization;
using Lumina.Excel.Sheets;

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
        DeepDungeonType.PalaceOfTheDead => Services.DataManager.GetExcelSheet<JournalGenre>()?.GetRow(99).Name.ExtractText() ?? string.Empty,
        DeepDungeonType.HeavenOnHigh => Services.DataManager.GetExcelSheet<JournalGenre>()?.GetRow(100).Name.ExtractText() ?? string.Empty,
        DeepDungeonType.EurekaOrthos => Services.DataManager.GetExcelSheet<JournalGenre>()?.GetRow(101).Name.ExtractText() ?? string.Empty,
        _ => string.Empty
    };
}
