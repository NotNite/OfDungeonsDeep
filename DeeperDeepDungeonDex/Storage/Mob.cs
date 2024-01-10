using System.Collections.Generic;
using FFXIVClientStructs.FFXIV.Client.Game.InstanceContent;

namespace DeeperDeepDungeonDex.Storage;

public class Mob {
    public InstanceContentType InstanceContentType = InstanceContentType.DeepDungeon;
    public Threat Threat = Threat.Undefined;
    public Aggro Aggro = Aggro.Undefined;
    public List<Weakness> Weakness = new();
}
