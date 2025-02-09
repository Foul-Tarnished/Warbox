using StudioCore.Editor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace StudioCore.Editors.TableEditor.Actions;

public class RemoveTableRow : EditorAction
{
    private int RowIndex;
    private List<XElement> Elements;
    private XElement Row;


    public RemoveTableRow(List<XElement> elements, int rowIndex)
    {
        Elements = elements;
        RowIndex = rowIndex;
        Row = new XElement(Elements.ElementAt(rowIndex));
    }

    public override ActionEvent Execute()
    {
        Elements.RemoveAt(RowIndex);

        return ActionEvent.NoEvent;
    }

    public override ActionEvent Undo()
    {
        Elements.Insert(RowIndex, Row);

        return ActionEvent.NoEvent;
    }
}