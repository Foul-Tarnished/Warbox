using ImGuiNET;
using StudioCore.Editors.TextEditor;
using StudioCore.Interface;
using StudioCore.TextEditor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace StudioCore.Editors.TableEditor;

public class TableDataView
{
    private TableEditorScreen Screen;
    private TableEditorState EditorState;

    private SortedDictionary<string, GenericTableView> TableViews = new();

    public TableDataView(TableEditorScreen screen)
    {
        Screen = screen;
        EditorState = screen.EditorState;

        foreach(var entry in Screen.TableDefinitions)
        {
            var name = entry.Attribute("Name").Value;
            var imguiName = entry.Attribute("ImGuiName").Value;
            var listKey = entry.Attribute("ListKey").Value;
            var aliasKey = entry.Attribute("AliasKey").Value;
            var rowKey = entry.Attribute("RowKey").Value;

            var newView = new GenericTableView(screen, name, imguiName, listKey, aliasKey, rowKey);

            TableViews.Add(name, newView);
        }

        TableMeta.Setup();
    }

    public void Display()
    {
        if (ImGui.Begin("Rows##tableRowView"))
        {
            var xmlName = GetSelectedXmlName();

            foreach(var entry in TableViews)
            {
                if (entry.Value.Name == xmlName)
                {
                    entry.Value.DisplayEntries();
                }
            }

            ImGui.End();
        }


        if (ImGui.Begin("Properties##tablePropertyView"))
        {
            var xmlName = GetSelectedXmlName();

            foreach (var entry in TableViews)
            {
                if (entry.Value.Name == xmlName)
                {
                    entry.Value.DisplayProperties();
                }
            }

            ImGui.End();
        }
    }

    public void Shortcuts()
    {
        var xmlName = GetSelectedXmlName();

        foreach (var entry in TableViews)
        {
            if (entry.Value.Name == xmlName)
            {
                entry.Value.Shortcuts();
            }
        }
    }

    public string GetSelectedXmlName()
    {
        if (EditorState.SelectedStatus == null)
            return "";

        var xmlName = EditorState.SelectedStatus.Name;

        // Only assess the relevant part of the file name (i.e. ignore PTF part)
        if (xmlName.Contains("__"))
        {
            xmlName = xmlName.Split("__")[0];
        }

        return xmlName;
    }
}
