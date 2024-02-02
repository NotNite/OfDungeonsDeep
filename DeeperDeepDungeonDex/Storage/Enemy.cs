using System.Collections.Generic;
using Dalamud.Interface.Internal;
using DeeperDeepDungeonDex.Components;
using DeeperDeepDungeonDex.Controllers;

namespace DeeperDeepDungeonDex.Storage;

public class Enemy : IDrawableMob {
    public string Name => Plugin.GetEnemyName(this);
    public uint Id { get; set; }
    public int? Hp { get; set; }
    public AttackType? AttackType { get; set; }
    public uint? AttackDamage { get; set; }
    public string? AttackName { get; set; }
    public int StartFloor { get; set; }
    public int EndFloor { get; set; }
    public Aggro Aggro { get; set; }
    public Dictionary<Status, bool>? Vulnerabilities { get; set; }
    public string? Image { get; set; }
    public DeepDungeonType DungeonType { get; set; }
    public List<Ability?>? Abilities { get; set; }
    public IDalamudTextureWrap? ImageSmall { get; set; }
    public IDalamudTextureWrap? ImageLarge { get; set; }
}
