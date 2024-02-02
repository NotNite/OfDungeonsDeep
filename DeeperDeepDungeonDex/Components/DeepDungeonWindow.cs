using Dalamud.Interface.Windowing;

namespace DeeperDeepDungeonDex.Components;

/// <summary>
/// This class is an extension of a Dalamud Window, to allow for easier collapse control.
/// </summary>
public abstract class DeepDungeonWindow : Window {
    private bool isCollapsed;

    protected DeepDungeonWindow(string name) : base(name) {
    }

    public void UnCollapseOrShow() {
        TryUnCollapse();
        IsOpen = true;
    }

    public void UnCollapseOrToggle() {
        TryUnCollapse();
        Toggle();
    }

    private void TryUnCollapse() {
        if (isCollapsed) {
            UnCollapse();
        }
    }
    
    private void UnCollapse() {
        isCollapsed = false;
        Collapsed = false;
    }

    public override void Update() {
        isCollapsed = true;
    }
    
    public override void Draw() {
        isCollapsed = false;
        Collapsed = null;
    }
}
