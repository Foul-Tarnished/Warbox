using ImGuiNET;
using StudioCore.Configuration;
using StudioCore.Editor;
using StudioCore.Graphics;
using StudioCore.Interface;
using StudioCore.TextEditor;
using StudioCore.Utilities;
using System.Collections.Generic;
using System.Diagnostics;

namespace StudioCore.Core;

/// <summary>
/// Handler class that holds all of the editors and related editor state for access elsewhere.
/// </summary>
public class EditorHandler
{
    public List<EditorScreen> EditorList;
    public EditorScreen FocusedEditor;

    public TextEditorScreen TextEditor;

    public EditorHandler(IGraphicsContext _context)
    {
        TextEditor  = new TextEditorScreen(_context.Window, _context.Device);

        EditorList = [
            TextEditor
        ];

        FocusedEditor = TextEditor;

        foreach (EditorScreen editor in EditorList)
        {
            editor.Init();
        }
    }

    public void UpdateEditors()
    {
        foreach (EditorScreen editor in EditorList)
        {
            editor.OnProjectChanged();
        }
    }

    public void SaveAllFocusedEditor()
    {
        FocusedEditor.SaveAll();
    }

    public void SaveFocusedEditor()
    {
        FocusedEditor.Save();
    }

    public void HandleEditorShortcuts()
    {
        if (InputTracker.GetKeyDown(KeyBindings.Current.CORE_Save))
        {
            Warbox.ProjectHandler.WriteProjectConfig(Warbox.ProjectHandler.CurrentProject);
            SaveFocusedEditor();
        }

        if (InputTracker.GetKeyDown(KeyBindings.Current.CORE_SaveAll))
        {
            Warbox.ProjectHandler.WriteProjectConfig(Warbox.ProjectHandler.CurrentProject);
            SaveAllFocusedEditor();
        }
    }

    private bool MayChangeProject()
    {
        if(TaskManager.AnyActiveTasks())
        {
            return false;
        }

        return true;
    }

    private void DisplayTaskStatus()
    {
        var status = "";

        if (TaskManager.AnyActiveTasks())
        {
            status = status + "Active tasks still on going.\n";
        }

        if (status != "")
        {
            UIHelper.ShowHoverTooltip(status);
        }
    }

    public void HandleEditorSharedBar()
    {
        ImGui.Separator();

        // Dropdown: File
        if (ImGui.BeginMenu("File"))
        {
            // New Project
            UIHelper.ShowMenuIcon($"{ForkAwesome.File}");
            DisplayTaskStatus();
            if (ImGui.MenuItem("New Project", "", false, MayChangeProject()))
            {
                Warbox.ProjectHandler.ClearProject();
                Warbox.ProjectHandler.IsInitialLoad = true;
            }

            // Open Project
            UIHelper.ShowMenuIcon($"{ForkAwesome.Folder}");
            DisplayTaskStatus();
            if (ImGui.MenuItem("Open Project", "", false, MayChangeProject()))
            {
                Warbox.ProjectHandler.OpenProjectDialog();
            }

            // Recent Projects
            UIHelper.ShowMenuIcon($"{ForkAwesome.FolderOpen}");
            DisplayTaskStatus();
            if (ImGui.BeginMenu("Recent Projects", MayChangeProject() && CFG.Current.RecentProjects.Count > 0))
            {
                Warbox.ProjectHandler.DisplayRecentProjects();

                ImGui.EndMenu();
            }

            // Open in Explorer
            UIHelper.ShowMenuIcon($"{ForkAwesome.Archive}");
            if (ImGui.BeginMenu("Open in Explorer",
                    !TaskManager.AnyActiveTasks() && CFG.Current.RecentProjects.Count > 0))
            {
                if (ImGui.MenuItem("Project Folder", "", false))
                {
                    var projectPath = Warbox.ProjectRoot;
                    Process.Start("explorer.exe", projectPath);
                }

                if (ImGui.MenuItem("Game Folder", "", false))
                {
                    var gamePath = Warbox.GameRoot;
                    Process.Start("explorer.exe", gamePath);
                }

                if (ImGui.MenuItem("Config Folder", "", false))
                {
                    var configPath = CFG.GetConfigFolderPath();
                    Process.Start("explorer.exe", configPath);
                }

                ImGui.EndMenu();
            }

            // Save
            UIHelper.ShowMenuIcon($"{ForkAwesome.FloppyO}");
            if (ImGui.MenuItem($"Save Selected {FocusedEditor.SaveType}",
                    KeyBindings.Current.CORE_Save.HintText))
            {
                Warbox.ProjectHandler.WriteProjectConfig(Warbox.ProjectHandler.CurrentProject);
                SaveFocusedEditor();
            }

            // Save All
            UIHelper.ShowMenuIcon($"{ForkAwesome.FloppyO}");
            if (ImGui.MenuItem($"Save All Modified {FocusedEditor.SaveType}", KeyBindings.Current.CORE_SaveAll.HintText))
            {
                Warbox.ProjectHandler.WriteProjectConfig(Warbox.ProjectHandler.CurrentProject);
                SaveAllFocusedEditor();
            }

            ImGui.EndMenu();
        }
    }
}
