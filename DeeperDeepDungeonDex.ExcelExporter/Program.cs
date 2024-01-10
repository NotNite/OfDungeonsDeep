using System.Text.Json;
using Lumina;
using Lumina.Excel.GeneratedSheets2;

var lumina = new GameData(args[0]);
var bnpcName = lumina.Excel.GetSheet<BNpcName>()!;
var names = new Dictionary<uint, string>();
foreach (var row in bnpcName) names[row.RowId] = row.Singular;
Directory.CreateDirectory("./processor");
File.WriteAllText("./processor/names.json", JsonSerializer.Serialize(names));
