using Dalamud.Interface.Windowing;
using FFXIVClientStructs.FFXIV.Common.Math;
using ImGuiNET;
using OfDungeonsDeep.Components;

namespace OfDungeonsDeep.Controllers;

public class FloorDataWindow : DeepDungeonWindow {
    private IDrawableFloorSet? floorSet;

    public FloorDataWindow() : base("OfDungeonsDeep - Floor Info") {
        SizeConstraints = new WindowSizeConstraints {
            MinimumSize = new Vector2(350.0f, 250.0f),
            MaximumSize = new Vector2(float.PositiveInfinity),
        };

        Flags |= ImGuiWindowFlags.NoTitleBar;
    }

    public void SetFloor(IDrawableFloorSet? drawableFloorSet) {
        floorSet = drawableFloorSet;

        if (floorSet is not null) {
            UnCollapseOrShow();
        }
    }

    public override bool DrawConditions() {
        if (floorSet is null) return false;
        if (!Plugin.InDeepDungeon()) return false;
        if (!Plugin.Configuration.EnableFloorWindow) return false;

        return true;
    }

    public override void PreDraw() {
        if (Plugin.Configuration.LockFloorWindow) {
            Flags |= ImGuiWindowFlags.NoMove;
            Flags |= ImGuiWindowFlags.NoResize;
        } else {
            Flags &= ~ImGuiWindowFlags.NoMove;
            Flags &= ~ImGuiWindowFlags.NoResize;
        }
    }
    
    public override void Draw() {
        base.Draw();
        
        floorSet?.Draw();
    }
    
    public override void OnClose() {
        Plugin.Controller.WindowController.RemoveWindow(this);
    }
}
