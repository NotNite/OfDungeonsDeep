using Dalamud.Configuration;
using System;

namespace DeeperDeepDungeonDex;

[Serializable]
public class Configuration : IPluginConfiguration {
    public int Version { get; set; } = 0;

    public bool EnableTargetWindow = true;
    public bool EnableFloorWindow = true;
    public bool ShowFloorEveryFloor = false;

    public void Save() => Services.PluginInterface.SavePluginConfig(this);
}
