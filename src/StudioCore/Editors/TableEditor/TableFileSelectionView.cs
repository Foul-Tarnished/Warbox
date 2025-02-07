using ImGuiNET;
using StudioCore.Configuration;
using StudioCore.Core.Data;
using StudioCore.Editors.TextEditor;
using StudioCore.Interface;
using StudioCore.TextEditor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace StudioCore.Editors.TableEditor;

public class TableFileSelectionView
{
    private TableEditorScreen Screen;
    private TableEditorState EditorState;

    private string SearchText = "";

    public TableFileSelectionView(TableEditorScreen screen)
    {
        Screen = screen;
        EditorState = screen.EditorState;
    }

    public void Display()
    {
        var width = ImGui.GetWindowWidth();

        if (ImGui.Begin("Files##tableFileView"))
        {
            ImGui.SetNextItemWidth(width * 0.75f);
            ImGui.InputText($"##tableFileSearchBar", ref SearchText, 255);
            UIHelper.ShowHoverTooltip("Filters the list.");

            ImGui.BeginChild("tableListSection");

            foreach(var entry in TableDefinition.Categories)
            {
                DisplayCategory(entry);
            }

            ImGui.EndChild();

            ImGui.End();
        }
    }

    private void DisplayCategory(string category)
    {
        ImGuiTreeNodeFlags flags = ImGuiTreeNodeFlags.DefaultOpen;

        if (ImGui.CollapsingHeader(category, flags))
        {
            for (int i = 0; i < Warbox.DataHandler.Tables.Count; i++)
            {
                var entry = Warbox.DataHandler.Tables.ElementAt(i);
                var status = entry.Key;
                var name = entry.Key.Name;

                var display = false;

                var tableEntry = TableDefinition.Definitions.Where(e => e.Attribute("Name").Value == name).FirstOrDefault();

                if (tableEntry != null && tableEntry.Attribute("Category") != null)
                {
                    if (tableEntry.Attribute("Category").Value == category)
                    {
                        display = true;
                    }
                }

                if (display)
                {
                    if (TextSearchFilters.FilterFileList(name, SearchText))
                    {
                        SelectionRow(i, entry, status, name);
                    }
                }
            }
        }
    }

    private void SelectionRow(int index, KeyValuePair<DataStatus, XDocument> entry, DataStatus status, string name)
    {
        // Typically the name of the file is one of the headers, so just do this
        var displayName = TableMeta.GetHeaderName(EditorState, "Name", $"{name}", true);

        if (ImGui.Selectable($"{displayName}##tableFileEntry{name}{index}", EditorState.SelectedStatus == status))
        {
            EditorState.UpdateSelection(entry);
        }

        // Arrow Selection
        if (ImGui.IsItemHovered() && EditorState.SelectNextTable)
        {
            EditorState.SelectNextTable = false;
            EditorState.UpdateSelection(entry);
        }
        if (ImGui.IsItemFocused() && (InputTracker.GetKey(Veldrid.Key.Up) || InputTracker.GetKey(Veldrid.Key.Down)))
        {
            EditorState.SelectNextTable = true;
        }

        // Context
        if (EditorState.SelectedStatus == status)
        {
            if (ImGui.BeginPopupContextItem($"##tableFileEntryContext{index}"))
            {

                ImGui.EndPopup();
            }
        }
    }

    public void Shortcuts()
    {
    }
}
