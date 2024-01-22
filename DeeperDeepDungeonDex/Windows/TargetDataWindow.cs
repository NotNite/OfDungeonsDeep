using System.Numerics;
using Dalamud.Game.ClientState.Objects.Enums;
using Dalamud.Game.ClientState.Objects.Types;
using ImGuiNET;

namespace DeeperDeepDungeonDex.System;

public class TargetDataWindow : DeepDungeonWindow {
    private IDrawableMob? targetEnemy;

    public TargetDataWindow() : base("##DeeperDeepDungeonDex_TargetDataWindow") {
        SizeConstraints = new WindowSizeConstraints {
            MinimumSize = new Vector2(325.0f, 200.0f),
            MaximumSize = new Vector2(float.PositiveInfinity),
        };

        Flags |= ImGuiWindowFlags.NoTitleBar;
        IsOpen = true;
    }

    public override bool DrawConditions() {
        if (Plugin.Configuration is {EnableTargetWindow: false}) return false;
        if (!Plugin.InDeepDungeon()) return false;
        if (!Plugin.StorageManager.DataReady) return false;
        if (Services.TargetManager.Target is null) return false;
        if (Services.TargetManager.Target is not BattleNpc { BattleNpcKind: BattleNpcSubKind.Enemy }) return false;
        if (Services.ClientState.LocalPlayer is {IsDead: true}) return false;

        return true;
    }
    
    public override void PreDraw() {
        if (Plugin.Configuration.LockTargetWindow) {
            Flags |= ImGuiWindowFlags.NoMove;
            Flags |= ImGuiWindowFlags.NoResize;
        } else {
            Flags &= ~ImGuiWindowFlags.NoMove;
            Flags &= ~ImGuiWindowFlags.NoResize;
        }
    }

    public override void Draw() {
        base.Draw();
        
        targetEnemy?.Draw(WindowExtraButton.PopOutWithLock);
    }

    public void UpdateTarget(IDrawableMob enemy) {
        targetEnemy = enemy;
    }
}
