using ImGuiNET;
using StudioCore.Configuration;
using StudioCore.Core.Data;
using StudioCore.Interface;
using StudioCore.KCD;
using StudioCore.TextEditor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace StudioCore.Editors.TextEditor;

public class TextRowView
{
    private TextEditorScreen Screen;
    private TextEditorState EditorState;

    private string SearchText = "";

    public TextRowView(TextEditorScreen screen)
    {
        Screen = screen;
        EditorState = screen.EditorState;
    }

    public void Display()
    {
        var width = ImGui.GetWindowWidth();

        var curStatus = EditorState.SelectedStatus;
        var curDocument = EditorState.SelectedDocument;
        var curText = EditorState.SelectedText;

        if (ImGui.Begin("Rows##textRowView"))
        {
            if (curText == null || curText == null)
            {
                ImGui.Text("No text file selected.");
            }
            else
            {
                ImGui.SetNextItemWidth(width * 0.75f);
                ImGui.InputText($"##textRowViewSearch", ref SearchText, 255);
                UIHelper.ShowHoverTooltip("Filters the list.");

                ImGui.BeginChild("rowListSection");

                if (curStatus != null && curDocument != null && curText != null)
                {
                    foreach (var entry in curText.Rows)
                    {
                        var firstCell = entry.Cells.FirstOrDefault();

                        if (firstCell != null)
                        {
                            var name = firstCell;

                            var text1 = "";
                            if (entry.Cells.Count >= 2)
                                text1 = entry.Cells[1];

                            var text2 = "";
                            if (entry.Cells.Count >= 3)
                                text2 = entry.Cells[2];

                            if (TextSearchFilters.FilterRowList(name, text1, text2, SearchText))
                            {
                                SelectionRow(entry, name, text1);
                            }
                        }
                    }
                }

                ImGui.EndChild();
            }

            ImGui.End();
        }
    }

    private void SelectionRow(KCDText.Row entry, string name, string text1)
    {
        var curTextRow = EditorState.SelectedTextRow;

        if (ImGui.Selectable($"{name}##textRow{name}", entry == curTextRow))
        {
            EditorState.SelectedTextRow = entry;
        }

        // Only display aliases for entries of reasonable length
        if (EditorState.SelectedStatus.Name != "text_ui_dialog")
        {
            if (text1 != "" && text1.Length < 100)
            {
                UIHelper.DisplayAlias(text1);
            }
        }

        // Arrow Selection
        if (ImGui.IsItemHovered() && EditorState.SelectNextTextRow)
        {
            EditorState.SelectNextTextRow = false;
            EditorState.SelectedTextRow = entry;
        }
        if (ImGui.IsItemFocused() && (InputTracker.GetKey(Veldrid.Key.Up) || InputTracker.GetKey(Veldrid.Key.Down)))
        {
            EditorState.SelectNextTextRow = true;
        }

        // Context
        if (entry == curTextRow)
        {
            if (ImGui.BeginPopupContextItem($"##textRowContext"))
            {
                // Duplicate
                if (ImGui.Selectable("Duplicate"))
                {
                    SuspendView = true;
                    EditorState.SelectedText.Rows.Add(entry.DeepCopy());
                }

                // Remove
                if (ImGui.Selectable("Remove"))
                {
                    SuspendView = true;
                    var rowToRemove = EditorState.SelectedText.Rows.Where(e => e.Equals(entry)).FirstOrDefault();
                    EditorState.SelectedText.Rows.Remove(rowToRemove);
                }

                ImGui.EndPopup();
            }
        }
    }

    public void Shortcuts()
    {

    }
}
