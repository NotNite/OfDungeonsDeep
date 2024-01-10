using System.Collections.Generic;

namespace DeeperDeepDungeonDex.Storage;

public class JobSpecifics {
    public Difficulty Difficulty = Difficulty.Unrated;
    public List<string> Timing = new();
    public List<NoteData> Notes = new();
}
