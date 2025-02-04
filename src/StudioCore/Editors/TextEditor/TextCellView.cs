using ImGuiNET;
using StudioCore.Core.Data;
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

public class TextCellView
{
    private TextEditorScreen Screen;
    private TextEditorState EditorState;

    public TextCellView(TextEditorScreen screen)
    {
        Screen = screen;
        EditorState = screen.EditorState;
    }

    public void Display()
    {
        var curStatus = EditorState.SelectedStatus;
        var curDocument = EditorState.SelectedDocument;
        var curText = EditorState.SelectedText;
        var curTextRow = EditorState.SelectedTextRow;

        if (curTextRow == null || curTextRow == null)
        {
            ImGui.Text("No text row selected.");
            return;
        }

        if (ImGui.Begin("Entries##textCellView"))
        {
            if (curTextRow != null)
            {
                for(int i = 0; i < curTextRow.Cells.Count; i++)
                {
                    var cell = curTextRow.Cells[i];

                    if(ImGui.InputText($"##textEntry{i}", ref cell, 255))
                    {

                    }
                }
            }

            ImGui.End();
        }
    }

    public void Shortcuts()
    {

    }
}
