using System.Linq;
using DeeperDeepDungeonDex.Storage;
using ImGuiNET;

namespace DeeperDeepDungeonDex.System;

public class MobDataWindow(string name, Enemy enemy) : DeepDungeonWindow(name) {
    public Enemy Enemy { get; } = enemy;
    
    public override void Draw() {
        ImGui.TextUnformatted($"Name: {Plugin.GetEnemyName(Enemy)}");
        ImGui.TextUnformatted($"Aggro: {this.Enemy.Aggro}");

        if (this.Enemy.AttackType is not null)
            ImGui.TextUnformatted($"Attack type: {this.Enemy.AttackType}");

        var vulnerabilities = this.Enemy.Vulnerabilities
            .Where(x => x.Value)
            .Select(x => x.Key);

        ImGui.TextUnformatted($"Weakness: {string.Join(", ", vulnerabilities)}");
    }

    public override void OnClose() {
        Plugin.Controller.WindowController.RemoveWindow(this);
    }
}
