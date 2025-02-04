using ImGuiNET;
using StudioCore.Core.Data;
using StudioCore.KCD;
using StudioCore.TextEditor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace StudioCore.Editors.TextEditor;

public class TextEditorState
{
    private TextEditorScreen Screen;

    public DataStatus SelectedStatus;
    public XDocument SelectedDocument;
    public KCDText SelectedText;
    public KCDText.Row SelectedTextRow;

    public TextEditorState(TextEditorScreen screen)
    {
        Screen = screen;
    }

    public void UpdateSelection(KeyValuePair<DataStatus, XDocument> selection)
    {
        SelectedStatus = selection.Key;
        SelectedDocument = selection.Value;
        SelectedText = new KCDText(selection.Value);
    }
}
