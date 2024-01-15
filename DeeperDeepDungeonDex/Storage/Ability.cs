using System.Collections.Generic;
using System.Linq;
using Dalamud.Interface.Utility;
using DeeperDeepDungeonDex.System;
using ImGuiNET;
using Lumina.Excel.GeneratedSheets2;

namespace DeeperDeepDungeonDex.Storage;

public class Ability {
    public required uint Id;
    public AttackType Type;
    public uint? Potency;

    public static void DrawAbilityList(List<Ability>? abilities, IDrawableMob enemy) {
        if (abilities is null) return;
        
        if (!abilities.Any()) {
            ImGui.TextUnformatted("No Special Abilities");
        }
            
        if (ImGui.BeginTable("###AbilityInfoTable", 4, ImGuiTableFlags.Resizable | ImGuiTableFlags.RowBg)) {
            ImGui.TableSetupColumn("##AbilityName", ImGuiTableColumnFlags.WidthFixed, 100.0f * ImGuiHelpers.GlobalScale);
            ImGui.TableSetupColumn("##Icon", ImGuiTableColumnFlags.WidthFixed, 18.0f * ImGuiHelpers.GlobalScale);
            ImGui.TableSetupColumn("##Potency", ImGuiTableColumnFlags.WidthFixed, 50.0f * ImGuiHelpers.GlobalScale);
            ImGui.TableSetupColumn("##Description", ImGuiTableColumnFlags.WidthStretch);

            foreach (var ability in abilities) {
                ability.Draw(enemy.DungeonType, enemy.StartFloor, enemy.Id);
            }
            
            ImGui.EndTable();
        }
    }

    private void Draw(DeepDungeonType type, int floor, uint id) {
        if (Services.DataManager.GetExcelSheet<Action>()?.GetRow(Id) is not { } ability) return;
        if (Strings.ResourceManager.GetString($"AbilityNote_{type.ToString()}_{((floor / 10) * 10) + 1}_{id}_{Id}") is not { } description) return;

        ImGui.TableNextColumn();
        ImGui.TextUnformatted(ability.Name);
        if (ImGui.IsItemHovered()) {
            ImGui.SetTooltip(ability.Name);
        }

        ImGui.TableNextColumn();
        if (Potency is not null) {
            Type.DrawIcon();
        }
            
        ImGui.TableNextColumn();
        ImGui.TextUnformatted(Potency is not null ? Potency.ToString() : "N/A");

        ImGui.TableNextColumn();
        ImGui.TextUnformatted(description);
        if (ImGui.IsItemHovered()) {
            ImGui.SetTooltip(description.Replace("; ", "\n").Replace("%", "%%"));
        }
    }
}
