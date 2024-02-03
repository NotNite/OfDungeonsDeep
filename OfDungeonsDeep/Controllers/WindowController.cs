using System;
using System.Collections.Generic;
using System.Linq;
using Dalamud.Game.Command;
using Dalamud.Interface.Windowing;
using Dalamud.Utility;
using OfDungeonsDeep.Components;

namespace OfDungeonsDeep.Controllers;

public class WindowController : IDisposable {
    private const string CommandName = "/dddd";
    
    private readonly WindowSystem windowSystem;
    private readonly List<DeepDungeonWindow> windows;
    public readonly TargetDataWindow TargetDataWindow;
    private readonly DexWindow dexWindow;
    private readonly ConfigurationWindow configWindow;
    private readonly FloorDataWindow floorDataWindow;

    public WindowController() {
        windows = new List<DeepDungeonWindow>();
        windowSystem = new WindowSystem("OfDungeonsDeep");
        
        windowSystem.AddWindow(configWindow = new ConfigurationWindow());
        windowSystem.AddWindow(TargetDataWindow = new TargetDataWindow());
        windowSystem.AddWindow(floorDataWindow = new FloorDataWindow());
        windowSystem.AddWindow(dexWindow = new DexWindow());
        
        Services.CommandManager.AddHandler(CommandName, new CommandInfo(OnCommand) {
            HelpMessage = "Open Configuration Window\n/dddd dex \u2192 Open Monster Dex\n/dddd floor \u2192 Show Floor Info"
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
        switch (args = args?.ToLowerInvariant() ?? string.Empty) {
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
                        ShowFloorSetData(floorSetData);
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

    public void TryAddDataWindow(IDrawableMob target) {
        if (windows.FirstOrDefault(window => string.Equals(window.WindowName,  GetMobWindowName(target), StringComparison.InvariantCultureIgnoreCase)) is null) {
            var newWindow = new MobDataWindow(GetMobWindowName(target), target);

            windows.Add(newWindow);
            windowSystem.AddWindow(newWindow);

            newWindow.UnCollapseOrShow();
        }
    }

    public void ShowFloorSetData(IDrawableFloorSet floorSet) {
        floorDataWindow.SetFloor(floorSet);
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

    public void RemoveWindowForFloor() {
        floorDataWindow.SetFloor(null);
    }
    
    private static string GetMobWindowName(IDrawableMob enemy) {
        return $"Enemy Info - {enemy.Name}##{enemy.Id}";
    }
}
