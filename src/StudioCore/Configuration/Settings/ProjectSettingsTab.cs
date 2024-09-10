using ImGuiNET;
using StudioCore.Interface;
using StudioCore.Utilities;

namespace StudioCore.Configuration.Settings;

public class ProjectSettingsTab
{
    public ProjectSettingsTab() { }

    public void Display()
    {
        if (ImGui.CollapsingHeader("General", ImGuiTreeNodeFlags.DefaultOpen))
        {
            ImGui.Checkbox("Enable Automatic Recent Project Loading", ref CFG.Current.Project_LoadRecentProjectImmediately);
            UIHelper.ShowHoverTooltip("The last loaded project will be automatically loaded when Smithbox starts up if this is enabled.");

            ImGui.Checkbox("Enable Recovery Folder", ref CFG.Current.System_EnableRecoveryFolder);
            UIHelper.ShowHoverTooltip("Enable a recovery project to be created upon an unexpected crash.");
        }

        if (ImGui.CollapsingHeader("Automatic Save", ImGuiTreeNodeFlags.DefaultOpen))
        {
            ImGui.Checkbox("Enable Automatic Save", ref CFG.Current.System_EnableAutoSave);
            UIHelper.ShowHoverTooltip("All changes will be saved at the interval specificed.");

            ImGui.Text("Automatic Save Interval");
            UIHelper.ShowHoverTooltip("Interval in seconds between each automatic save.");

            if (ImGui.InputInt("##AutomaticSaveInterval", ref CFG.Current.System_AutoSaveIntervalSeconds))
            {
                if (CFG.Current.System_AutoSaveIntervalSeconds < 10)
                {
                    CFG.Current.System_AutoSaveIntervalSeconds = 10;
                }

                Warbox.ProjectHandler.UpdateTimer();
            }

            ImGui.Text("Automatically Save:");
            UIHelper.ShowHoverTooltip("Determines which elements of Smithbox will be automatically saved, if automatic save is enabled.");

            ImGui.Checkbox("Project", ref CFG.Current.System_EnableAutoSave_Project);
            UIHelper.ShowHoverTooltip("The project.json will be automatically saved.");
        }
    }
}
