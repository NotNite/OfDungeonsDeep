using System.Collections.Generic;

namespace DeeperDeepDungeonDex.Storage;

public class Floorset {
    public required string Boss;
    public List<Ability> BossAbilities = new();
    public Dictionary<string, JobSpecifics> JobSpecifics = new();
}
