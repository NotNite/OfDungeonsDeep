using System;
using System.Linq;
using Dalamud.Game.ClientState.Objects.Enums;
using Dalamud.Game.ClientState.Objects.Types;
using Dalamud.Plugin.Services;
using DeeperDeepDungeonDex.Storage;

namespace DeeperDeepDungeonDex.System;

public class DeeperDeepDungeonDexController : IDisposable {
    public WindowController WindowController;
    
    private uint currentFloor;
    private uint currentFloorSet;
    private DeepDungeonType dungeonType;
    private BattleNpc? lastFrameGameObject;
    
    public DeeperDeepDungeonDexController() {
        WindowController = new WindowController();
        
        Services.DutyState.DutyStarted += OnDutyStarted;
        Services.Framework.Update += OnFrameworkUpdate;

        if (Plugin.InDeepDungeon()) {
            OnDutyStarted(this, Services.ClientState.TerritoryType);
        }
    }
    
    public void Dispose() {
        WindowController.Dispose();
        
        Services.DutyState.DutyStarted -= OnDutyStarted;
        Services.Framework.Update -= OnFrameworkUpdate;
    }
    
    private void OnDutyStarted(object? sender, ushort e) {
        if (!Plugin.InDeepDungeon()) return;
        if (!Plugin.StorageManager.DataReady) return;

        UpdateData();

        if (Plugin.StorageManager.Floorsets.TryGetValue(dungeonType, out var floorSets)) {
            if (floorSets.TryGetValue(currentFloorSet, out var floorSetData)) {
                WindowController.TryAddDataWindow(floorSetData);
            }
        }
    }

    private void UpdateData() {
        currentFloor = Plugin.GetFloor() ?? 0;
        currentFloorSet = Plugin.GetFloorSetId() ?? 0;
        dungeonType = Plugin.GetDeepDungeonType() ?? DeepDungeonType.Unknown;
    }

    private void OnFrameworkUpdate(IFramework framework) {
        // Do absolutely nothing if we aren't in a Deep Dungeon.
        // This shouldn't really need to be stated, but times are changing I guess.
        if (!Plugin.InDeepDungeon()) return;

        // If the data hasn't finished loading, don't try to access it.
        if (!Plugin.StorageManager.DataReady) return;

        if (Services.TargetManager.Target is BattleNpc { BattleNpcKind: BattleNpcSubKind.Enemy } currentTarget) {
            if (lastFrameGameObject is null || (lastFrameGameObject is not null && currentTarget.NameId != lastFrameGameObject.NameId)) {
                UpdateTarget(currentTarget);
            }
        }
        
        lastFrameGameObject = Services.TargetManager.Target as BattleNpc;
    }
    
    private void UpdateTarget(BattleNpc currentTarget) {
        if (currentFloor is 0) return;
        if (dungeonType is DeepDungeonType.Unknown) return;
        if (currentFloorSet is 0) return;
        if (!currentTarget.IsValid()) return;
        if (!Plugin.StorageManager.Enemies[dungeonType].TryGetValue(currentFloorSet, out var enemies)) return;
        if (enemies.FirstOrDefault(enemy => enemy.Id == currentTarget.NameId) is not { } enemyData) return;
        
        WindowController.TargetDataWindow.UpdateTarget(enemyData);
    }
}
