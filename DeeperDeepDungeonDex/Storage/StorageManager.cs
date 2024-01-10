using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;

namespace DeeperDeepDungeonDex.Storage;

public class StorageManager {
    public Dictionary<uint, Enemy> Enemies = new();
    public Dictionary<DeepDungeonType, Dictionary<uint, Floorset>> Floorsets = new();

    public void Load() {
        var assemblyDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!;
        var data = Path.Combine(assemblyDir, "Data");

        var options = new JsonSerializerOptions {IncludeFields = true};
        this.Enemies = JsonSerializer.Deserialize<Dictionary<uint, Enemy>>(
            File.ReadAllText(Path.Combine(data, "enemies.json")),
            options
        )!;
        Services.PluginLog.Debug("Loaded {Count} enemies", this.Enemies.Count);
        
        this.Floorsets = JsonSerializer.Deserialize<Dictionary<DeepDungeonType, Dictionary<uint, Floorset>>>(
            File.ReadAllText(Path.Combine(data, "floorsets.json")),
            options
        )!;
        Services.PluginLog.Debug("Loaded {Count} floorsets", this.Floorsets.Sum(x => x.Value.Count));
    }
}
