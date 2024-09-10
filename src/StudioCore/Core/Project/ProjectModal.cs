using ImGuiNET;
using StudioCore.Interface;
using StudioCore.Platform;
using StudioCore.Utilities;
using System;
using System.IO;
using System.Linq;
using System.Numerics;

namespace StudioCore.Core.Project;

public class ProjectModal
{
    public Project newProject;

    public string newProjectDirectory = "";

    public ProjectModal()
    {
        newProject = new Project();

        newProjectDirectory = "";
    }

    public bool IsLogicalDrive(string path)
    {
        return Directory.GetLogicalDrives().Contains(path);
    }

    public void Display()
    {
        ImGui.BeginTabBar("ProjectModelTabs");

        if (ImGui.BeginTabItem("Load Project"))
        {
            DisplayProjectLoadOptions();

            ImGui.EndTabItem();
        }
;
        if (ImGui.BeginTabItem("Create Project"))
        {
            DisplayNewProjectCreation();

            ImGui.EndTabItem();
        }

        ImGui.EndTabBar();
    }

    public void RecentProjectEntry(CFG.RecentProject p, int id)
    {
        if (ImGui.MenuItem($@"{p.GameType}: {p.Name}##{id}"))
        {
            if (File.Exists(p.ProjectFile))
            {
                var path = p.ProjectFile;

                Warbox.ProjectHandler.LoadProjectFromJSON(path);
                Warbox.ProjectHandler.IsInitialLoad = false;
            }
            else
            {
                DialogResult result = PlatformUtils.Instance.MessageBox(
                    $"Project file at \"{p.ProjectFile}\" does not exist.\n\n" +
                    $"Remove project from list of recent projects?",
                    $"Project.json cannot be found", MessageBoxButtons.YesNo);
                if (result == DialogResult.Yes)
                {
                    CFG.RemoveRecentProject(p);
                }
            }
        }

        if (ImGui.BeginPopupContextItem())
        {
            if (ImGui.Selectable("Remove from list"))
            {
                CFG.RemoveRecentProject(p);
                CFG.Save();
            }

            ImGui.EndPopup();
        }
    }

    public void DisplayProjectLoadOptions()
    {
        var scale = Warbox.GetUIScale();
        var width = ImGui.GetWindowWidth() / 100;

        if (CFG.Current.RecentProjects.Count > 0)
        {
            ImGui.Separator();
            UIHelper.WrappedText("Recent Projects");
            ImGui.Separator();

            Warbox.ProjectHandler.DisplayRecentProjects();

            ImGui.Separator();
        }

        if (ImGui.Button("Load New Project", new Vector2(width * 95, 32 * scale)))
        {
            Warbox.ProjectHandler.OpenProjectDialog();
        }

        if (CFG.Current.LastProjectFile != "")
        {
            if (ImGui.Button("Load Recent Project", new Vector2(width * 95, 32 * scale)))
            {
                Warbox.ProjectHandler.LoadRecentProject();
            }
        }
    }

    public void DisplayNewProjectCreation()
    {
        // Project Name
        ImGui.AlignTextToFramePadding();
        ImGui.Text("Project Name:      ");
        UIHelper.ShowHoverTooltip("Project's display name. Only affects visuals within Geobox.");
        ImGui.SameLine();

        var pname = newProject.Config != null ? newProject.Config.ProjectName : "Blank";

        if (ImGui.InputText("##pname", ref pname, 255))
        {
            newProject.Config.ProjectName = pname;
        }

        // Project Directory
        ImGui.AlignTextToFramePadding();
        ImGui.Text("Project Directory: ");
        UIHelper.ShowHoverTooltip("The mod directory.");
        ImGui.SameLine();
        ImGui.InputText("##pdir", ref newProjectDirectory, 255);
        ImGui.SameLine();
        if (ImGui.Button($@"{ForkAwesome.FileO}"))
        {
            if (PlatformUtils.Instance.OpenFolderDialog("Select project directory...", out var path))
            {
                if (IsLogicalDrive(path))
                {
                    DialogResult message = PlatformUtils.Instance.MessageBox(
                        "Project Directory has been placed in a drive root. This is not allowed. Please select a different location.", "Error",
                        MessageBoxButtons.OK);
                }
                else
                {
                    newProjectDirectory = path;
                }
            }
        }

        // Data Directory
        ImGui.AlignTextToFramePadding();
        ImGui.Text("Data Directory:    ");
        UIHelper.ShowHoverTooltip("The game data directory.");
        ImGui.SameLine();

        var gname = newProject.Config != null ? newProject.Config.GameRoot : "";
        if (ImGui.InputText("##dataDirectoryInput", ref gname, 255))
        {
            if (File.Exists(gname))
            {
                newProject.Config.GameRoot = Path.GetDirectoryName(gname);
            }
            else
            {
                newProject.Config.GameRoot = gname;
            }

            newProject.Config.GameType = Warbox.ProjectHandler.GetProjectTypeFromDirectory(gname);
        }

        ImGui.SameLine();

        if (ImGui.Button($@"{ForkAwesome.FileO}##dataDirectorySelect"))
        {
            if (PlatformUtils.Instance.OpenFolderDialog(
                    "Select game directory...",
                    out var path))
            {
                newProject.Config.GameRoot = Path.GetDirectoryName(path);
                newProject.Config.GameType = Warbox.ProjectHandler.GetProjectTypeFromDirectory(path);
            }
        }

        ImGui.Text($@"Project Type: {newProject.Config.GameType}");

        ImGui.Separator();

        // Create
        if (ImGui.Button("Create", new Vector2(120, 0) * Warbox.GetUIScale()))
        {
            newProject.ProjectJsonPath = $@"{newProjectDirectory}\project.json";

            bool validProject = CanCreateNewProject();

            if (validProject)
            {
                Warbox.ProjectHandler.WriteProjectConfig(newProject);

                Warbox.ProjectHandler.CurrentProject = newProject;

                // Only proceed if load is successful
                if (Warbox.ProjectHandler.LoadProject(newProject.ProjectJsonPath))
                    Warbox.ProjectHandler.IsInitialLoad = false;
            }
        }
    }

    public bool CanCreateNewProject()
    {
        var validated = true;

        if (newProject.Config.GameType != ProjectType.Undefined)
        {
            if (newProject.Config.GameRoot == null ||
                !Directory.Exists(newProject.Config.GameRoot))
            {
                PlatformUtils.Instance.MessageBox(
                    "Your game directory path does not exist. Please select a valid directory.", "Error",
                    MessageBoxButtons.OK);
                validated = false;
            }

            if (validated && newProject.Config.GameType == ProjectType.Undefined)
            {
                PlatformUtils.Instance.MessageBox("Your game directory is not a valid supported game.",
                    "Error",
                    MessageBoxButtons.OK);
                validated = false;
            }

            if (validated && (newProjectDirectory == null || !Directory.Exists(newProjectDirectory)))
            {
                PlatformUtils.Instance.MessageBox("Your selected project directory is not valid.", "Error",
                    MessageBoxButtons.OK);
                validated = false;
            }

            if (validated && File.Exists($@"{newProjectDirectory}\project.json"))
            {
                DialogResult message = PlatformUtils.Instance.MessageBox(
                    "Your selected project directory already contains a project.json. Would you like to replace it?",
                    "Error",
                    MessageBoxButtons.YesNo);
                if (message == DialogResult.No)
                {
                    validated = false;
                }
            }

            if (validated && newProject.Config.GameRoot == newProjectDirectory)
            {
                DialogResult message = PlatformUtils.Instance.MessageBox(
                    "Project Directory is the same as Game Directory, which allows game files to be overwritten directly.\n\n" +
                    "Continue and create project anyway?", "Caution",
                    MessageBoxButtons.OKCancel);
                if (message != DialogResult.OK)
                {
                    validated = false;
                }
            }

            if (validated && IsLogicalDrive(newProjectDirectory))
            {
                DialogResult message = PlatformUtils.Instance.MessageBox(
                    "Project Directory has been placed in a drive root. This is not allowed. Please select a different location.", "Error",
                    MessageBoxButtons.OK);
                validated = false;
            }

            if (validated && (newProject.Config.ProjectName == null || newProject.Config.ProjectName == ""))
            {
                PlatformUtils.Instance.MessageBox("You must specify a project name.", "Error",
                    MessageBoxButtons.OK);
                validated = false;
            }
        }
        else
        {
            PlatformUtils.Instance.MessageBox(
                    "No valid game directory has been detected. Please select a valid directory.", "Error",
                    MessageBoxButtons.OK);
            validated = false;
        }

        return validated;
    }
}
