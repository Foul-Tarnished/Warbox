using StudioCore.Core.Data;
using StudioCore.Editor;
using StudioCore.KCD;
using StudioCore.KCD.Tables;
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

    public IKCDTable SelectedTable;

    public bool SelectNextTable = false;


    public ActionManager EditorActionManager = new();

    public TableEditorState(TableEditorScreen screen)
    {
        Screen = screen;
    }
    public void UpdateSelection(KeyValuePair<DataStatus, XDocument> selection)
    {
        UpdateSelectedDocument();

        SelectedStatus = selection.Key;
        SelectedDocument = selection.Value;

        SelectedTable = CreateTable(selection);
    }
    public void UpdateSelectedDocument()
    {
        if (SelectedTable != null && SelectedStatus.Modified)
        {
            if (Warbox.DataHandler.Tables.ContainsKey(SelectedStatus))
            {
                //Warbox.DataHandler.Tables[SelectedStatus] = SelectedText.ExportXML();
            }
        }
    }

    public IKCDTable CreateTable(KeyValuePair<DataStatus, XDocument> selection)
    {
        switch(selection.Key.Name)
        {
            case "rpg_param": return XmlHelper.DeserializeFromXDocument<RpgParam>(selection.Value);
        }
        
        // Return dummy for unsupport tables
        return new Dummy();
    }
}
