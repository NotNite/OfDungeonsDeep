using System.Text.Json.Serialization;
using Dalamud.Interface.Internal;
using Dalamud.Interface.Utility;
using ImGuiNET;

namespace OfDungeonsDeep.Storage;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum AttackType {
    Physical,
    Magic,
    Unique
}

public static class AttackTypeExtensions {
    public static IDalamudTextureWrap? Texture(this AttackType type) => type switch {
        AttackType.Magic => Services.TextureProvider.GetIcon(60012),
        AttackType.Physical => Services.TextureProvider.GetIcon(60011),
        AttackType.Unique => Services.TextureProvider.GetIcon(60013),
        _ => null,
    };

    public static void DrawIcon(this AttackType type) {
        if (type.Texture() is { } texture) {
            ImGui.Image(texture.ImGuiHandle, ImGuiHelpers.ScaledVector2(18.0f, 18.0f));
        }
    }
    
    public static void DrawAttack(this AttackType type, uint? attackDamage = null, string? attackName = null) {
        if (attackDamage is null) {
            ImGui.TextUnformatted("N/A");
        } else {
            var autoDamageType = type.Texture(); 
            ImGui.BeginGroup();
            if (autoDamageType is not null) {
                ImGui.Image(autoDamageType.ImGuiHandle, ImGuiHelpers.ScaledVector2(18.0f, 18.0f));
            }
            if (autoDamageType is not null) {
                ImGui.SameLine();
            }
            ImGui.TextUnformatted(attackDamage.ToString());
            ImGui.EndGroup();
            if (ImGui.IsItemHovered() && attackName is not null) {
                ImGui.SetTooltip(attackName);
            }
        }
    }
}
