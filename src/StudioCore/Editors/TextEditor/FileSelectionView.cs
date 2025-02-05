using ImGuiNET;
using StudioCore.Configuration;
using StudioCore.Interface;
using StudioCore.TextEditor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Assimp.Metadata;
using System.Xml.Linq;
using StudioCore.Core.Data;

namespace StudioCore.Editors.TextEditor;

public class FileSelectionView
{
    private TextEditorScreen Screen;
    private TextEditorState EditorState;

    private string SearchText = "";

    public FileSelectionView(TextEditorScreen screen)
    {
        Screen = screen;
        EditorState = screen.EditorState;
    }

    public void Display()
    {
        var width = ImGui.GetWindowWidth();

        if (ImGui.Begin("Files##textFileView"))
        {
            ImGui.SetNextItemWidth(width * 0.75f);
            ImGui.InputText($"##textFileViewSearch", ref SearchText, 255);
            UIHelper.ShowHoverTooltip("Filters the list.");

            ImGui.BeginChild("fileListSection");

            foreach (var entry in Warbox.DataHandler.Localization)
            {
                var status = entry.Key;
                var name = entry.Key.Name;

                if (TextSearchFilters.FilterFileList(name, SearchText))
                {
                    SelectionRow(entry, status, name);
                }
            }

            ImGui.EndChild();

            ImGui.End();
        }
    }

    private void SelectionRow(KeyValuePair<DataStatus, XDocument> entry, DataStatus status, string name)
    {
        if (ImGui.Selectable($"{name}##fileEntry{name}", EditorState.SelectedStatus == status))
        {
            EditorState.UpdateSelection(entry);
        }

        // Arrow Selection
        if (ImGui.IsItemHovered() && EditorState.SelectNextText)
        {
            EditorState.SelectNextText = false;
            EditorState.UpdateSelection(entry);
        }
        if (ImGui.IsItemFocused() && (InputTracker.GetKey(Veldrid.Key.Up) || InputTracker.GetKey(Veldrid.Key.Down)))
        {
            EditorState.SelectNextText = true;
        }

        // Context
        if (EditorState.SelectedStatus == status)
        {
            if (ImGui.BeginPopupContextItem($"##fileEntryContext"))
            {
                // Create
                if (ImGui.Selectable("Create"))
                {
                    var newStatus = new DataStatus("test", "test.xml");
                    var newDoc = new XDocument(status);

                    Warbox.DataHandler.Localization.Add(newStatus, newDoc);
                    // TODO
                }

                ImGui.EndPopup();
            }
        }
    }

    public void Shortcuts()
    {

    }
}
