using Dalamud.Interface.Windowing;

namespace DeeperDeepDungeonDex.System;

/// <summary>
/// This class is an extension of a Dalamud Window, to allow for easier collapse control.
/// </summary>
public abstract class DeepDungeonWindow(string name) : Window(name) {
    private bool isCollapsed;

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
}
