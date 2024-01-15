using System.Numerics;
using ImGuiNET;

namespace DeeperDeepDungeonDex.System;

public class MobDataWindow : DeepDungeonWindow {
    private readonly IDrawableMob enemy;

    public MobDataWindow(string name, IDrawableMob enemyData) : base(name) {
        enemy = enemyData;
        
        SizeConstraints = new WindowSizeConstraints {
            MinimumSize = new Vector2(325.0f, 250.0f),
            MaximumSize = new Vector2(325.0f, 250.0f),
        };

        Flags |= ImGuiWindowFlags.NoResize;
        Flags |= ImGuiWindowFlags.NoTitleBar;
    }

    public override void Draw() {
        base.Draw();
        enemy.Draw(false, WindowExtraButton.Close);
    }

    public override void OnClose() {
        Plugin.Controller.WindowController.RemoveWindow(this);
    }
}
