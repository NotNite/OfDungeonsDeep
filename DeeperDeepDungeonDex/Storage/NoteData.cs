using System.Collections.Generic;
using ImGuiNET;

namespace DeeperDeepDungeonDex.Storage;

public class NoteData {
    public List<string> Notes = new();
    public List<NoteData> Subnotes = new();

    public void Draw() {
        foreach (var note in Notes) {
            ImGui.TextUnformatted(note);

            ImGui.Indent();
            foreach (var subNote in Subnotes) {
                subNote.Draw();
            }
            ImGui.Unindent();
        }
    }
}
