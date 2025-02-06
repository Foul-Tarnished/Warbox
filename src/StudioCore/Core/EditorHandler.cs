using ImGuiNET;
using StudioCore.Configuration;
using StudioCore.Editor;
using StudioCore.Editors.TableEditor;
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
    public TableEditorScreen TableEditor;

    public EditorHandler(IGraphicsContext _context)
    {
        TextEditor  = new TextEditorScreen(_context.Window, _context.Device);
        TableEditor = new TableEditorScreen(_context.Window, _context.Device);

        EditorList = [
            TableEditor,
            TextEditor
        ];

        FocusedEditor = TableEditor;
    }

    public void UpdateEditors()
    {
        foreach (EditorScreen editor in EditorList)
        {
            editor.OnProjectChanged();
        }
    }

    public void SaveFocusedEditor()
    {
        FocusedEditor.Save();
    }

    public void PackageFocusedEditor()
    {
        FocusedEditor.Package();
    }

    public void HandleEditorShortcuts()
    {
        if (InputTracker.GetKeyDown(KeyBindings.Current.CORE_Save))
        {
            Warbox.ProjectHandler.WriteProjectConfig(Warbox.ProjectHandler.CurrentProject);
            SaveFocusedEditor();
        }

        if (InputTracker.GetKeyDown(KeyBindings.Current.CORE_Package))
        {
            Warbox.ProjectHandler.WriteProjectConfig(Warbox.ProjectHandler.CurrentProject);
            PackageFocusedEditor();
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
            DisplayTaskStatus();
            if (ImGui.MenuItem("New Project", "", false, MayChangeProject()))
            {
                Warbox.ProjectHandler.ClearProject();
                Warbox.ProjectHandler.IsInitialLoad = true;
            }

            // Open Project
            DisplayTaskStatus();
            if (ImGui.MenuItem("Open Project", "", false, MayChangeProject()))
            {
                Warbox.ProjectHandler.OpenProjectDialog();
            }

            // Recent Projects
            DisplayTaskStatus();
            if (ImGui.BeginMenu("Recent Projects", MayChangeProject() && CFG.Current.RecentProjects.Count > 0))
            {
                Warbox.ProjectHandler.DisplayRecentProjects();

                ImGui.EndMenu();
            }

            // Open in Explorer
            if (ImGui.BeginMenu("Open in Explorer",
                    !TaskManager.AnyActiveTasks() && CFG.Current.RecentProjects.Count > 0))
            {
                if (ImGui.MenuItem("Project Folder", "", false))
                {
                    var projectPath = Warbox.ProjectDataRoot;
                    Process.Start("explorer.exe", projectPath);
                }

                if (ImGui.MenuItem("Game Folder", "", false))
                {
                    var gamePath = Warbox.DataRoot;
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
            if (ImGui.MenuItem($"Save", KeyBindings.Current.CORE_Save.HintText))
            {
                Warbox.ProjectHandler.WriteProjectConfig(Warbox.ProjectHandler.CurrentProject);
                SaveFocusedEditor();
            }

            // Package
            if (ImGui.MenuItem($"Package", KeyBindings.Current.CORE_Package.HintText))
            {
                Warbox.ProjectHandler.WriteProjectConfig(Warbox.ProjectHandler.CurrentProject);
                PackageFocusedEditor();
            }

            ImGui.EndMenu();
        }

        ImGui.Separator();
    }
}
