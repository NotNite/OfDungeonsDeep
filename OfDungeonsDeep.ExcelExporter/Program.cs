using System.Text.Json;
using Lumina;
using Lumina.Excel.Sheets;
using Action = Lumina.Excel.Sheets.Action;

var lumina = new GameData(args[0]);
var bnpcName = lumina.Excel.GetSheet<BNpcName>()!;
var action = lumina.Excel.GetSheet<Action>()!;

var names = new Dictionary<uint, string>();
var actions = new Dictionary<uint, string>();
foreach (var row in bnpcName) names[row.RowId] = row.Singular.ExtractText();
foreach (var row in action) actions[row.RowId] = row.Name.ExtractText();


Directory.CreateDirectory("./processor");
File.WriteAllText("./processor/names.json", JsonSerializer.Serialize(names));
File.WriteAllText("./processor/actions.json", JsonSerializer.Serialize(actions));
