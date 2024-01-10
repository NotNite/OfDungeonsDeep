using System;
using Dalamud.Game.ClientState.Objects.Enums;
using Dalamud.Game.ClientState.Objects.Types;
using Dalamud.Interface.Windowing;
using Dalamud.Plugin.Services;
using DeeperDeepDungeonDex.Storage;
using ImGuiNET;

namespace DeeperDeepDungeonDex.Windows;

public class MobWindow : Window, IDisposable {
    private GameObject? target;
    private Mob? targetInfo;

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
        if (Services.TargetManager.Target is BattleNpc {BattleNpcKind: BattleNpcSubKind.Enemy} bnpc
            && bnpc.IsValid()
            && Plugin.InDeepDungeon()
            && Plugin.StorageManager.Mobs.TryGetValue(bnpc.NameId, out var mob)) {
            this.target = bnpc;
            this.targetInfo = mob;
        } else {
            this.target = null;
            this.targetInfo = null;
        }
    }

    public override bool DrawConditions() => this.target != null && this.targetInfo != null;

    public override void Draw() {
        if (this.target == null || this.targetInfo == null) return;
        ImGui.TextUnformatted($"Name: {this.target.Name.TextValue}");
        ImGui.TextUnformatted($"Threat: {this.targetInfo.Threat}");
        ImGui.TextUnformatted($"Aggro: {this.targetInfo.Aggro}");
        ImGui.TextUnformatted($"Weakness: {string.Join(", ", this.targetInfo.Weakness)}");
    }
}
