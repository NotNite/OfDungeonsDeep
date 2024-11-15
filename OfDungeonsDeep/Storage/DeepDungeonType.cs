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
        DeepDungeonType.PalaceOfTheDead => Services.DataManager.GetExcelSheet<JournalGenre>()?.GetRow(96).Name.ExtractText() ?? string.Empty,
        DeepDungeonType.HeavenOnHigh => Services.DataManager.GetExcelSheet<JournalGenre>()?.GetRow(97).Name.ExtractText() ?? string.Empty,
        DeepDungeonType.EurekaOrthos => Services.DataManager.GetExcelSheet<JournalGenre>()?.GetRow(98).Name.ExtractText() ?? string.Empty,
        _ => string.Empty
    };
}
