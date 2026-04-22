using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Dalamud.Interface.Utility;
using Dalamud.Bindings.ImGui;
using Lumina.Excel.Sheets;
using OfDungeonsDeep.Components;
using Lumina.Data;

namespace OfDungeonsDeep.Storage;

public class Ability {
    public uint? Id;
    public AttackType? Type;
    public string? Potency;

    public static void DrawAbilityList(List<Ability?>? abilities, IDrawableMob enemy) {
        if (abilities is null) return;
        
        if (!abilities.Any()) {
            ImGui.TextUnformatted("No Special Abilities");
            return;
        }
            
        if (ImGui.BeginTable("###AbilityInfoTable", 3, ImGuiTableFlags.Resizable | ImGuiTableFlags.RowBg | ImGuiTableFlags.SizingStretchProp)) {
            ImGui.TableSetupColumn("##AbilityName", ImGuiTableColumnFlags.WidthFixed, 100.0f * ImGuiHelpers.GlobalScale);
            ImGui.TableSetupColumn("##Potency", ImGuiTableColumnFlags.WidthFixed, 75.0f * ImGuiHelpers.GlobalScale);
            ImGui.TableSetupColumn("##Description", ImGuiTableColumnFlags.WidthStretch);

            foreach (var ability in abilities) {
                ability?.Draw(enemy.DungeonType, enemy.StartFloor, enemy.Id);
            }
            
            ImGui.EndTable();
        }
    }

    private void Draw(DeepDungeonType type, int floor, uint id) {
        if (Id is null || Services.DataManager.GetExcelSheet<Action>()?.GetRow(Id.Value) is not { } ability) return;
        if (Services.DataManager.GameData.Excel.GetSheet<Action>(Language.English)?.GetRow(Id.Value) is not { } abilityEn) return;
        if (Strings.ResourceManager.GetString($"AbilityNote_{type.ToString()}_{Plugin.GetFloorSetId(floor)}_{id}_{Id}") is not { } description) return;

        var titleCaseName = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(ability.Name.ExtractText());
        var titleCaseNameEn = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(abilityEn.Name.ExtractText());
        
        ImGui.TableNextRow();

        ImGui.TableNextColumn();
        ImGui.TextUnformatted(titleCaseName);
        if (ImGui.IsItemHovered()) {
            ImGui.SetTooltip(titleCaseNameEn);
        }

        ImGui.TableNextColumn();
        DrawAbilityDamage();
        if (ImGui.IsItemHovered() && Potency is not null) {
            ImGui.BeginTooltip();
            DrawAbilityDamage();
            ImGui.EndTooltip();
        }
        
        ImGui.TableNextColumn();
        ImGui.PushTextWrapPos(0.0f);
        ImGui.TextUnformatted(description);
        ImGui.PopTextWrapPos();

        if (ImGui.IsItemHovered()) {
            ImGui.SetTooltip(description.Replace("; ", "\n").Replace("%", "%%"));
        }
    }

    private void DrawAbilityDamage() {
        if (Potency is not null && Type is not null && Potency is not "n/a") {
            Type?.DrawIcon();
            ImGui.SameLine();
            ImGui.TextUnformatted(Potency ?? "");
        }
    }
}