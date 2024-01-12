using System;
using System.Collections.Generic;
using System.Linq;
using Dalamud.Game.ClientState.Objects.Enums;
using Dalamud.Game.ClientState.Objects.Types;
using Dalamud.Interface.Windowing;
using Dalamud.Plugin.Services;
using DeeperDeepDungeonDex.Storage;
using ImGuiNET;

namespace DeeperDeepDungeonDex.Windows;

public class MobWindow : Window, IDisposable {
    private GameObject? target;
    private Enemy? targetInfo;

    public MobWindow() : base("##DDDD_MobWindow") {
        this.IsOpen = true;
        this.RespectCloseHotkey = false;
        this.ShowCloseButton = false;

        Services.Framework.Update += this.FrameworkUpdate;
    }

    public void Dispose() {
        Services.Framework.Update -= this.FrameworkUpdate;
    }

    private void FrameworkUpdate(IFramework framework) {
        var floorset = Plugin.GetFloorsetId();
        var type = Plugin.GetDeepDungeonType();
        if (
            Services.TargetManager.Target is BattleNpc {BattleNpcKind: BattleNpcSubKind.Enemy} bnpc
            && bnpc.IsValid()
            && floorset is not null
            && type is not null
            && Plugin.StorageManager.Enemies[type.Value].TryGetValue(floorset.Value, out var enemies)
        ) {
            var enemy = enemies.FirstOrDefault(x => x.Id == bnpc.NameId);
            this.target = bnpc;
            this.targetInfo = enemy;
        } else {
            this.target = null;
            this.targetInfo = null;
        }
    }

    public override bool DrawConditions() => this.target != null && this.targetInfo != null;

    public override void Draw() {
        if (this.target == null || this.targetInfo == null) return;
        ImGui.TextUnformatted($"Name: {this.target.Name.TextValue}");
        ImGui.TextUnformatted($"Aggro: {this.targetInfo.Aggro}");

        if (this.targetInfo.AttackType is not null)
            ImGui.TextUnformatted($"Attack type: {this.targetInfo.AttackType}");

        var vulns = this.targetInfo.Vulnerabilities
            .Where(x => x.Value)
            .Select(x => x.Key);

        ImGui.TextUnformatted($"Weakness: {string.Join(", ", vulns)}");
    }
}
