using System;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Reflection;
using Dalamud.Interface.Internal;
using Dalamud.Interface.Utility;
using DeeperDeepDungeonDex.Storage;
using ImGuiNET;
using Status = Lumina.Excel.GeneratedSheets2.Status;

namespace DeeperDeepDungeonDex.System;

public class MobDataWindow : DeepDungeonWindow {
    public Enemy Enemy { get; }
    private IDalamudTextureWrap? mobImageSmall;
    private IDalamudTextureWrap? mobImageLarge;

    public MobDataWindow(string name, Enemy enemy) : base(name) {
        Enemy = enemy;
        
        SizeConstraints = new WindowSizeConstraints {
            MinimumSize = new Vector2(300.0f, 200.0f),
            MaximumSize = new Vector2(float.PositiveInfinity),
        };

        if (Enemy.Image is not null) {
            var assemblyDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!;

            mobImageLarge = Services.TextureProvider.GetTextureFromFile(new FileInfo(GetImagePath(assemblyDir, "Images")));
            mobImageSmall = Services.TextureProvider.GetTextureFromFile(new FileInfo( GetImagePath(assemblyDir, "Thumbnails")));
        }
    }
    
    private string GetImagePath(string assemblyDir, string folder) {
        if (Enemy.Image is null) return string.Empty;
        
        return Path.Combine(
            assemblyDir,
            "Data",
            folder,
            Plugin.GetDeepDungeonType() switch {
                DeepDungeonType.PalaceOfTheDead => "potd",
                DeepDungeonType.HeavenOnHigh => "hoh",
                DeepDungeonType.EurekaOrthos => "eo",
                _ => throw new ArgumentOutOfRangeException()
            },
            Plugin.GetFloorSetId()?.ToString("000") ?? "000",
            Enemy.Image
        );
    }
    
    public override void Draw() {
        base.Draw();
        
        const float portraitHeight = 75.0f;
        
        var portraitSize = new Vector2(ImGui.GetContentRegionMax().X * 0.25f, portraitHeight * ImGuiHelpers.GlobalScale);
        if (ImGui.BeginChild("MobPortrait", portraitSize, false)) {
            DrawPortrait();
        }
        ImGui.EndChild();
        
        ImGui.SameLine();
        var basicInfoSize = new Vector2(ImGui.GetContentRegionAvail().X, portraitHeight * ImGuiHelpers.GlobalScale);
        if (ImGui.BeginChild("BasicInfo", basicInfoSize, false)) {
            var nameSize = new Vector2(ImGui.GetContentRegionAvail().X, ImGui.GetTextLineHeight());
            if (ImGui.BeginChild("MobName", nameSize, false)) {
                DrawMobName();
            }
            ImGui.EndChild();

            var mobIdSize = new Vector2(ImGui.GetContentRegionAvail().X, ImGui.GetTextLineHeight());
            ImGui.SetCursorPosX(ImGui.GetContentRegionAvail().X - mobIdSize.X);
            if (ImGui.BeginChild("MobId", mobIdSize, false)) {
                DrawMobHpAndId();
            }
            ImGui.EndChild();

            var mobVulnerabilitiesSize = new Vector2(ImGui.GetContentRegionAvail().X, 32.0f * ImGuiHelpers.GlobalScale);
            if (ImGui.BeginChild("MobVulnerabilities", mobVulnerabilitiesSize, false)) {
                DrawVulnerabilities();
            }
            ImGui.EndChild();
        }
        ImGui.EndChild();

        var mobDataSize = new Vector2(ImGui.GetContentRegionAvail().X, 100.0f);
        if (ImGui.BeginChild("MobData", mobDataSize, true)) {
            
        }
        ImGui.EndChild();

        //
        // ImGui.TextUnformatted($"Name: {Plugin.GetEnemyName(Enemy)}");
        // ImGui.TextUnformatted($"Aggro: {this.Enemy.Aggro}");
        //
        // if (this.Enemy.AttackType is not null)
        //     ImGui.TextUnformatted($"Attack type: {this.Enemy.AttackType}");
        //
        // var vulnerabilities = this.Enemy.Vulnerabilities
        //     .Where(x => x.Value)
        //     .Select(x => x.Key);
        //
        // ImGui.TextUnformatted($"Weakness: {string.Join(", ", vulnerabilities)}");
    }
    
    private void DrawVulnerabilities() {
        foreach (var (status, isVulnerable) in Enemy.Vulnerabilities) {
            if (Services.TextureProvider.GetIcon((uint) status) is { } image) {
                ImGui.Image(image.ImGuiHandle, image.Size * ImGuiHelpers.GlobalScale * 0.50f, Vector2.Zero, Vector2.One, isVulnerable ? Vector4.One : Vector4.One with { W = 0.33f });

                if (ImGui.IsItemHovered() && Services.DataManager.GetExcelSheet<Status>()?.FirstOrDefault(statusEffect => statusEffect.Icon == (uint)status) is {} statusInfo ) {
                    ImGui.SetTooltip(statusInfo.Name);
                }
                ImGui.SameLine();
            }
        }
    }
    
    private void DrawMobHpAndId() {
        ImGui.Text($"HP {Enemy.Hp:n0}");
        ImGui.SameLine();
        
        var mobId = $"Id {Enemy.Id.ToString()}";
        var mobIdTextSize = ImGui.CalcTextSize(mobId);
        ImGui.SetCursorPosX(ImGui.GetContentRegionMax().X - mobIdTextSize.X);
        ImGui.Text(mobId);
    }

    private void DrawPortrait() {
        if (mobImageSmall is not null && mobImageLarge is not null) {
            ImGui.Image(mobImageSmall.ImGuiHandle, ImGui.GetContentRegionAvail());
            if (ImGui.IsItemClicked()) {
                ImGui.OpenPopup("ImagePopup");
            }

            if (ImGui.BeginPopup("ImagePopup")) {
                ImGui.Image(mobImageLarge.ImGuiHandle, mobImageLarge.Size);
                ImGui.EndPopup();
            } else if (ImGui.IsItemHovered()) {
                ImGui.SetTooltip("Click to see Full Resolution");
            }
        }
    }
    
    private void DrawMobName() {
        ImGui.Text(Plugin.GetEnemyName(Enemy));
    }

    public override void OnClose() {
        Plugin.Controller.WindowController.RemoveWindow(this);
    }
}
