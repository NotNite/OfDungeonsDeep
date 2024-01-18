using System.Numerics;
using ImGuiNET;

namespace DeeperDeepDungeonDex.System;

public class MobDataWindow : DeepDungeonWindow {
    private readonly IDrawableMob enemy;

    public MobDataWindow(string name, IDrawableMob enemyData) : base(name) {
        enemy = enemyData;
        
        SizeConstraints = new WindowSizeConstraints {
            MinimumSize = new Vector2(325.0f, 125.0f),
            MaximumSize = new Vector2(float.PositiveInfinity),
        };

        Flags |= ImGuiWindowFlags.NoTitleBar;
    }

    public override void Draw() {
        base.Draw();
        enemy.Draw(WindowExtraButton.Close);
    }

    public override void OnClose() {
        Plugin.Controller.WindowController.RemoveWindow(this);
    }
}
