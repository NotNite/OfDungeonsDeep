using System.Collections.Generic;

namespace DeeperDeepDungeonDex.Storage;

public class JobSpecifics {
    public Difficulty Difficulty = Difficulty.Unrated;
    public List<NoteData> Notes = new();
}
