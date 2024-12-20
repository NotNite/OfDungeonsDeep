using System;
using System.Globalization;
using System.Threading.Tasks;
using Dalamud.Plugin;
using FFXIVClientStructs.FFXIV.Client.Game.Event;
using FFXIVClientStructs.FFXIV.Client.Game.InstanceContent;
using Lumina.Excel.Sheets;
using OfDungeonsDeep.Components;
using OfDungeonsDeep.Controllers;
using OfDungeonsDeep.Storage;

namespace OfDungeonsDeep;

public sealed class Plugin : IDalamudPlugin {
    public static Configuration Configuration = null!;
    public static StorageManager StorageManager = null!;
    public static OfDungeonsDeepController Controller = null!;

    public Plugin(IDalamudPluginInterface pluginInterface) {
        Strings.Culture = new CultureInfo(pluginInterface.UiLanguage);

        pluginInterface.Create<Services>();
        Configuration = Services.PluginInterface.GetPluginConfig() as Configuration ?? new Configuration();

        StorageManager = new StorageManager();
        Task.Run(StorageManager.Load).ContinueWith(_ => {
            Controller = new OfDungeonsDeepController();
        });
    }

    public void Dispose() {
        Controller.Dispose();
    }

    public static unsafe InstanceContentDeepDungeon* GetDirector() {
        var eventFramework = EventFramework.Instance();
        return eventFramework == null ? null : eventFramework->GetInstanceContentDeepDungeon();
    }

    public static unsafe bool InDeepDungeon() => GetDirector() != null;

    public static unsafe byte? GetFloor() {
        var director = GetDirector();
        if (director is null) return null;
        return director->Floor;
    }
    
    public static uint? GetFloorSetId() {
        if (GetFloor() is { } floor) {
            return (uint) ((floor - 1) / 10 * 10 + 1);
        }
        
        return null;
    }

    public static int GetFloorSetId(int floor) {
        return (floor - 1) / 10 * 10 + 1;
    }

    public static DeepDungeonType? GetDeepDungeonType() {
        if (Services.DataManager.GetExcelSheet<TerritoryType>()?.GetRow(Services.ClientState.TerritoryType) is { } territoryInfo) {
            return territoryInfo switch {
                { TerritoryIntendedUse.Value.RowId: 31, ExVersion.RowId: 0 or 1 } => DeepDungeonType.PalaceOfTheDead,
                { TerritoryIntendedUse.Value.RowId: 31, ExVersion.RowId: 2 } => DeepDungeonType.HeavenOnHigh,
                { TerritoryIntendedUse.Value.RowId: 31, ExVersion.RowId: 4 } => DeepDungeonType.EurekaOrthos,
                _ => null
            };
        }

        return null;
    }
    
    public static string GetEnemyName(IDrawableMob enemy) {
        if (Services.DataManager.GetExcelSheet<BNpcName>() is { } bnpcNameSheet) {
            return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(bnpcNameSheet.GetRow(enemy.Id)!.Singular.ExtractText());
        }

        throw new Exception($"Exception trying to get mob name from enemy #{enemy.Id}");
    }
}
