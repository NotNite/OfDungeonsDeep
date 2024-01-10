using System.Text;
using System.Text.Json;
using DeeperDeepDungeonDex.Common;
using Lumina;
using Lumina.Excel.GeneratedSheets2;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

var lumina = new GameData(args[0]);
var bnpcName = lumina.Excel.GetSheet<BNpcName>()!;
var compendium = "./compendium";

var enemies = new Dictionary<uint, Enemy>();
var floorsets = new Dictionary<DeepDungeonType, Dictionary<uint, Floorset>>();

var deserializer = new DeserializerBuilder()
    .WithNamingConvention(UnderscoredNamingConvention.Instance)
    .IgnoreUnmatchedProperties()
    .Build();

uint? ResolveId(string file, string name) {
    try {
        var id = bnpcName
            .Single(x => string.Equals(x.Singular.RawString, name, StringComparison.CurrentCultureIgnoreCase))
            .RowId;
        return id;
    } catch (Exception e) {
        Console.WriteLine($"Failed to find BNpcName for {name} in {file}");
        //Console.WriteLine(e);
        return null;
    }
}

var enemiesDirs = Directory.GetFileSystemEntries(
    compendium,
    "*_enemies");
foreach (var dir in enemiesDirs) {
    var files = Directory.GetFiles(dir, "*.md");
    foreach (var file in files) {
        //Console.WriteLine(file);
        try {
            var frontmatter = File.ReadAllText(file).Split("---")[1];
            var enemy = deserializer.Deserialize<Enemy>(frontmatter);
            var id = ResolveId(file, enemy.Name);
            if (id is not null) enemies[id.Value] = enemy;
        } catch (Exception e) {
            Console.WriteLine($"Failed to parse {file}");
            Console.WriteLine(e);
        }
    }
}

var floorsetDirs = Directory.GetFileSystemEntries(
    compendium,
    "*_floorset");
foreach (var dir in floorsetDirs) { }

if (!Directory.Exists("./Data")) Directory.CreateDirectory("./Data");
File.WriteAllText(
    Path.Combine("./Data", "enemies.json"),
    JsonSerializer.Serialize(
        enemies,
        new JsonSerializerOptions {IncludeFields = true}
    )
);
