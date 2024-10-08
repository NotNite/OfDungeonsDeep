using System;
using System.Linq;
using Dalamud.Game.ClientState.Objects.Enums;
using Dalamud.Game.ClientState.Objects.Types;
using Dalamud.Plugin.Services;
using OfDungeonsDeep.Storage;

namespace OfDungeonsDeep.Controllers;

public class OfDungeonsDeepController : IDisposable {
    public WindowController WindowController;
    
    private uint currentFloor;
    private uint currentFloorSet;
    private DeepDungeonType dungeonType;
    private IBattleNpc? lastFrameGameObject;

    public OfDungeonsDeepController() {
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
        
    private void OnFloorChanged() {
        if (!Plugin.InDeepDungeon()) return;
        if (!Plugin.StorageManager.DataReady) return;
        if (!Plugin.Configuration.ShowFloorEveryFloor) return;
        
        TryShowFloorInfo();
    }
    
    private void OnDutyStarted(object? sender, ushort e) {
        if (!Plugin.InDeepDungeon()) return;
        if (!Plugin.StorageManager.DataReady) return;

        TryShowFloorInfo();
    }

    private void UpdateData() {
        currentFloorSet = Plugin.GetFloorSetId() ?? 0;
        dungeonType = Plugin.GetDeepDungeonType() ?? DeepDungeonType.Unknown;
        
        if (Plugin.GetFloor() is {} floor && currentFloor != floor) {
            currentFloor = floor;
            OnFloorChanged();
        }
    }

    private void OnFrameworkUpdate(IFramework framework) {
        // Do absolutely nothing if we aren't in a Deep Dungeon.
        // This shouldn't really need to be stated, but times are changing I guess.
        if (!Plugin.InDeepDungeon()) return;

        // If the data hasn't finished loading, don't try to access it.
        if (!Plugin.StorageManager.DataReady) return;

        UpdateData();

        if (Services.TargetManager.Target is IBattleNpc { BattleNpcKind: BattleNpcSubKind.Enemy } currentTarget) {
            if (lastFrameGameObject is null || (lastFrameGameObject is not null && currentTarget.EntityId != lastFrameGameObject.EntityId)) {
                UpdateTarget(currentTarget);
            }
        }
        
        lastFrameGameObject = Services.TargetManager.Target as IBattleNpc;
    }
    
    private void UpdateTarget(IBattleNpc currentTarget) {
        if (currentFloor is 0) return;
        if (dungeonType is DeepDungeonType.Unknown) return;
        if (currentFloorSet is 0) return;
        if (!currentTarget.IsValid()) return;
        
        if (Plugin.StorageManager.Enemies.TryGetValue(dungeonType, out var dungeonEnemies)) {
            if (dungeonEnemies.TryGetValue(currentFloorSet, out var enemies)) {
                if (enemies.FirstOrDefault(enemy => enemy.Id == currentTarget.NameId) is { } enemyData) {
                    WindowController.TargetDataWindow.UpdateTarget(enemyData);
                    return;
                }
            }
        }

        if (Plugin.StorageManager.Floorsets.TryGetValue(dungeonType, out var bossEnemies)) {
            if (bossEnemies.TryGetValue(currentFloorSet, out var floorBoss)) {
                if (floorBoss.Id == currentTarget.NameId) {
                    WindowController.TargetDataWindow.UpdateTarget(floorBoss);
                    return;
                }
            }
        }
    }

    private void TryShowFloorInfo() {
        if (Plugin.StorageManager.Floorsets.TryGetValue(dungeonType, out var floorSets)) {
            if (floorSets.TryGetValue(currentFloorSet, out var floorSetData)) {
                WindowController.ShowFloorSetData(floorSetData);
            }
        }
    }
}
