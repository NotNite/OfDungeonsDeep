using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;

namespace DeeperDeepDungeonDex.Storage;

public class StorageManager {
    public Dictionary<DeepDungeonType, Dictionary<uint, List<Enemy>>> Enemies = new();
    public Dictionary<DeepDungeonType, Dictionary<uint, Floorset>> Floorsets = new();

    // Temporary hack until we implement floor detection
    public Dictionary<uint, Enemy> AllEnemies =>
        this.Enemies.SelectMany(x => x.Value)
            .SelectMany(x => x.Value)
            .GroupBy(x => x.Id)
            .ToDictionary(x => x.Key, x => x.First());

    // TODO: handle localization
    public void Load() {
        var assemblyDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!;
        var data = Path.Combine(assemblyDir, "Data");

        var options = new JsonSerializerOptions {IncludeFields = true};
        this.Enemies = JsonSerializer.Deserialize<Dictionary<DeepDungeonType, Dictionary<uint, List<Enemy>>>>(
            File.ReadAllText(Path.Combine(data, "enemies.en.json")),
            options
        )!;
        Services.PluginLog.Debug("Loaded {Count} enemies", this.AllEnemies.Count);

        this.Floorsets = JsonSerializer.Deserialize<Dictionary<DeepDungeonType, Dictionary<uint, Floorset>>>(
            File.ReadAllText(Path.Combine(data, "floorsets.en.json")),
            options
        )!;
        Services.PluginLog.Debug("Loaded {Count} floorsets", this.Floorsets.Sum(x => x.Value.Count));
    }
}
