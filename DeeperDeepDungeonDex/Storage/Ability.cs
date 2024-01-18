using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Dalamud.Interface.Utility;
using DeeperDeepDungeonDex.System;
using ImGuiNET;
using Lumina.Excel.GeneratedSheets2;

namespace DeeperDeepDungeonDex.Storage;

public class Ability {
    public uint? Id;
    public AttackType? Type;
    public string? Potency;

    public static void DrawAbilityList(List<Ability?>? abilities, IDrawableMob enemy) {
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
                ability?.Draw(enemy.DungeonType, enemy.StartFloor, enemy.Id);
            }
            
            ImGui.EndTable();
        }
    }

    private void Draw(DeepDungeonType type, int floor, uint id) {
        if (Id is null || Services.DataManager.GetExcelSheet<Action>()?.GetRow(Id.Value) is not { } ability) return;
        if (Strings.ResourceManager.GetString($"AbilityNote_{type.ToString()}_{((floor / 10) * 10) + 1}_{id}_{Id}") is not { } description) return;

        var titleCaseName = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(ability.Name);
        
        ImGui.TableNextColumn();
        ImGui.TextUnformatted(titleCaseName);
        if (ImGui.IsItemHovered()) {
            ImGui.SetTooltip(titleCaseName);
        }

        ImGui.TableNextColumn();
        if (Potency is not null) {
            Type?.DrawIcon();
        }
            
        ImGui.TableNextColumn();
        ImGui.TextUnformatted(Potency ?? "n/a");
        if (ImGui.IsItemHovered() && Potency is not null) {
            ImGui.SetTooltip(Potency.Replace("%", "%%"));
        }
        
        ImGui.TableNextColumn();
        ImGui.TextUnformatted(description);
        if (ImGui.IsItemHovered()) {
            ImGui.SetTooltip(description.Replace("; ", "\n").Replace("%", "%%"));
        }
    }
}
