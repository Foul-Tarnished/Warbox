using ImGuiNET;
using StudioCore.Configuration;
using StudioCore.Editor;
using System.Numerics;
using Veldrid;
using Veldrid.Sdl2;
using StudioCore.Utilities;
using StudioCore.Core.Project;
using StudioCore.Interface;
using StudioCore.Editors.TextEditor;

namespace StudioCore.TextEditor;

public class TextEditorScreen : EditorScreen
{
    public string EditorName => "Text Editor";
    public string CommandEndpoint => "text";

    public ActionManager EditorActionManager = new();

    public TextEditorState EditorState;
    public FileSelectionView FileSelectionView;
    public TextRowView TextRowView;
    public TextCellView TextCellView;

    public TextEditorScreen(Sdl2Window window, GraphicsDevice device)
    {
        EditorState = new(this);
        FileSelectionView = new(this);
        TextRowView = new(this);
        TextCellView = new(this);
    }

    public void DrawEditorMenu()
    {
        if (ImGui.BeginMenu("Edit"))
        {
            if (ImGui.MenuItem($"Undo", KeyBindings.Current.CORE_UndoAction.HintText, false,
                    EditorActionManager.CanUndo()))
            {
                EditorActionManager.UndoAction();
            }

            if (ImGui.MenuItem("Undo All", "", false,
                    EditorActionManager.CanUndo()))
            {
                EditorActionManager.UndoAllAction();
            }

            if (ImGui.MenuItem("Redo", KeyBindings.Current.CORE_RedoAction.HintText, false,
                    EditorActionManager.CanRedo()))
            {
                EditorActionManager.RedoAction();
            }

            ImGui.EndMenu();
        }
    }

    public void OnGUI(string[] initcmd)
    {
        var scale = Warbox.GetUIScale();

        // Docking setup
        ImGui.PushStyleColor(ImGuiCol.Text, CFG.Current.ImGui_Default_Text_Color);
        ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, new Vector2(4, 4) * scale);
        Vector2 wins = ImGui.GetWindowSize();
        Vector2 winp = ImGui.GetWindowPos();
        winp.Y += 20.0f * scale;
        wins.Y -= 20.0f * scale;
        ImGui.SetNextWindowPos(winp);
        ImGui.SetNextWindowSize(wins);

        var dsid = ImGui.GetID("DockSpace_TextEntries");
        ImGui.DockSpace(dsid, new Vector2(0, 0), ImGuiDockNodeFlags.None);

        Shortcuts();
        FileSelectionView.Shortcuts();
        TextRowView.Shortcuts();
        TextCellView.Shortcuts();

        EditorCommandQueue(initcmd);

        FileSelectionView.Display();
        TextRowView.Display();
        TextCellView.Display();

        ImGui.PopStyleVar();
        ImGui.PopStyleColor(1);
    }

    public void OnProjectChanged()
    {
        ResetActionManager();
    }

    public void Save()
    {
    }

    private void ResetActionManager()
    {
        EditorActionManager.Clear();
    }

    public void Shortcuts()
    {
        // Only allow key shortcuts when an item [text box] is not currently activated
        if (EditorActionManager.CanUndo() && InputTracker.GetKeyDown(KeyBindings.Current.CORE_UndoAction))
        {
            EditorActionManager.UndoAction();
        }

        if (EditorActionManager.CanRedo() && InputTracker.GetKeyDown(KeyBindings.Current.CORE_RedoAction))
        {
            EditorActionManager.RedoAction();
        }
    }

    public void EditorCommandQueue(string[] initcmd)
    {
        // Parse select commands
        if (initcmd != null && initcmd[0] == "select")
        {

        }
    }
}
