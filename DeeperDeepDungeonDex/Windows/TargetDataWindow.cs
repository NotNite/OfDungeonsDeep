using System.Linq;
using Dalamud.Game.ClientState.Objects.Enums;
using Dalamud.Game.ClientState.Objects.Types;
using Dalamud.Interface;
using DeeperDeepDungeonDex.Storage;
using FFXIVClientStructs.FFXIV.Common.Math;
using ImGuiNET;

namespace DeeperDeepDungeonDex.System;

public class TargetDataWindow : DeepDungeonWindow {
    private EnemyView? enemyView;

    public TargetDataWindow() : base("##DeeperDeepDungeonDex_TargetDataWindow") {
        SizeConstraints = new WindowSizeConstraints {
            MinimumSize = new Vector2(325.0f, 250.0f),
            MaximumSize = new Vector2(325.0f, 250.0f),
        };

        Flags |= ImGuiWindowFlags.NoResize;
        Flags |= ImGuiWindowFlags.NoTitleBar;

        IsOpen = true;
    }

    public override bool DrawConditions() {
        if (!Plugin.InDeepDungeon()) return false;
        if (!Plugin.StorageManager.DataReady) return false;
        if (Services.TargetManager.Target is null) return false;
        if (Services.TargetManager.Target is not BattleNpc { BattleNpcKind: BattleNpcSubKind.Enemy }) return false;

        return true;
    }

    public override void Draw() {
        base.Draw();
        
        enemyView?.Draw();
    }

    public void UpdateTarget(Enemy enemy) {
        enemyView = new EnemyView(enemy, FontAwesomeIcon.ArrowUpRightFromSquare, () => Plugin.Controller.WindowController.TryAddMobDataWindow(enemy));
    }
}
