using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Numerics;
using Dalamud.Interface;
using Dalamud.Interface.Components;
using Dalamud.Interface.Internal;
using Dalamud.Interface.Utility;
using ImGuiNET;
using Lumina.Excel.GeneratedSheets2;
using OfDungeonsDeep.Storage;
using AttackType = OfDungeonsDeep.Storage.AttackType;
using Status = OfDungeonsDeep.Storage.Status;
using Storage_AttackType = OfDungeonsDeep.Storage.AttackType;
using Storage_Status = OfDungeonsDeep.Storage.Status;

namespace OfDungeonsDeep.Components;

public interface IDrawableMob {
    string Name { get; }
    uint Id { get; set; }
    int? Hp { get; set; }
    Storage_AttackType? AttackType { get; set; }
    uint? AttackDamage { get; set; }
    string? AttackName { get; set; }
    int StartFloor { get; set; }
    int EndFloor { get; set; }
    Aggro Aggro { get; set; }
    Dictionary<Storage_Status, bool>? Vulnerabilities { get; set; }
    string? Image { get; set; }
    DeepDungeonType DungeonType { get; set; }
    List<Ability?>? Abilities { get; set; }
    
    IDalamudTextureWrap? ImageSmall { get; set; }
    IDalamudTextureWrap? ImageLarge { get; set; }
    
    public void Draw(WindowExtraButton buttonType) {
        var topSegmentSize = new Vector2(ImGui.GetContentRegionAvail().X, 110.0f * ImGuiHelpers.GlobalScale);
        if (ImGui.BeginChild("TopSegment",  topSegmentSize, false)) {
            var portraitSize = new Vector2(110.0f * ImGuiHelpers.GlobalScale, 110.0f * ImGuiHelpers.GlobalScale);
            if (ImGui.BeginChild("##Portrait", portraitSize, false)) {
                DrawPortrait();
            }
            ImGui.EndChild();
            
            ImGui.SameLine();
            var portraitSideInfoSize = new Vector2(ImGui.GetContentRegionAvail().X, portraitSize.Y);
            if (ImGui.BeginChild("##PortraitSideInfo", portraitSideInfoSize, false)) {
                DrawPortraitSideInfo(buttonType);
            }
            ImGui.EndChild();
        }
        ImGui.EndChild();

        ImGui.Separator();
        
        DrawAbilityList();

        ImGui.Separator();
        
        DrawNotes();
    }

    private void DrawPortrait() {
        ImageSmall ??= Services.TextureProvider.GetTextureFromFile(new FileInfo( GetImagePath("Thumbnails")));
        ImageLarge ??= Services.TextureProvider.GetTextureFromFile(new FileInfo(GetImagePath("Images")));

        if (ImageSmall is not null && ImageLarge is not null) {
            var rectPosition = ImGui.GetCursorScreenPos();
            ImGui.Image(ImageSmall.ImGuiHandle, ImGui.GetContentRegionAvail());
            ImGui.GetWindowDrawList().AddRect(rectPosition, rectPosition + ImGui.GetContentRegionMax(), ImGui.GetColorU32(KnownColor.White.Vector() with { W = 0.75f }));
            
            if (ImGui.IsItemClicked()) {
                ImGui.OpenPopup("ImagePopup");
            }
        
            if (ImGui.BeginPopup("ImagePopup")) {
                ImGui.Image(ImageLarge.ImGuiHandle, ImageLarge.Size);
                ImGui.EndPopup();
            } else if (ImGui.IsItemHovered()) {
                ImGui.SetTooltip("Click to see Full Resolution");
            }
        }
    }
    
    private void DrawAbilityList() {
        Ability.DrawAbilityList(Abilities, this);
    }
        
    private void DrawNotes() {
        var mobEntry = this switch {
            Enemy => Strings.ResourceManager.GetString($"EnemyNote_{DungeonType.ToString()}_{Plugin.GetFloorSetId(StartFloor)}_{Id}"),
            FloorSet => Strings.ResourceManager.GetString($"FloorsetNote_{DungeonType.ToString()}_{Plugin.GetFloorSetId(StartFloor):000}"),
            _ => throw new ArgumentOutOfRangeException()
        };

        if (mobEntry is not null) {
            foreach (var line in mobEntry.Split("\n")) {
                var tabs = line.TakeWhile(char.IsWhiteSpace).Count();
                var cleanedLine = line.TrimStart().Replace("-", string.Empty);

                if (tabs > 0) {
                    ImGui.Indent(15.0f * tabs);
                }
                ImGuiHelpers.SafeTextWrapped(cleanedLine);
                if (tabs > 0) {
                    ImGui.Unindent(15 * tabs);
                }
            }
        } else {
            ImGui.TextUnformatted("No Notes");
        }
    }

    private void DrawPortraitSideInfo(WindowExtraButton buttonType) {
        if (ImGui.BeginTable("PortraitSideInfoTable", 2, ImGuiTableFlags.SizingStretchProp | ImGuiTableFlags.NoClip, ImGui.GetContentRegionAvail())) {
            ImGui.TableSetupColumn("##First", ImGuiTableColumnFlags.WidthStretch, 100);
            ImGui.TableSetupColumn("##Second", ImGuiTableColumnFlags.WidthStretch, 130);
            
            ImGui.TableNextColumn();
            DrawMobName();

            ImGui.TableNextColumn();
            DrawUtilityButton(buttonType);

            ImGui.TableNextRow();
            ImGui.TableNextColumn();
            DrawMobHealth();

            ImGui.TableNextColumn();
            DrawAutoAttackDamage();
                    
            ImGui.TableNextRow();
            ImGui.TableNextColumn();
            DrawFloorRange();

            ImGui.TableNextColumn();
            DrawAggroType();

            ImGui.TableNextRow();
            ImGui.TableNextColumn();
            DrawVulnerabilities();
                    
            ImGui.EndTable();
        }
    }
    
    private void DrawMobName() {
        ImGui.AlignTextToFramePadding();
        ImGui.TextUnformatted(Name);
    }
    
    private void DrawMobHealth() {
        ImGui.PushFont(UiBuilder.IconFont);
        ImGui.TextUnformatted(FontAwesomeIcon.Heart.ToIconString());
        ImGui.PopFont();
        ImGui.SameLine();
        ImGui.TextUnformatted(Hp.ToString());
    }
    
    private void DrawAutoAttackDamage() {
        switch (this) {
            case FloorSet when AttackDamage is not null:
                ImGui.PushFont(UiBuilder.IconFont);
                ImGui.TextUnformatted(FontAwesomeIcon.Explosion.ToIconString());
                ImGui.PopFont();
                ImGui.SameLine();
                ImGui.Text(AttackDamage.ToString());
                break;
            
            default:
                AttackType?.DrawAttack(AttackDamage, AttackName);
                break;
        }
    }
    
    private void DrawFloorRange() {
        ImGui.PushFont(UiBuilder.IconFont);
        ImGui.TextUnformatted(FontAwesomeIcon.Stairs.ToIconString());
        ImGui.PopFont();
        ImGui.SameLine();
        switch (this) {
            case FloorSet:
                ImGui.TextUnformatted($"{StartFloor + 9}");
                break;
            
            case Enemy:
                ImGui.TextUnformatted($"{StartFloor} - {EndFloor}");
                break;
        }
    }
    
    private void DrawAggroType() {
        Aggro.Draw();
    }
    
    private void DrawVulnerabilities() {
        if (Vulnerabilities is null) return;
        
        foreach (var (status, isVulnerable) in Vulnerabilities) {
            if (Services.TextureProvider.GetIcon((uint) status) is { } image) {
                if (status is not Status.Resolution) {
                    ImGui.Image(image.ImGuiHandle, image.Size * 0.5f, Vector2.Zero, Vector2.One, isVulnerable ? Vector4.One : Vector4.One / 2.5f );
                    if (ImGui.IsItemHovered() && Services.DataManager.GetExcelSheet<Lumina.Excel.GeneratedSheets2.Status>()?.FirstOrDefault(statusEffect => statusEffect.Icon == (uint)status) is {} statusInfo ) {
                        ImGui.SetTooltip(statusInfo.Name);
                    }
                } else {
                    ImGui.Image(image.ImGuiHandle, ImGuiHelpers.ScaledVector2(32.0f, 32.0f), Vector2.Zero, Vector2.One, isVulnerable ? Vector4.One : Vector4.One / 2.5f );
                    if (ImGui.IsItemHovered() && Services.DataManager.GetExcelSheet<DeepDungeonItem>()?.GetRow(16) is {} resolution) {
                        ImGui.SetTooltip(resolution.Name);
                    }
                }
                ImGui.SameLine();
            }
        }
    }
    
    private void DrawUtilityButton(WindowExtraButton buttonType) {
        switch (buttonType) {
            case WindowExtraButton.PopOutWithLock:
                DrawLockUnlockButton(ref Plugin.Configuration.LockTargetWindow);
                ImGui.SameLine();
                DrawPopoutButton(); 
                break;
                        
            case WindowExtraButton.CloseWithLock:
                DrawLockUnlockButton(ref Plugin.Configuration.LockedMobWindows);
                ImGui.SameLine();
                DrawCloseButton();
                break;
            
            case WindowExtraButton.PopOut:
                DrawPopoutButton();
                break;
        }
    }
    
    private void DrawCloseButton() {
        ImGui.SetCursorPosX(ImGui.GetContentRegionMax().X - 23.0f * ImGuiHelpers.GlobalScale);
        if (ImGuiComponents.IconButton("Button", FontAwesomeIcon.Times)) {
            Plugin.Controller.WindowController.RemoveWindowForEnemy(this);
        }
    }

    private void DrawLockUnlockButton(ref HashSet<uint> lockedIdSet) {
        ImGui.SetCursorPosX(ImGui.GetContentRegionMax().X - 23.0f * ImGuiHelpers.GlobalScale * 2.0f - ImGui.GetStyle().ItemSpacing.X);
        if (lockedIdSet.Contains(Id)) {
            if (ImGuiComponents.IconButton("Unlock", FontAwesomeIcon.Lock)) {
                lockedIdSet.Remove(Id);
                Plugin.Configuration.Save();
            }
        } else {
            if (ImGuiComponents.IconButton("Lock", FontAwesomeIcon.LockOpen)) {
                lockedIdSet.Add(Id);
                Plugin.Configuration.Save();
            }
        }
    }

    private void DrawPopoutButton() {
        ImGui.SetCursorPosX(ImGui.GetContentRegionMax().X - 23.0f * ImGuiHelpers.GlobalScale);
        if (ImGuiComponents.IconButton("Button", FontAwesomeIcon.ArrowUpRightFromSquare)) {
            Plugin.Controller.WindowController.TryAddDataWindow(this);
        }
    }
    
    private static void DrawLockUnlockButton(ref bool setting) {
        ImGui.SetCursorPosX(ImGui.GetContentRegionMax().X - 23.0f * ImGuiHelpers.GlobalScale * 2.0f - ImGui.GetStyle().ItemSpacing.X);
        if (setting) {
            if (ImGuiComponents.IconButton("Unlock", FontAwesomeIcon.Lock)) {
                setting = false;
                Plugin.Configuration.Save();
            }
        } else {
            if (ImGuiComponents.IconButton("Lock", FontAwesomeIcon.LockOpen)) {
                setting = true;
                Plugin.Configuration.Save();
            }
        }
    }

    private string GetImagePath(string folder) {
        if (Image is null) return string.Empty;
        
        return Path.Combine(
            Services.PluginInterface.AssemblyLocation.Directory?.FullName!,
            "Data",
            folder,
            DungeonType switch {
                DeepDungeonType.PalaceOfTheDead => "potd",
                DeepDungeonType.HeavenOnHigh => "hoh",
                DeepDungeonType.EurekaOrthos => "eo",
                _ => throw new ArgumentOutOfRangeException(nameof(folder))
            },
            Plugin.GetFloorSetId(StartFloor).ToString("000"),
            Image
        );
    }
}
