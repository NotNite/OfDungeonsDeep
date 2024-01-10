using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text.Json;
using DeeperDeepDungeonDex.Common;

namespace DeeperDeepDungeonDex.Storage;

public class StorageManager {
    public Dictionary<uint, Enemy> Enemies = new();

    public void Load() {
        var assemblyDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!;
        var data = Path.Combine(assemblyDir, "Data");

        this.Enemies = JsonSerializer.Deserialize<Dictionary<uint, Enemy>>(
            File.ReadAllText(Path.Combine(data, "enemies.json")),
            new JsonSerializerOptions {IncludeFields = true}
        )!;
        Services.PluginLog.Debug("Loaded {Count} enemies", this.Enemies.Count);
    }
}
