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

public class TextRowView
{
    private TextEditorScreen Screen;
    private TextEditorState EditorState;

    public TextRowView(TextEditorScreen screen)
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

        if (curText == null || curText == null)
        {
            ImGui.Text("No XML document loaded.");
            return;
        }

        if (ImGui.Begin("Rows##textRowView"))
        {
            if(curStatus != null && curDocument != null && curText != null)
            {
                foreach(var entry in curText.Rows)
                {
                    var firstCell = entry.Cells.FirstOrDefault();

                    if (firstCell != null)
                    {
                        var name = firstCell;

                        if (ImGui.Selectable($"{name}##textRow{name}", entry == curTextRow))
                        {
                            EditorState.SelectedTextRow = entry;
                        }
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
