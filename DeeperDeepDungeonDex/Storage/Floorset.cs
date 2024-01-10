using System.Collections.Generic;

namespace DeeperDeepDungeonDex.Storage;

public class Floorset {
    public required string Boss;
    public List<Ability> BossAbilities = new();
    public List<NoteData> BossNotes = new();
    public Dictionary<string, JobSpecifics> JobSpecifics = new();
}
