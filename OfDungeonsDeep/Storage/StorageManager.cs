using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;

namespace OfDungeonsDeep.Storage;

public class StorageManager {
    public Dictionary<DeepDungeonType, Dictionary<uint, List<Enemy>>> Enemies = new();
    public Dictionary<DeepDungeonType, Dictionary<uint, FloorSet>> Floorsets = new();
    public bool DataReady;

    public void Load() {
        var assemblyDir = Services.PluginInterface.AssemblyLocation.DirectoryName!;
        var data = Path.Combine(assemblyDir, "Data");

        var options = new JsonSerializerOptions {IncludeFields = true};
        this.Enemies = JsonSerializer.Deserialize<Dictionary<DeepDungeonType, Dictionary<uint, List<Enemy>>>>(
            File.ReadAllText(Path.Combine(data, "enemies.json")),
            options
        )!;

        this.Floorsets = JsonSerializer.Deserialize<Dictionary<DeepDungeonType, Dictionary<uint, FloorSet>>>(
            File.ReadAllText(Path.Combine(data, "floorsets.json")),
            options
        )!;
        Services.PluginLog.Debug("Loaded {Count} floorsets", this.Floorsets.Sum(x => x.Value.Count));

        DataReady = true;
    }
}
