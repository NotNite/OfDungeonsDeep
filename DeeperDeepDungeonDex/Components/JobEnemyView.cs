using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using Dalamud;
using Dalamud.Interface;
using Dalamud.Interface.Utility;
using DeeperDeepDungeonDex.Storage;
using ImGuiNET;
using Lumina.Excel.GeneratedSheets2;

namespace DeeperDeepDungeonDex.System;

public class JobEnemyView : EnemyView {
    private readonly Enemy enemy;
    private string? selectedJob;

    public JobEnemyView(Enemy enemyData) : base(enemyData, FontAwesomeIcon.ArrowUpRightFromSquare, () => Plugin.Controller.WindowController.TryAddMobDataWindow(enemyData)) {
        enemy = enemyData;
    }

    public override void Draw() {
        base.Draw();
        
        foreach (var (job, _) in enemy.JobSpecifics) {
            if (Services.TextureProvider.GetIcon(GetClassJobIcon(job)) is { } jobIcon) {
                var cursorPosition = ImGui.GetCursorScreenPos();
                
                if (selectedJob == job) {
                    ImGui.GetWindowDrawList().AddRectFilled(cursorPosition, cursorPosition + new Vector2(32.0f, 32.0f), ImGui.GetColorU32(KnownColor.Gray.Vector()));
                }
                
                ImGui.Image(jobIcon.ImGuiHandle, new Vector2(32.0f, 32.0f));
                ImGui.GetWindowDrawList().AddRect(cursorPosition, cursorPosition + new Vector2(32.0f, 32.0f), ImGui.GetColorU32(KnownColor.White.Vector()));

                if (ImGui.IsItemClicked()) {
                    selectedJob = job;
                }
            }
            
            
            //
            // ImGui.Text(tips.Difficulty.ToString());
            //
            // foreach (var timing in tips.Timing) {
            //     ImGui.Text(timing);
            // }
            //
            // DrawAllNotes(tips.Notes);
        }

        if (selectedJob is not null) {
            if (ImGui.BeginChild("JobTips", ImGui.GetContentRegionAvail(), true)) {
                if (ImGui.BeginTable("JobTipsTable", 2, ImGuiTableFlags.SizingStretchProp)) {
                    ImGui.TableNextColumn();
                    ImGui.Text("Difficulty");
                    
                    ImGui.TableNextColumn();
                    ImGui.Text(enemy.JobSpecifics[selectedJob].Difficulty.ToString());
                    
                    ImGui.EndTable();
                }
                
                ImGuiHelpers.ScaledDummy(5.0f);
                
            }
            ImGui.EndChild();
        }
    }
    
    private void DrawAllNotes(IEnumerable<NoteData> notes) {
        foreach (var note in notes) {
            DrawAllNotes(note);
        }
    }
    
    private void DrawAllNotes(NoteData notes) {
        foreach (var note in notes.Notes) {
            ImGui.Text(note);
        }

        foreach (var subNote in notes.Subnotes) {
            DrawAllNotes(subNote);
        }
    }

    private uint GetClassJobIcon(string classJob) {
        if (Services.DataManager.GetExcelSheet<ClassJob>(ClientLanguage.English)?
                .FirstOrDefault(job => string.Equals(job.Abbreviation.RawString, classJob, StringComparison.InvariantCultureIgnoreCase)) is {RowId: var jobRowId}) {
            return 62000u + jobRowId;
        }

        return 0;
    }
}
