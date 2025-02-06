using ImGuiNET;
using StudioCore.Editors.TableEditor.Views;
using StudioCore.Editors.TextEditor;
using StudioCore.Interface;
using StudioCore.KCD.Tables;
using StudioCore.TextEditor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudioCore.Editors.TableEditor;

public class TableDataView
{
    private TableEditorScreen Screen;
    private TableEditorState EditorState;

    private RpgParamView RpgParamView;

    public TableDataView(TableEditorScreen screen)
    {
        Screen = screen;
        EditorState = screen.EditorState;

        RpgParamView = new RpgParamView(screen, this);
    }

    public void Display()
    {
        if (ImGui.Begin("Table Data##tableDataView"))
        {
            switch (EditorState.SelectedTable)
            {
                case RpgParam:
                    RpgParamView.Display();
                    break;

                default:
                    ImGui.Text("This table is not supported yet.");
                    break;
            }

            ImGui.End();
        }
    }

    public void Shortcuts()
    {
        switch (EditorState.SelectedTable)
        {
            case RpgParam:
                RpgParamView.Shortcuts();
                break;
        }
    }
}
