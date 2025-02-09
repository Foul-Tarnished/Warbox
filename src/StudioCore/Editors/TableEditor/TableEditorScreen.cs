﻿using ImGuiNET;
using StudioCore.Configuration;
using StudioCore.Core.Data;
using StudioCore.Editor;
using StudioCore.Editors.TextEditor;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Veldrid.Sdl2;
using Veldrid;
using System.Xml.Linq;
using CommunityToolkit.HighPerformance;

namespace StudioCore.Editors.TableEditor;

public class TableEditorScreen : EditorScreen
{
    public string EditorName => "Tables";
    public string CommandEndpoint => "table";

    public TableEditorState EditorState;
    public TableFileSelectionView FileSelectionView;
    public TableDataView TableDataView;
    public TableToolsView TableToolsView;

    public ActionManager EditorActionManager = new();

    public TableEditorScreen(Sdl2Window window, GraphicsDevice device)
    {
        TableDefinition.Setup();

        EditorState = new(this);
        FileSelectionView = new(this);
        TableDataView = new(this);
        TableToolsView = new(this);
    }

    public void DrawEditorMenu()
    {
        if (ImGui.BeginMenu("File"))
        {
            // Save
            if (ImGui.MenuItem($"Save", KeyBindings.Current.CORE_Save.HintText))
            {
                Warbox.ProjectHandler.WriteProjectConfig(Warbox.ProjectHandler.CurrentProject);
                Save();
            }

            // Save PTF
            if (ImGui.MenuItem($"Save PTF", KeyBindings.Current.CORE_Save.HintText))
            {
                Warbox.ProjectHandler.WriteProjectConfig(Warbox.ProjectHandler.CurrentProject);
                SavePTF();
            }

            ImGui.EndMenu();
        }

        if (ImGui.BeginMenu("Edit"))
        {
            if (ImGui.MenuItem($"Undo", KeyBindings.Current.CORE_UndoAction.HintText, false,
                    EditorActionManager.CanUndo()))
            {
                EditorActionManager.UndoAction();
            }

            if (ImGui.MenuItem("Undo All", "", false,
                    EditorActionManager.CanUndo()))
            {
                EditorActionManager.UndoAllAction();
            }

            if (ImGui.MenuItem("Redo", KeyBindings.Current.CORE_RedoAction.HintText, false,
                    EditorActionManager.CanRedo()))
            {
                EditorActionManager.RedoAction();
            }

            ImGui.EndMenu();
        }
    }

    public void OnGUI(string[] initcmd)
    {
        var scale = Warbox.GetUIScale();

        // Docking setup
        ImGui.PushStyleColor(ImGuiCol.Text, CFG.Current.ImGui_Default_Text_Color);
        ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, new Vector2(4, 4) * scale);
        Vector2 wins = ImGui.GetWindowSize();
        Vector2 winp = ImGui.GetWindowPos();
        winp.Y += 20.0f * scale;
        wins.Y -= 20.0f * scale;
        ImGui.SetNextWindowPos(winp);
        ImGui.SetNextWindowSize(wins);

        var dsid = ImGui.GetID("DockSpace_TextEntries");
        ImGui.DockSpace(dsid, new Vector2(0, 0), ImGuiDockNodeFlags.None);

        Shortcuts();
        FileSelectionView.Shortcuts();
        TableDataView.Shortcuts();

        EditorCommandQueue(initcmd);

        FileSelectionView.Display();
        TableDataView.Display();
        TableToolsView.Display();

        ImGui.PopStyleVar();
        ImGui.PopStyleColor(1);
    }

    public void OnProjectChanged()
    {
        ResetActionManager();
    }

    public void Save()
    {
        var outputDir = $"{Warbox.ProjectDataRoot}\\Data";

        if (!Directory.Exists(outputDir))
            Directory.CreateDirectory(outputDir);

        var status = EditorState.SelectedStatus;
        var document = Warbox.DataHandler.Tables[status];

        var writePath = status.Path;
        var fileDir = $"{outputDir}\\{writePath}";

        // If it is a project-specific file, use the status Path as it is a full path
        if (writePath.Contains(outputDir))
        {
            fileDir = $"{writePath}";
        }

        document.Save(fileDir);

        TaskLogs.AddLog($"{fileDir} saved.");
    }

    public void SavePTF()
    {
        return;

        var outputDir = $"{Warbox.ProjectDataRoot}\\Data";
        var ptfName = Warbox.ProjectHandler.CurrentProject.Config.ProjectName.Replace(" ", "_").ToLower().Trim();


        if (!Directory.Exists(outputDir))
            Directory.CreateDirectory(outputDir);

        var status = EditorState.SelectedStatus;
        var outputDocument = new XDocument(Warbox.DataHandler.Tables[status]);

        if (status.Name.Contains("__"))
        {
            TaskLogs.AddLog($"This file is already a PTF table, you cannot save it as a PTF again.");
            return;
        }

        var writePath = status.Path;

        var vanillaEntry = Warbox.DataHandler.Vanilla_Tables.Where(e => e.Key.Name == status.Name).FirstOrDefault();
        var vanillaDocument = vanillaEntry.Value;

        // Create a hash set of serialized node strings for all elements in doc1
        var doc1Elements = vanillaDocument.Descendants()
            .Select(e => e.Name + "|" + string.Join("|", e.Attributes().Select(a => a.Name + "=" + a.Value)) + "|" + e.Value)
            .ToHashSet();

        // TODO: this is not working correctly

        // Remove matching elements from doc2
        outputDocument.Descendants()
            .Where(e => doc1Elements.Contains(e.Name + "|" + string.Join("|", e.Attributes().Select(a => a.Name + "=" + a.Value)) + "|" + e.Value))
            .Remove();

        var outputPath = writePath.Replace(".xml", $"__{ptfName}.xml");

        outputDocument.Save(outputPath);

        TaskLogs.AddLog($"{outputPath} saved.");
    }

    private void ResetActionManager()
    {
        EditorActionManager.Clear();
    }

    public void Shortcuts()
    {
        if (InputTracker.GetKeyDown(KeyBindings.Current.CORE_Save))
        {
            Warbox.ProjectHandler.WriteProjectConfig(Warbox.ProjectHandler.CurrentProject);
            Save();
        }

        if (InputTracker.GetKeyDown(KeyBindings.Current.CORE_SavePTF))
        {
            Warbox.ProjectHandler.WriteProjectConfig(Warbox.ProjectHandler.CurrentProject);
            SavePTF();
        }

        if (EditorActionManager.CanUndo() && InputTracker.GetKeyDown(KeyBindings.Current.CORE_UndoAction))
        {
            EditorActionManager.UndoAction();
        }

        if (EditorActionManager.CanRedo() && InputTracker.GetKeyDown(KeyBindings.Current.CORE_RedoAction))
        {
            EditorActionManager.RedoAction();
        }
    }

    public void EditorCommandQueue(string[] initcmd)
    {
        // Parse select commands
        if (initcmd != null && initcmd[0] == "select")
        {

        }
    }
}
