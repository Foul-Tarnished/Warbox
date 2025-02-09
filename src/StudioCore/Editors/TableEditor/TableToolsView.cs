using ImGuiNET;
using StudioCore.Configuration;
using StudioCore.Core.Data;
using StudioCore.Editors.TableEditor.Tools;
using StudioCore.Editors.TextEditor;
using StudioCore.Interface;
using StudioCore.TextEditor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace StudioCore.Editors.TableEditor;

public class TableToolsView
{
    private TableEditorScreen Screen;
    private TableEditorState EditorState;

    private string SearchText = "";

    public TableToolsView(TableEditorScreen screen)
    {
        Screen = screen;
        EditorState = screen.EditorState;
    }

    public void Display()
    {
        var width = ImGui.GetWindowWidth();

        if (ImGui.Begin("Tools##tableToolsView"))
        {
            ImGui.BeginTabBar("toolTabs");

            if (ImGui.BeginTabItem("Documentation"))
            {
                DisplayWiki();

                ImGui.EndTabItem();
            }

            if (ImGui.BeginTabItem("GUID Generator"))
            {
                DisplayGUIDGenerator();

                ImGui.EndTabItem();
            }

            if (ImGui.BeginTabItem("Property Search"))
            {
                DisplayPropertySearch();

                ImGui.EndTabItem();
            }

            if (ImGui.BeginTabItem("Mass Edit"))
            {
                DisplayMassEdit();

                ImGui.EndTabItem();
            }

            ImGui.EndTabBar();

            ImGui.End();
        }
    }

    public void Shortcuts()
    {

    }

    public void DisplayWiki()
    {

    }

    public void DisplayMassEdit()
    {

    }

    public void DisplayPropertySearch()
    {

    }

    private string guidOutput = "";

    public void DisplayGUIDGenerator()
    {
        var width = ImGui.GetWindowWidth();
        var buttonSize = new Vector2(width, 24);

        ImGui.SetNextItemWidth(width);
        ImGui.InputText("##guidOutput", ref guidOutput, 255);

        if(ImGui.Button("Generate", buttonSize))
        {
            var guid = TableGUIDGenerator.GenerateGuidV4();
            guidOutput = guid.ToString();
        }
    }
}
