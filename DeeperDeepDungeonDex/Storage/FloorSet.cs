using System.Collections.Generic;
using Dalamud.Interface.Internal;
using DeeperDeepDungeonDex.System;

namespace DeeperDeepDungeonDex.Storage;

public class FloorSet : IDrawableMob {
    public required int Floor;
    
    public required string Boss;
    public List<Ability>? BossAbilities = new();
    public Dictionary<string, JobSpecifics> JobSpecifics = new();

    public string Name => Plugin.GetEnemyName(this);
    public uint Id { get; set; }
    public int? Hp { get; set; }
    public AttackType? AttackType { get; set; }
    public uint? AttackDamage { get; set; }
    public string? AttackName { get; set; }

    public int StartFloor {
        get => Floor;
        set => Floor = value;
    }
    
    public int EndFloor {
        get => Floor;
        set => Floor = value;
    }
    
    public Aggro Aggro { get; set; }
    public Dictionary<Status, bool>? Vulnerabilities { get; set; }
    public string? Image { get; set; }
    public DeepDungeonType DungeonType { get; set; }

    public List<Ability>? Abilities {
        get => BossAbilities;
        set => BossAbilities = value;
    }

    public IDalamudTextureWrap? ImageSmall { get; set; }
    public IDalamudTextureWrap? ImageLarge { get; set; }
}
