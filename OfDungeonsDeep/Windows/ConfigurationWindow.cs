using System.Numerics;
using Dalamud.Interface.Windowing;
using ImGuiNET;
using OfDungeonsDeep.Components;

namespace OfDungeonsDeep.Controllers;

public class ConfigurationWindow : DeepDungeonWindow {

    public ConfigurationWindow() : base("OfDungeonsDeep - Configuration") {
        SizeConstraints = new WindowSizeConstraints {
            MinimumSize = new Vector2(300.0f, 150.0f),
            MaximumSize = new Vector2(float.PositiveInfinity),
        };
    }

    public override void Draw() {
        base.Draw();
        var changed = ImGui.Checkbox("Enable Target Window", ref Plugin.Configuration.EnableTargetWindow);

        changed |= ImGui.Checkbox("Enable Floor Window", ref Plugin.Configuration.EnableFloorWindow);
        changed |= ImGui.Checkbox("Show Floor Window on Every Floor", ref Plugin.Configuration.ShowFloorEveryFloor);

        if (changed) {
            Plugin.Configuration.Save();
        }
    }
}
