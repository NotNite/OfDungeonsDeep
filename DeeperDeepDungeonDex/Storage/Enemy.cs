using System.Collections.Generic;

namespace DeeperDeepDungeonDex.Storage;

public class Enemy {
    public required string Name;
    public required uint Id;
    public string? Nickname;
    public string? Family;
    public string? Image;

    public required int StartFloor;
    public required int EndFloor;
    public int? Hp;

    public required Aggro Aggro;
    public string? AttackName;
    public AttackType? AttackType;

    public List<NoteData> Notes = new();
    public Dictionary<Status, bool> Vulnerabilities = new();    // true | false | unknown
    public Dictionary<string, JobSpecifics> JobSpecifics = new(); // key is job code (MCH etc
}
