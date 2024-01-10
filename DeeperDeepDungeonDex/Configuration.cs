﻿using Dalamud.Configuration;
using System;

namespace DeeperDeepDungeonDex;

[Serializable]
public class Configuration : IPluginConfiguration {
    public int Version { get; set; } = 0;

    public void Save() {
        Services.PluginInterface.SavePluginConfig(this);
    }
}