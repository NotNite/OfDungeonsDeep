using System.Text.Json.Serialization;
using Lumina.Excel.Sheets;

namespace OfDungeonsDeep.Storage;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum DeepDungeonType {
    Unknown,
    PalaceOfTheDead,
    HeavenOnHigh,
    EurekaOrthos,
    PilgrimsTraverse,
}

public static class DeepDungeonTypeExtensions {
    public static string Localized(this DeepDungeonType type) => type switch {
        DeepDungeonType.PalaceOfTheDead => Services.DataManager.GetExcelSheet<JournalGenre>()?.GetRow(102).Name.ExtractText() ?? string.Empty,
        DeepDungeonType.HeavenOnHigh => Services.DataManager.GetExcelSheet<JournalGenre>()?.GetRow(103).Name.ExtractText() ?? string.Empty,
        DeepDungeonType.EurekaOrthos => Services.DataManager.GetExcelSheet<JournalGenre>()?.GetRow(104).Name.ExtractText() ?? string.Empty,
        DeepDungeonType.PilgrimsTraverse=> Services.DataManager.GetExcelSheet<JournalGenre>()?.GetRow(105).Name.ExtractText() ?? string.Empty,
        _ => string.Empty
    };
}
