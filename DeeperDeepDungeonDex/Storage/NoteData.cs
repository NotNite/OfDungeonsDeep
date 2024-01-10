using System.Collections.Generic;

namespace DeeperDeepDungeonDex.Storage;

public class NoteData {
    public List<string> Notes;
    public List<NoteData> Subnotes;
}
