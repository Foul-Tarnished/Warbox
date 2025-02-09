using ImGuiNET;
using StudioCore.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace StudioCore.Editors.TableEditor;

public static class TableMeta
{
    public static Dictionary<string, XDocument> Meta = new();

    public static void Setup()
    {
        // Get table meta
        var metaDir = $"{AppContext.BaseDirectory}\\Assets\\Data\\Meta\\";

        string[] xmlFiles = Directory.GetFiles(metaDir, "*.xml", SearchOption.AllDirectories);
        foreach (string file in xmlFiles)
        {
            var name = Path.GetFileNameWithoutExtension(file);
            XDocument doc = XDocument.Load(file);

            Meta.Add(name, doc);
        }
    }

    /// <summary>
    /// Get the 'pure' filename, without any appended parts
    /// </summary>
    public static string GetPureXmlName(string fileName)
    {
        // Only assess the relevant part of the file name (i.e. ignore PTF part)
        if (fileName.Contains("__"))
        {
            fileName = fileName.Split("__")[0];
        }

        return fileName;
    }

    /// <summary>
    /// Returns the header pretty name and description
    /// </summary>
    public static string GetHeaderName(TableEditorState editorState, string metaField, string nodeName, bool useName = false)
    {
        var displayedString = nodeName;
        var fileName = nodeName;

        if (!useName)
            fileName = GetPureXmlName(editorState.SelectedStatus.Name);

        if (Meta.ContainsKey(fileName))
        {
            var targetMeta = Meta[fileName];

            List<XElement> elements = targetMeta.Descendants($"{nodeName}").ToList();

            foreach (var entry in elements)
            {
                if (entry.Attribute(metaField) != null)
                {
                    return entry.Attribute(metaField).Value;
                }
            }
        }

        return displayedString;
    }

    /// <summary>
    /// Returns the attribute pretty name and description
    /// </summary>
    public static string GetAttributeName(TableEditorState editorState, string metaField, string nodeName, string attributeName)
    {
        var displayedString = attributeName;

        var fileName = GetPureXmlName(editorState.SelectedStatus.Name);

        if (Meta.ContainsKey(fileName))
        {
            var targetMeta = Meta[fileName];

            List<XElement> elements = targetMeta.Descendants($"{nodeName}-{attributeName}").ToList();

            foreach (var entry in elements)
            {
                if (entry.Attribute(metaField) != null)
                {
                    return entry.Attribute(metaField).Value;
                }
            }
        }

        return displayedString;
    }

    /// <summary>
    /// Returns true if the attribute is marked as bool
    /// </summary>
    public static bool IsBoolAttribute(TableEditorState editorState, string metaField, string nodeName, string attributeName)
    {
        var fileName = GetPureXmlName(editorState.SelectedStatus.Name);

        if (Meta.ContainsKey(fileName))
        {
            var targetMeta = Meta[fileName];

            List<XElement> elements = targetMeta.Descendants($"{nodeName}-{attributeName}").ToList();

            foreach (var entry in elements)
            {
                if (entry.Attribute(metaField) != null)
                {
                    return true;
                }
            }
        }

        return false;
    }

    /// <summary>
    /// Handler for the action column in all Generic Table Views
    /// </summary>
    public static void DisplayActionColumn(XDocument document, XElement element, XAttribute attribute, int i)
    {
        // TODO: add check against meta, only display if needed
        return;

        if (ImGui.Button($"{ForkAwesome.ArrowDown}##exampleButton{attribute.Name}{i}"))
        {
            ImGui.OpenPopup($"examplePopup{attribute.Name}{i}");
        }

        if (ImGui.BeginPopup($"examplePopup{attribute.Name}{i}"))
        {
            ImGui.Text("enum stuff here");

            ImGui.EndPopup();
        }
    }

    /// <summary>
    /// Handler for the info column in all Generic Table Views
    /// </summary>
    public static void DisplayInfoColumn(XDocument document, XElement element, XAttribute attribute, int i)
    {
        var width = ImGui.GetWindowWidth();

        // TODO: add check against meta, only display if needed
        return;

        ImGui.AlignTextToFramePadding();
        ImGui.SetNextItemWidth(width * 0.15f);
        ImGui.Text("TODO: meta stuff here");
    }

    /// <summary>
    /// Handler for the attribute add in all Generic Table Views
    /// </summary>
    public static void DisplayAttributeAddSection(XDocument document, XElement element)
    {
        var width = ImGui.GetWindowWidth();
        return;

        // META will contain list of all possible attributes,
        // this section will allow the user to add any that are mossing from the current element
    }
}

public class AttributeDescriptor
{
    public string Name { get; set; }
    public string ScriptName { get; set; }
    public string Description { get; set; }
    public string ReferenceParameters { get; set; }
}