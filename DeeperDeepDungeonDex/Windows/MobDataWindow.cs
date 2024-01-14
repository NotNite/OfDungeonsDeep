using System.Numerics;
using Dalamud.Interface;
using DeeperDeepDungeonDex.Storage;
using ImGuiNET;

namespace DeeperDeepDungeonDex.System;

public class MobDataWindow : DeepDungeonWindow {
    private readonly EnemyView enemyView;

    public MobDataWindow(string name, Enemy enemyData) : base(name) {
        enemyView = new EnemyView(enemyData, FontAwesomeIcon.Times, OnClose);
        
        SizeConstraints = new WindowSizeConstraints {
            MinimumSize = new Vector2(325.0f, 185.0f),
            MaximumSize = new Vector2(325.0f, 185.0f),
        };

        Flags |= ImGuiWindowFlags.NoResize;
        Flags |= ImGuiWindowFlags.NoTitleBar;
    }

    public override void Draw() {
        base.Draw();
        enemyView.Draw();
    }

    public override void OnClose() {
        Plugin.Controller.WindowController.RemoveWindow(this);
    }
}
