using StudioCore.Editor;
using StudioCore.KCD;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace StudioCore.Editors.TextEditor.Actions;

public class AddTextRow : EditorAction
{
    private int RowIndex;
    private KCDText.Row NewRow;
    private KCDText SourceText;

    public AddTextRow(KCDText sourceText, int rowIndex)
    {
        SourceText = sourceText;
        RowIndex = rowIndex;
    }

    public override ActionEvent Execute()
    {
        NewRow = SourceText.Rows[RowIndex].NewCopy();

        SourceText.Rows.Insert(RowIndex, NewRow);

        return ActionEvent.NoEvent;
    }

    public override ActionEvent Undo()
    {
        SourceText.Rows.Remove(NewRow);

        return ActionEvent.NoEvent;
    }
}