using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text.Json;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace DeeperDeepDungeonDex.Storage;

public class StorageManager {
    public Dictionary<uint, Mob> Mobs = new();

    public void Load() {
        var assemblyDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!;
        var data = Path.Combine(assemblyDir, "Data");
        var index = JsonSerializer.Deserialize<StorageIndex>(
            File.ReadAllText(Path.Combine(data, "index.json")),
            new JsonSerializerOptions {IncludeFields = true}
        )!;

        var deserializer = new DeserializerBuilder()
            .WithNamingConvention(PascalCaseNamingConvention.Instance)
            .Build();

        foreach (var mobDataPath in index.MobData) {
            var mobDataStr = File.ReadAllText(Path.Combine(data, mobDataPath));
            var mobData = deserializer.Deserialize<Dictionary<string, Mob>>(mobDataStr);
            foreach (var (key, value) in mobData) {
                var id = uint.Parse(key.Split('-')[0]);
                // TryAdd because the dataset is fucking wrong lmao
                this.Mobs.TryAdd(id, value);
            }
        }
        
        Services.PluginLog.Debug("Loaded {Count} mobs", this.Mobs.Count);
    }
}
