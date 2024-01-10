using VYaml.Annotations;

namespace DeeperDeepDungeonDex.Common;

[YamlObject(NamingConvention.SnakeCase)]
public partial class Enemy {
    public required string Name;
    public string? Nickname;
    public string? Family;

    public required int StartFloor;
    public required int EndFloor;
    public int? Hp;

    public bool Patrol = false;
    public required Aggro Agro; // Wish this wasn't misspelled in the markdown lmao
    public string? AttackName;
    public AttackType? AttackType;

    public Dictionary<Status, string> Vulnerabilities = new();    // true | false | unknown
    public Dictionary<string, JobSpecifics> JobSpecifics = new(); // key is job code (MCH etc
}
