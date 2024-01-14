using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Reflection;
using Dalamud.Interface;
using Dalamud.Interface.Components;
using Dalamud.Interface.Internal;
using Dalamud.Interface.Utility;
using DeeperDeepDungeonDex.Storage;
using ImGuiNET;
using Status = Lumina.Excel.GeneratedSheets2.Status;

namespace DeeperDeepDungeonDex.System;

public class EnemyView {
    private readonly Enemy enemy;
    private readonly IDalamudTextureWrap? mobImageSmall;
    private readonly IDalamudTextureWrap? mobImageLarge;
    private readonly Action buttonAction;
    private readonly FontAwesomeIcon buttonIcon;
    
    public EnemyView(Enemy enemyData, FontAwesomeIcon buttonIcon, Action onButtonAction) {
        enemy = enemyData;
        buttonAction = onButtonAction;
        this.buttonIcon = buttonIcon;
        
        if (enemy.Image is not null) {
            mobImageLarge = Services.TextureProvider.GetTextureFromFile(new FileInfo(GetImagePath("Images")));
            mobImageSmall = Services.TextureProvider.GetTextureFromFile(new FileInfo( GetImagePath("Thumbnails")));
        }
    }

    public virtual void Draw() {
        const float portraitHeight = 85.0f;
        var portraitSize = new Vector2(ImGui.GetContentRegionMax().X * 0.25f, portraitHeight * ImGuiHelpers.GlobalScale);
        if (ImGui.BeginChild("MobPortrait", portraitSize, false)) {
            DrawPortrait();
        }
        ImGui.EndChild();
        
        ImGui.SameLine();
        var basicInfoSize = new Vector2(ImGui.GetContentRegionAvail().X, portraitHeight * ImGuiHelpers.GlobalScale);
        if (ImGui.BeginChild("BasicInfo", basicInfoSize, false)) {
            if (ImGui.BeginTable("NameCloseTable", 2)) {
                ImGui.TableSetupColumn("##Name", ImGuiTableColumnFlags.WidthStretch);
                ImGui.TableSetupColumn("##Close", ImGuiTableColumnFlags.WidthFixed, 25.0f * ImGuiHelpers.GlobalScale);

                ImGui.TableNextColumn();
                var nameSize = new Vector2(ImGui.GetContentRegionAvail().X, ImGui.GetTextLineHeight() + 5.0f * ImGuiHelpers.GlobalScale);
                if (ImGui.BeginChild("MobName", nameSize, false, ImGuiWindowFlags.NoScrollWithMouse | ImGuiWindowFlags.NoScrollbar)) {
                    ImGui.AlignTextToFramePadding();
                    DrawMobName();
                }
                ImGui.EndChild();

                ImGui.TableNextColumn();
                if (ImGuiComponents.IconButton("Button", buttonIcon)) {
                    buttonAction();
                }
                
                ImGui.EndTable();
            }

            var mobIdSize = new Vector2(ImGui.GetContentRegionAvail().X, ImGui.GetTextLineHeight());
            ImGui.SetCursorPosX(ImGui.GetContentRegionAvail().X - mobIdSize.X);
            if (ImGui.BeginChild("MobId", mobIdSize, false, ImGuiWindowFlags.NoScrollWithMouse)) {
                DrawBasicMobData();
            }
            ImGui.EndChild();

            var mobVulnerabilitiesSize = new Vector2(ImGui.GetContentRegionAvail().X, 32.0f * ImGuiHelpers.GlobalScale);
            if (ImGui.BeginChild("MobVulnerabilities", mobVulnerabilitiesSize, false)) {
                DrawVulnerabilities();
            }
            ImGui.EndChild();
        }
        ImGui.EndChild();

        ImGuiHelpers.ScaledDummy(5.0f);
        
        var mobDataSize = new Vector2(ImGui.GetContentRegionAvail().X, 65.0f * ImGuiHelpers.GlobalScale);
        if (ImGui.BeginChild("MobData", mobDataSize, false)) {
            DrawMobData();
        }
        ImGui.EndChild();
    }
    
        private void DrawMobData() {
        if (ImGui.BeginTable("DataTable", 2, ImGuiTableFlags.SizingStretchProp)) {
            ImGui.TableNextColumn();
            ImGui.Text("Floors");

            ImGui.TableNextColumn();
            ImGui.Text(string.Join(", ",  Enumerable.Range(enemy.StartFloor, enemy.EndFloor - enemy.StartFloor + 1)));

            ImGui.TableNextColumn();
            ImGui.Text("Aggro");

            ImGui.TableNextColumn();
            ImGui.Text(enemy.Aggro.ToString());

            if (enemy.AttackName is not null) {
                ImGui.TableNextColumn();
                ImGui.Text("Attack");

                ImGui.TableNextColumn();
                ImGui.Text(enemy.AttackName);
            }
            
            ImGui.EndTable();
        }
    }

    private void DrawVulnerabilities() {
        foreach (var (status, isVulnerable) in enemy.Vulnerabilities) {
            if (Services.TextureProvider.GetIcon((uint) status) is { } image) {
                ImGui.Image(image.ImGuiHandle, image.Size * ImGuiHelpers.GlobalScale * 0.50f, Vector2.Zero, Vector2.One, isVulnerable ? Vector4.One : Vector4.One / 2.5f );

                if (ImGui.IsItemHovered() && Services.DataManager.GetExcelSheet<Status>()?.FirstOrDefault(statusEffect => statusEffect.Icon == (uint)status) is {} statusInfo ) {
                    ImGui.SetTooltip(statusInfo.Name);
                }
                ImGui.SameLine();
            }
        }
    }
    
    private void DrawBasicMobData() {
        if (ImGui.BeginTable("BasicInfoTable", 3, ImGuiTableFlags.SizingStretchSame, ImGui.GetContentRegionAvail())) {
            ImGui.TableNextColumn();
            ImGui.Text($"{enemy.Family ?? string.Empty}");

            ImGui.TableNextColumn();
            ImGui.Text($"HP {enemy.Hp ?? 0}");
            
            ImGui.TableNextColumn();
            var idText = $"ID {enemy.Id}";
            var idTextSize = ImGui.CalcTextSize(idText);
            ImGui.SetCursorPosX(ImGui.GetContentRegionMax().X - idTextSize.X);
            ImGui.Text(idText);
            
            ImGui.EndTable();
        }
    }

    private void DrawPortrait() {
        if (mobImageSmall is not null && mobImageLarge is not null) {
            var rectPosition = ImGui.GetCursorScreenPos();
            ImGui.Image(mobImageSmall.ImGuiHandle, ImGui.GetContentRegionAvail());
            ImGui.GetWindowDrawList().AddRect(rectPosition, rectPosition + ImGui.GetContentRegionMax(), ImGui.GetColorU32(KnownColor.White.Vector() with { W = 0.75f }));
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
        ImGui.Text(Plugin.GetEnemyName(enemy));
    }
    
    private string GetImagePath(string folder) {
        if (enemy.Image is null) return string.Empty;
        
        return Path.Combine(
            Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!,
            "Data",
            folder,
            Plugin.GetDeepDungeonType() switch {
                DeepDungeonType.PalaceOfTheDead => "potd",
                DeepDungeonType.HeavenOnHigh => "hoh",
                DeepDungeonType.EurekaOrthos => "eo",
                _ => throw new ArgumentOutOfRangeException(nameof(folder))
            },
            Plugin.GetFloorSetId()?.ToString("000") ?? "000",
            enemy.Image
        );
    }
}
