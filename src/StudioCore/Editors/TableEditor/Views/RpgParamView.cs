using ImGuiNET;
using StudioCore.Editors.TextEditor;
using StudioCore.Interface;
using StudioCore.KCD.Tables;
using StudioCore.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace StudioCore.Editors.TableEditor.Views;

public class RpgParamView
{
    private TableEditorScreen Screen;
    private TableEditorState EditorState;
    private TableDataView DataView;

    private string SearchKeyText = "";
    private string SearchValueText = "";

    private bool AllowKeyEdit = false;
    private float KeyColumnWidth = 0.3f;
    private float ValueColumnWidth = 0.6f;

    public RpgParamView(TableEditorScreen screen, TableDataView dataView)
    {
        Screen = screen;
        EditorState = screen.EditorState;
        DataView = dataView;
    }

    public void Display()
    {
        var width = ImGui.GetWindowWidth();

        var currentTable = (RpgParam)EditorState.SelectedTable;
        var entries = currentTable.RpgParams.RpgParamList;

        ImGui.SetNextItemWidth(width * KeyColumnWidth);
        ImGui.DragFloat("##keyColumnWidth", ref KeyColumnWidth, 0.01f, 0.1f, 1.0f);
        UIHelper.ShowHoverTooltip("Edit the width of the key column.");
        ImGui.SameLine();

        ImGui.SetNextItemWidth(width * ValueColumnWidth);
        ImGui.DragFloat("##valueColumnWidth", ref ValueColumnWidth, 0.01f, 0.1f, 1.0f);
        UIHelper.ShowHoverTooltip("Edit the width of each value column.");
        ImGui.SameLine();

        if (AllowKeyEdit)
        {
            if (ImGui.Button($"{ForkAwesome.Lock}"))
            {
                AllowKeyEdit = false;
            }
            UIHelper.ShowHoverTooltip("Disable the ability to edit the keys.");
        }
        else
        {
            if (ImGui.Button($"{ForkAwesome.Unlock}"))
            {
                AllowKeyEdit = true;
            }
            UIHelper.ShowHoverTooltip("Enable the ability to edit the keys.");
        }

        ImGui.Separator();

        ImGui.SetNextItemWidth(width * KeyColumnWidth);
        ImGui.InputText($"##rpgParam_KeySearchBar", ref SearchKeyText, 255);
        UIHelper.ShowHoverTooltip("Filters the list.");

        ImGui.SameLine();

        ImGui.SetNextItemWidth(width * ValueColumnWidth);
        ImGui.InputText($"##rpgParam_ValueSearchBar", ref SearchValueText, 255);
        UIHelper.ShowHoverTooltip("Filters the list.");

        ImGui.Separator();

        if (ImGui.Button("Add New Entry", new Vector2(width * KeyColumnWidth, 24)))
        {

        }

        ImGui.BeginChild("rpgParamSection");

        for (int i = 0; i < entries.Count + 1; i++)
        {
            if (i < entries.Count)
            {
                var entry = entries[i];
                var key = entry.Key;
                var value = entry.Value;

                if (!TextSearchFilters.FilterTableKey(key, SearchKeyText))
                {
                    continue;
                }

                if (!TextSearchFilters.FilterTableKey(value, SearchValueText))
                {
                    continue;
                }

                ImGui.SetNextItemWidth(width * KeyColumnWidth);
                if (AllowKeyEdit)
                {
                    if (ImGui.InputText($"##keyInput{i}", ref key, 255))
                    {
                        entry.Key = key;
                    }
                }
                else
                {
                    if (ImGui.InputText($"##keyInput{i}", ref key, 255, ImGuiInputTextFlags.ReadOnly))
                    {
                        entry.Key = key;
                    }
                }

                ImGui.SameLine();

                ImGui.SetNextItemWidth(width * ValueColumnWidth);
                if (ImGui.InputText($"##valueInput{i}", ref value, 255))
                {
                    entry.Value = value;
                }
            }
        }

        ImGui.EndChild();
    }

    public void Shortcuts()
    {

    }
}
