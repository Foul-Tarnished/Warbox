using ImGuiNET;
using StudioCore.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace StudioCore.Editors.TableEditor;

public static class TableContextMenu
{
    /// <summary>
    /// Handler for the context menu in all Generic Table Views row lists
    /// </summary>
    public static void DisplayTableRowEntryContextMenu(string imguiName, XDocument document, XElement element, int i)
    {
        if (ImGui.BeginPopupContextItem($"##{imguiName}EntryContext{i}"))
        {

            ImGui.EndPopup();
        }
    }
}
