using System.Collections.Generic;

namespace DeeperDeepDungeonDex.Storage;

public class NoteData {
    public List<string> Notes = new();
    public List<NoteData> Subnotes = new();
}
