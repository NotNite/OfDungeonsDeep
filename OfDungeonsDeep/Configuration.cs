using Dalamud.Configuration;
using System;
using System.Collections.Generic;

namespace OfDungeonsDeep;

[Serializable]
public class Configuration : IPluginConfiguration {
    public int Version { get; set; } = 0;

    public bool EnableTargetWindow = true;
    public bool EnableDeadTargetWindow = true;
    public bool EnableFloorWindow = true;
    public bool ShowFloorEveryFloor = false;
    public HashSet<uint> LockedMobWindows = new();
    public bool LockFloorWindow = false;
    public bool LockTargetWindow = false;

    public void Save() => Services.PluginInterface.SavePluginConfig(this);
}
