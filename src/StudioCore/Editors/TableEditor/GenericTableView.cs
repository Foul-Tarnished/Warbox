using ImGuiNET;
using StudioCore.Configuration;
using StudioCore.Editors.TextEditor;
using StudioCore.Interface;
using StudioCore.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace StudioCore.Editors.TableEditor;

public class GenericTableView
{
    private TableEditorScreen Screen;
    private TableEditorState EditorState;

    private string SearchKeyText = "";
    private string SearchValueText = "";

    public string Name = "";
    private string ImGuiName = "";
    private string NodeListAttributeKey = "";
    private string NodeListAliasKey = "";
    private string RowEntryKey = "";

    private string CurrentlySelectedRowEntry = "";

    private bool SelectNextRowEntry = false;

    public GenericTableView(TableEditorScreen screen, string name, string imguiName, string nodeListAttributeKey, string nodeListAliasKey, string rowEntryKey)
    {
        Screen = screen;
        EditorState = screen.EditorState;

        Name = name;
        ImGuiName = imguiName;
        NodeListAttributeKey = nodeListAttributeKey;
        NodeListAliasKey = nodeListAliasKey;
        RowEntryKey = rowEntryKey;
    }

    public void DisplayEntries()
    {
        var width = ImGui.GetWindowWidth();

        var currentDocument = EditorState.SelectedDocument;

        ImGui.SetNextItemWidth(width);
        ImGui.InputText($"##{ImGuiName}_KeySearchBar", ref SearchKeyText, 255);
        UIHelper.ShowHoverTooltip("Filters the list.");

        ImGui.Separator();

        if (ImGui.Button("Add New Entry", new Vector2(width, 24)))
        {

        }

        ImGui.BeginChild($"{ImGuiName}Section");

        List<XElement> elementList = currentDocument.Descendants(NodeListAttributeKey).ToList();

        for (int i = 0; i < elementList.Count + 1; i++)
        {
            if (i < elementList.Count)
            {
                var entry = elementList[i];
                var key = entry.Attribute(RowEntryKey).Value;
                var alias = "";
                XAttribute aliasAttribute = null;
                if (NodeListAliasKey != "")
                {
                    aliasAttribute = entry.Attribute(NodeListAliasKey);
                    if (aliasAttribute != null)
                    {
                        alias = aliasAttribute.Value;
                    }
                }

                if (!TextSearchFilters.FilterTableRowEntry(entry, alias, SearchKeyText))
                {
                    continue;
                }

                if (ImGui.Selectable($"{key}##{ImGuiName}selectEntry{i}", $"{key}{i}" == CurrentlySelectedRowEntry))
                {
                    CurrentlySelectedRowEntry = $"{key}{i}";
                }

                // Arrow Selection
                if (ImGui.IsItemHovered() && SelectNextRowEntry)
                {
                    SelectNextRowEntry = false;
                    CurrentlySelectedRowEntry = $"{key}{i}";
                }
                if (ImGui.IsItemFocused() && (InputTracker.GetKey(Veldrid.Key.Up) || InputTracker.GetKey(Veldrid.Key.Down)))
                {
                    SelectNextRowEntry = true;
                }

                if (alias != "")
                {
                    UIHelper.DisplayAlias(alias);
                }

                // Context
                if ($"{key}{i}" == CurrentlySelectedRowEntry)
                {
                    TableContextMenu.DisplayTableRowEntryContextMenu(ImGuiName, currentDocument, entry, i);
                }
            }
        }

        ImGui.EndChild();
    }

    public void DisplayProperties()
    {
        var width = ImGui.GetWindowWidth();

        ImGui.SetNextItemWidth(width);
        ImGui.InputText($"##{ImGuiName}_ValueSearchBar", ref SearchValueText, 255);
        UIHelper.ShowHoverTooltip("Filters the list.");

        ImGui.BeginChild($"{ImGuiName}PropertySection");

        var currentDocument = EditorState.SelectedDocument;

        List<XElement> rowEntries = currentDocument.Descendants(NodeListAttributeKey).ToList();
        var entry = rowEntries.Where(e => e.Attribute(RowEntryKey).Value == CurrentlySelectedRowEntry).FirstOrDefault();

        if (entry != null)
        {
            var attributes = entry.Attributes().ToList();

            //-------------------
            // Names
            //-------------------
            if (ImGui.BeginTable($"{ImGuiName}AttributeTable", 4, ImGuiTableFlags.SizingFixedFit))
            {
                ImGui.TableSetupColumn("Name", ImGuiTableColumnFlags.WidthFixed);
                ImGui.TableSetupColumn("Inputs", ImGuiTableColumnFlags.WidthFixed);
                ImGui.TableSetupColumn("Action", ImGuiTableColumnFlags.WidthFixed);
                ImGui.TableSetupColumn("Meta", ImGuiTableColumnFlags.WidthFixed);

                for (int i = 0; i < attributes.Count; i++)
                {
                    var attribute = attributes[i];

                    if (attribute != null)
                    {
                        if (!TextSearchFilters.FilterTableEntry(attribute.Value, SearchValueText))
                        {
                            continue;
                        }

                        ImGui.TableNextRow();

                        // Name Column
                        ImGui.TableSetColumnIndex(0);
                        ImGui.AlignTextToFramePadding();

                        var displayName = TableMeta.GetMetaString(
                            EditorState, 
                            "Name",
                            attribute.Name.ToString());

                        var description = TableMeta.GetMetaString(
                            EditorState,
                            "Description",
                            attribute.Name.ToString());

                        ImGui.SetNextItemWidth(width * 0.25f);
                        ImGui.Text(displayName);
                        UIHelper.ShowHoverTooltip(description);

                        // Inputs Column
                        ImGui.TableSetColumnIndex(1);

                        var tValue = attribute.Value;

                        ImGui.AlignTextToFramePadding();
                        ImGui.SetNextItemWidth(width * 0.5f);
                        ImGui.InputText($"##{ImGuiName}_input_{attribute.Name}{i}", ref tValue, 255);

                        attribute.Value = tValue;

                        // Action
                        ImGui.TableSetColumnIndex(2);
                        TableMeta.DisplayActionColumn(currentDocument, entry, attribute, i);

                        // Meta
                        ImGui.TableSetColumnIndex(3);
                        TableMeta.DisplayInfoColumn(currentDocument, entry, attribute, i);
                    }
                }

                ImGui.EndTable();
            }
        }

        TableMeta.DisplayAttributeAddSection(currentDocument, entry);

        ImGui.EndChild();
    }

    public void Shortcuts()
    {
        TableRowShortcuts.HandleRowShortcuts(EditorState.SelectedDocument, CurrentlySelectedRowEntry);
    }
}
