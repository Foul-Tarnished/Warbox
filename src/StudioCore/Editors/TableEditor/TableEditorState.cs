using StudioCore.Core.Data;
using StudioCore.Editor;
using StudioCore.KCD;
using StudioCore.TextEditor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace StudioCore.Editors.TableEditor;

public class TableEditorState
{
    private TableEditorScreen Screen;

    public DataStatus SelectedStatus;
    public XDocument SelectedDocument;

    public bool SelectNextTable = false;


    public ActionManager EditorActionManager = new();

    public TableEditorState(TableEditorScreen screen)
    {
        Screen = screen;
    }

    public void UpdateSelection(KeyValuePair<DataStatus, XDocument> selection)
    {
        SelectedStatus = selection.Key;
        SelectedDocument = selection.Value;
    }
}
