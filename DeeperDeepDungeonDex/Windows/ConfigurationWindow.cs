using System.Numerics;
using ImGuiNET;

namespace DeeperDeepDungeonDex.System;

public class ConfigurationWindow : DeepDungeonWindow {

    public ConfigurationWindow() : base("DeeperDeepDungeonDex - Configuration") {
        SizeConstraints = new WindowSizeConstraints {
            MinimumSize = new Vector2(300.0f, 100.0f),
            MaximumSize = new Vector2(float.PositiveInfinity),
        };
    }

    public override void Draw() {
        base.Draw();
        var changed = ImGui.Checkbox("Enable Target Window", ref Plugin.Configuration.EnableTargetWindow);

        changed |= ImGui.Checkbox("Enable Floor Window", ref Plugin.Configuration.EnableFloorWindow);

        if (changed) {
            Plugin.Configuration.Save();
        }
    }
}
