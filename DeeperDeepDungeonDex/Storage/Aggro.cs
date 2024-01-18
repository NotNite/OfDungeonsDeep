using System;
using System.Text.Json.Serialization;
using Dalamud.Interface;
using ImGuiNET;

namespace DeeperDeepDungeonDex.Storage;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum Aggro {
    Sight,
    Sound,
    Proximity
}

public static class AggroExtensions {
    public static string IconString(this Aggro aggro) => aggro switch {
        Aggro.Sight => FontAwesomeIcon.Eye.ToIconString(),
        Aggro.Sound => FontAwesomeIcon.AssistiveListeningSystems.ToIconString(),
        Aggro.Proximity => FontAwesomeIcon.Bullseye.ToIconString(),
        _ => throw new ArgumentOutOfRangeException(nameof(aggro))
    };

    public static void Draw(this Aggro aggro) {
        ImGui.PushFont(UiBuilder.IconFont);
        ImGui.TextUnformatted(aggro.IconString());
        ImGui.PopFont();
        ImGui.SameLine();
        ImGui.TextUnformatted(aggro.ToString());
    }
}
