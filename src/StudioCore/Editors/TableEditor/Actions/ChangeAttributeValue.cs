using StudioCore.Editor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace StudioCore.Editors.TableEditor.Actions;

public class ChangeAttributeValue : EditorAction
{
    private XAttribute Attribute;
    private string OldValue;
    private string NewValue;

    public ChangeAttributeValue(XAttribute attribute, string oldValue, string newValue)
    {
        Attribute = attribute;
        OldValue = oldValue;
        NewValue = newValue;
    }

    public override ActionEvent Execute()
    {
        Attribute.Value = NewValue;

        return ActionEvent.NoEvent;
    }

    public override ActionEvent Undo()
    {
        Attribute.Value = OldValue;

        return ActionEvent.NoEvent;
    }
}