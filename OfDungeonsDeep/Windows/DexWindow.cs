using System;
using System.Drawing;
using System.Linq;
using System.Numerics;
using Dalamud.Interface;
using Dalamud.Interface.Utility;
using Dalamud.Interface.Windowing;
using ImGuiNET;
using OfDungeonsDeep.Components;
using OfDungeonsDeep.Storage;

namespace OfDungeonsDeep.Controllers;

public class DexWindow : DeepDungeonWindow {
    private IDrawableMob? selectedEnemy;
    private DeepDungeonType dungeonType = DeepDungeonType.PalaceOfTheDead;
    private uint floorSet;
    
    public DexWindow() : base("OfDungeonsDeep - Monster Dex") {
        SizeConstraints = new Window.WindowSizeConstraints {
            MinimumSize = new Vector2(600.0f, 400.0f),
            MaximumSize = new Vector2(float.PositiveInfinity),
        };

        Flags |= ImGuiWindowFlags.NoScrollbar;
        Flags |= ImGuiWindowFlags.NoScrollWithMouse;
    }

    public override bool DrawConditions() {
        if (!Plugin.StorageManager.DataReady) return false;

        return true;
    }

    public override void Draw() {
        base.Draw();

        if (ImGui.BeginTable("ResizableTable", 2, ImGuiTableFlags.Resizable, ImGui.GetContentRegionAvail())) {
            ImGui.TableSetupColumn("##SelectableSide", ImGuiTableColumnFlags.WidthFixed, ImGui.GetContentRegionAvail().X / 3.0f);
            ImGui.TableSetupColumn("##ContentsSide", ImGuiTableColumnFlags.WidthStretch);
            
            ImGui.TableNextColumn();
            if (ImGui.BeginChild("SelectableChild", ImGui.GetContentRegionAvail(), false)) {
                var selectablesSize = new Vector2(ImGui.GetContentRegionAvail().X, ImGui.GetContentRegionAvail().Y - 54.0f * ImGuiHelpers.GlobalScale);
                if (ImGui.BeginChild("Selectables", selectablesSize, false)) {
                    DrawSelectables();
                }
                ImGui.EndChild();

                if (ImGui.BeginChild("FloorSelection", ImGui.GetContentRegionAvail(), false)) {
                    DrawFloorSelection();
                }
                ImGui.EndChild();
            }
            ImGui.EndChild();
            
            ImGui.TableNextColumn();
            if (ImGui.BeginChild("ContentsChild", ImGui.GetContentRegionAvail(), false)) {
                DrawContents();
            }
            ImGui.EndChild();
            
            ImGui.EndTable();
        }
    }
    
    private void DrawSelectables() {
        ImGui.PushStyleColor(ImGuiCol.FrameBg, ImGui.GetStyle().Colors[(int) ImGuiCol.FrameBg] with { W = 0.05f });
        ImGui.PushStyleVar(ImGuiStyleVar.ScrollbarSize, 0.0f);
        if (ImGui.BeginListBox("##EnemySelectables", ImGui.GetContentRegionAvail())) {

            if (Plugin.StorageManager.Enemies.TryGetValue(dungeonType, out var floorEnemies)) {
                if (floorEnemies.TryGetValue(floorSet * 10 + 1, out var enemies)) {
                    foreach (var enemy in enemies.OrderBy(Plugin.GetEnemyName)) {
                        if (ImGui.Selectable(Plugin.GetEnemyName(enemy), selectedEnemy == enemy)) {
                            selectedEnemy = enemy;
                        }
                    }
                }
            }

            if (Plugin.StorageManager.Floorsets.TryGetValue(dungeonType, out var floorSets)) {
                if (floorSets.TryGetValue(floorSet * 10 + 1, out var floorInfo)) {
                    ImGui.PushStyleColor(ImGuiCol.Text, KnownColor.Orange.Vector());
                    if (ImGui.Selectable(Plugin.GetEnemyName(floorInfo), selectedEnemy == floorInfo)) {
                        selectedEnemy = floorInfo;
                    }
                    ImGui.PopStyleColor();
                }
            }
            
            ImGui.EndListBox();
        }
        ImGui.PopStyleVar();
        ImGui.PopStyleColor();
    }
    
    private void DrawFloorSelection() {
        ImGui.PushItemWidth(ImGui.GetContentRegionAvail().X);
        if (ImGui.BeginCombo("##DungeonSelection", dungeonType.Localized())) {
            foreach (var type in Enum.GetValues<DeepDungeonType>().Skip(1)) { // Skip "Unknown" entry
                if (ImGui.Selectable(type.Localized(), dungeonType == type)) {
                    if (dungeonType != type) {
                        selectedEnemy = null;
                    }
                    
                    dungeonType = type;
                }
            }
            
            ImGui.EndCombo();
        }
        
        ImGui.PushItemWidth(ImGui.GetContentRegionAvail().X);
        if (ImGui.BeginCombo("##FloorSelection", GetFloorSetString(floorSet))) {

            foreach (uint index in Enumerable.Range(0, 10)) {
                if (ImGui.Selectable(GetFloorSetString(index), floorSet == index)) {
                    if (floorSet != index) {
                        selectedEnemy = null;
                    }
                    
                    floorSet = index;
                }
            }

            if (dungeonType is DeepDungeonType.PalaceOfTheDead) {
                foreach (uint index in Enumerable.Range(10, 10)) {
                    if (ImGui.Selectable(GetFloorSetString(index), floorSet == index)) {
                        if (floorSet != index) {
                            selectedEnemy = null;
                        }
                        
                        floorSet = index;
                    }
                }
            }
            
            ImGui.EndCombo();
        }
    }
    
    private void DrawContents() {
        if (selectedEnemy is not null) {
            selectedEnemy.Draw(WindowExtraButton.PopOut);
        } else {
            var nothingSelectedText = "Select an enemy";
            var textSize = ImGui.CalcTextSize(nothingSelectedText);
            ImGui.SetCursorPos(ImGui.GetContentRegionAvail() / 2.0f - textSize / 2.0f);
            ImGui.TextColored(KnownColor.Gray.Vector(), nothingSelectedText);
        }
    }

    private static string GetFloorSetString(uint index) 
        => $"{index * 10 + 1} - {(index + 1) * 10}";
}
