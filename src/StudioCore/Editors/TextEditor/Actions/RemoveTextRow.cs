using StudioCore.Editor;
using StudioCore.KCD;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudioCore.Editors.TextEditor.Actions;

public class RemoveTextRow : EditorAction
{
    private int RowIndex;
    private KCDText.Row TargetRow;
    private KCDText SourceText;

    public RemoveTextRow(KCDText sourceText, int rowIndex)
    {
        SourceText = sourceText;
        RowIndex = rowIndex;
    }

    public override ActionEvent Execute()
    {
        TargetRow = SourceText.Rows[RowIndex].NewCopy();

        SourceText.Rows.RemoveAt(RowIndex);

        return ActionEvent.NoEvent;
    }

    public override ActionEvent Undo()
    {
        SourceText.Rows.Insert(RowIndex, TargetRow);

        return ActionEvent.NoEvent;
    }
}