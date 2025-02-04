using ImGuiNET;
using StudioCore.TextEditor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudioCore.Editors.TextEditor;

public class FileSelectionView
{
    private TextEditorScreen Screen;
    private TextEditorState EditorState;

    public FileSelectionView(TextEditorScreen screen)
    {
        Screen = screen;
        EditorState = screen.EditorState;
    }

    public void Display()
    {
        if(ImGui.Begin("Files##textFileView"))
        {
            foreach(var entry in Warbox.DataHandler.Localization)
            {
                var status = entry.Key;
                var name = entry.Key.Name;

                if (ImGui.Selectable($"{name}##fileEntry{name}", EditorState.SelectedStatus == status))
                {
                    EditorState.UpdateSelection(entry);
                }
            }

            ImGui.End();
        }
    }

    public void Shortcuts()
    {

    }
}
