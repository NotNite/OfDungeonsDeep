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
    private readonly ConfigurationWindow configWindow;

    public WindowController() {
        windows = new List<DeepDungeonWindow>();
        windowSystem = new WindowSystem("DeeperDeepDungeonDex");
        
        windowSystem.AddWindow(configWindow = new ConfigurationWindow());
        windowSystem.AddWindow(TargetDataWindow = new TargetDataWindow());
        windowSystem.AddWindow(dexWindow = new DexWindow());
        
        Services.CommandManager.AddHandler(CommandName, new CommandInfo(OnCommand) {
            HelpMessage = "Open Configuration Window\n/dddd dex \u2192 Open Monster Dex\n/dddd floor \\u2192 Show Floor Info"
        });
        
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
        switch (args) {
            case null:
            case not null when args.IsNullOrEmpty():
                this.OpenConfigUi();
                break;
            
            case not null when args.Contains("dex"):
                dexWindow.UnCollapseOrToggle();
                break;
            
            case not null when args.Contains("floor") && windows.FirstOrDefault(window => window is FloorDataWindow) is {} floorWindow:
                floorWindow.UnCollapseOrShow();
                break;
            
            case not null when args.Contains("floor") && windows.FirstOrDefault(window => window is FloorDataWindow) is null && Plugin.GetDeepDungeonType() is {} dungeonType && Plugin.GetFloorSetId() is {} currentFloorSet:
                if (Plugin.StorageManager.Floorsets.TryGetValue(dungeonType, out var floorSets)) {
                    if (floorSets.TryGetValue(currentFloorSet, out var floorSetData)) {
                        TryAddDataWindow(floorSetData);
                    }
                }
                break;
        }
    }
    
    private void Draw() {
        windowSystem.Draw();
    }
    
    private void OpenConfigUi() {
        configWindow.UnCollapseOrToggle();
    }

    public void TryAddDataWindow(object target) {
        var windowName = target switch {
            Enemy enemy => GetMobWindowName(enemy),
            FloorSet floor => GetFloorWindowName(floor),
            _ => throw new ArgumentOutOfRangeException(nameof(target), target, null)
        };

        if (!windows.Any(window => string.Equals(window.WindowName, windowName, StringComparison.InvariantCultureIgnoreCase))) {
            DeepDungeonWindow newWindow = target switch {
                Enemy enemy => new MobDataWindow(windowName, enemy),
                FloorSet floor => new FloorDataWindow(windowName, floor),
                _ => throw new ArgumentOutOfRangeException(nameof(target), target, null)
            };
            
            windows.Add(newWindow);
            windowSystem.AddWindow(newWindow);

            newWindow.IsOpen = true;
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

    public void RemoveWindowForFloor(IDrawableFloorSet floorSet) {
        if (windows.FirstOrDefault(window => window.WindowName == GetFloorWindowName(floorSet)) is {} mobWindow) {
            RemoveWindow(mobWindow);
        }
    }
    
    private static string GetMobWindowName(IDrawableMob enemy) {
        return $"Enemy Info - {enemy.Name}##{enemy.Id}";
    }

    private static string GetFloorWindowName(IDrawableFloorSet floor) {
        return $"Floor Info - {floor.Title}##{floor.DungeonType}";
    }
}
