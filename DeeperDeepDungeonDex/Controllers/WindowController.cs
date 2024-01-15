using System;
using System.Collections.Generic;
using System.Linq;
using Dalamud.Game.Command;
using Dalamud.Interface.Windowing;
using Dalamud.Utility;
using DeeperDeepDungeonDex.Storage;

namespace DeeperDeepDungeonDex.System;

public class WindowController : IDisposable {
    private const string CommandName = "/dddd";
    
    private readonly WindowSystem windowSystem;
    private readonly List<DeepDungeonWindow> windows;
    public readonly TargetDataWindow TargetDataWindow;
    private readonly DexWindow dexWindow;

    public WindowController() {
        windows = new List<DeepDungeonWindow>();
        windowSystem = new WindowSystem("DeeperDeepDungeonDex");
        
        windowSystem.AddWindow(TargetDataWindow = new TargetDataWindow());
        windowSystem.AddWindow(dexWindow = new DexWindow());
        
        Services.CommandManager.AddHandler(CommandName, new CommandInfo(OnCommand));
        
        Services.PluginInterface.UiBuilder.Draw += this.Draw;
        Services.PluginInterface.UiBuilder.OpenConfigUi += this.OpenConfigUi;
    }

    public void Dispose() {
        Services.CommandManager.RemoveHandler(CommandName);

        Services.PluginInterface.UiBuilder.Draw -= this.Draw;
        Services.PluginInterface.UiBuilder.OpenConfigUi -= this.OpenConfigUi;
        
        windowSystem.RemoveAllWindows();
    }
    
    private void OnCommand(string command, string args) {
        if (args.IsNullOrEmpty()) {
            this.OpenConfigUi();
        } else if (args.Contains("dex")) {
            dexWindow.UnCollapseOrToggle();
        }
    }
    
    private void Draw() {
        windowSystem.Draw();
    }
    
    private void OpenConfigUi() {
        // TODO: Make Config UI
    }

    public void TryAddMobDataWindow(IDrawableMob enemy) {
        var targetWindowName = GetMobWindowName(enemy);
        
        if (!windows.Any(window => string.Equals(window.WindowName, targetWindowName, StringComparison.InvariantCultureIgnoreCase))) {
            var newMobWindow = new MobDataWindow(targetWindowName, enemy);
            windows.Add(newMobWindow);
            windowSystem.AddWindow(newMobWindow);

            newMobWindow.IsOpen = true;
        }
    }

    public void RemoveWindow(DeepDungeonWindow window) {
        if (windows.Contains(window)) {
            windowSystem.RemoveWindow(window);
            windows.Remove(window);
        }
    }

    public void RemoveWindowForEnemy(IDrawableMob enemy) {
        if (windows.FirstOrDefault(window => window.WindowName == GetMobWindowName(enemy)) is {} mobWindow) {
            RemoveWindow(mobWindow);
        }
    }
    
    private static string GetMobWindowName(IDrawableMob enemy) {
        return $"Enemy Info - {enemy.Name}##{enemy.Id}";
    }
}
