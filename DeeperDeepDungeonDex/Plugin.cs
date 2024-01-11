﻿using System.Globalization;
using System.Threading.Tasks;
using Dalamud.Game.Command;
using Dalamud.Plugin;
using Dalamud.Interface.Windowing;
using DeeperDeepDungeonDex.Storage;
using DeeperDeepDungeonDex.Windows;
using FFXIVClientStructs.FFXIV.Client.Game.Event;
using FFXIVClientStructs.FFXIV.Client.Game.InstanceContent;

namespace DeeperDeepDungeonDex;

public sealed class Plugin : IDalamudPlugin {
    private const string CommandName = "/dddd";

    public static Configuration Configuration = null!;
    public static StorageManager StorageManager = null!;
    public static WindowSystem WindowSystem = null!;
    public static MobWindow MobWindow = null!;

    public Plugin(DalamudPluginInterface pluginInterface) {
        Strings.Culture = new CultureInfo(pluginInterface.UiLanguage);

        pluginInterface.Create<Services>();
        Configuration = Services.PluginInterface.GetPluginConfig() as Configuration ?? new Configuration();

        StorageManager = new StorageManager();
        Task.Run(StorageManager.Load);

        WindowSystem = new WindowSystem("DeeperDeepDungeonDex");
        WindowSystem.AddWindow(MobWindow = new MobWindow());

        Services.CommandManager.AddHandler(CommandName, new CommandInfo(OnCommand));

        Services.PluginInterface.UiBuilder.Draw += this.Draw;
        Services.PluginInterface.UiBuilder.OpenConfigUi += this.OpenConfigUi;
    }

    public void Dispose() {
        Services.PluginInterface.UiBuilder.Draw -= this.Draw;
        Services.PluginInterface.UiBuilder.OpenConfigUi -= this.OpenConfigUi;

        Services.CommandManager.RemoveHandler(CommandName);

        WindowSystem.RemoveAllWindows();
        MobWindow.Dispose();
    }

    public static unsafe InstanceContentDeepDungeon* GetDirector() {
        var eventFramework = EventFramework.Instance();
        return eventFramework == null ? null : eventFramework->GetInstanceContentDeepDungeon();
    }

    public static unsafe bool InDeepDungeon() => GetDirector() != null;

    private void OnCommand(string command, string args) {
        this.OpenConfigUi();
    }

    private void Draw() {
        WindowSystem.Draw();
    }

    public void OpenConfigUi() {
        // TODO
    }
}
