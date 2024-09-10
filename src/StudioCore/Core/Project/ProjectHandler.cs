using ImGuiNET;
using StudioCore.Platform;
using StudioCore.UserProject;
using System;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Timers;
using static StudioCore.CFG;

namespace StudioCore.Core.Project;

public class ProjectHandler
{
    public Project CurrentProject;

    public ProjectModal ProjectModal;

    public Timer AutomaticSaveTimer;

    public RecentProject RecentProject;

    public bool IsInitialLoad = false;
    public bool ShowProjectLoadSelection = true;
    public bool RecentProjectLoad = false;

    public bool ImportRowNames = false;

    public ProjectHandler()
    {
        CurrentProject = new Project();
        ProjectModal = new ProjectModal();

        IsInitialLoad = true;
        UpdateProjectVariables();
    }
    public void OnGui()
    {
        if (!RecentProjectLoad && Current.Project_LoadRecentProjectImmediately)
        {
            RecentProjectLoad = true;
            IsInitialLoad = false;
            try
            {
                Warbox.ProjectHandler.LoadProjectFromJSON(Current.LastProjectFile);
            }
            catch (Exception ex)
            {
                TaskLogs.AddLog("Failed to load recent project.");
            }
        }

        if (IsInitialLoad)
        {
            ImGui.OpenPopup("Project Creation");
        }

        if (ImGui.BeginPopupModal("Project Creation", ref IsInitialLoad, ImGuiWindowFlags.NoTitleBar | ImGuiWindowFlags.AlwaysAutoResize))
        {
            ProjectModal.Display();

            ImGui.EndPopup();
        }
    }

    public void ReloadCurrentProject()
    {
        LoadProjectFromJSON(CurrentProject.ProjectJsonPath);
        Warbox.ProjectHandler.IsInitialLoad = false;
    }

    public bool LoadProject(string path)
    {
        if (CurrentProject.Config == null)
        {
            PlatformUtils.Instance.MessageBox(
                "Failed to load last project. Project will not be loaded after restart.",
                "Project Load Error", MessageBoxButtons.OK);
            return false;
        }

        if (path == "")
        {
            PlatformUtils.Instance.MessageBox(
                $"Path parameter was empty: {path}",
                "Project Load Error", MessageBoxButtons.OK);
            return false;
        }

        CurrentProject.ProjectJsonPath = path;

        SetGameRootDirectory(CurrentProject);

        Warbox.ProjectType = CurrentProject.Config.GameType;
        Warbox.GameRoot = CurrentProject.Config.GameRoot;
        Warbox.ProjectRoot = Path.GetDirectoryName(path);
        Warbox.SmithboxDataRoot = $"{Warbox.ProjectRoot}\\.warbox";

        if (Warbox.ProjectRoot == "")
            TaskLogs.AddLog("Warbox ProjectRoot is empty!");

        Warbox.SetProgramTitle($"{CurrentProject.Config.ProjectName} - Warbox");

        Warbox.EditorHandler.UpdateEditors();

        Current.LastProjectFile = path;
        Save();

        AddProjectToRecentList(CurrentProject);

        UpdateTimer();

        // Re-create this so project setup settings don't persist between projects (e.g. Import Row Names)
        ProjectModal = new ProjectModal();

        return true;
    }

    public bool LoadProjectFromJSON(string jsonPath)
    {
        if (CurrentProject == null)
        {
            CurrentProject = new Project();
        }

        // Fill CurrentProject.Config with contents
        CurrentProject.Config = ReadProjectConfig(jsonPath);

        if (CurrentProject.Config == null)
        {
            return false;
        }

        return LoadProject(jsonPath);
    }

    public void ClearProject()
    {
        CurrentProject = null;
        Warbox.SetProgramTitle("No Project - Warbox");
        Warbox.ProjectType = ProjectType.Undefined;
        Warbox.GameRoot = "";
        Warbox.ProjectRoot = "";
        Warbox.SmithboxDataRoot = "";
    }

    public void UpdateProjectVariables()
    {
        Warbox.SetProgramTitle($"{CurrentProject.Config.ProjectName} - Warbox");
        Warbox.ProjectType = CurrentProject.Config.GameType;
        Warbox.GameRoot = CurrentProject.Config.GameRoot;
        Warbox.ProjectRoot = Path.GetDirectoryName(CurrentProject.ProjectJsonPath);
        Warbox.SmithboxDataRoot = $"{Warbox.ProjectRoot}\\.warbox";
    }

    public void AddProjectToRecentList(Project targetProject)
    {
        // Add to recent project list
        RecentProject recent = new()
        {
            Name = targetProject.Config.ProjectName,
            GameType = targetProject.Config.GameType,
            ProjectFile = targetProject.ProjectJsonPath
        };
        AddMostRecentProject(recent);
    }

    public ProjectConfiguration ReadProjectConfig(string path)
    {
        var config = new ProjectConfiguration();

        if (File.Exists(path))
        {
            using (var stream = File.OpenRead(path))
            {
                config = JsonSerializer.Deserialize(stream, ProjectConfigurationSerializationContext.Default.ProjectConfiguration);
            }
        }

        return config;
    }

    public void WriteProjectConfig(Project targetProject)
    {
        if (targetProject == null)
            return;

        var config = targetProject.Config;
        var writePath = targetProject.ProjectJsonPath;

        if (writePath != "")
        {
            string jsonString = JsonSerializer.Serialize(config, typeof(ProjectConfiguration), ProjectConfigurationSerializationContext.Default);

            try
            {
                var fs = new FileStream(writePath, FileMode.Create);
                var data = Encoding.ASCII.GetBytes(jsonString);
                fs.Write(data, 0, data.Length);
                fs.Flush();
                fs.Dispose();
            }
            catch (Exception ex)
            {
                TaskLogs.AddLog($"{ex}");
            }
        }
    }

    public void SetGameRootDirectory(Project targetProject)
    {
        if (targetProject == null)
            return;

        if (!Directory.Exists(targetProject.Config.GameRoot))
        {
            PlatformUtils.Instance.MessageBox(
                $@"Could not find game data directory for {targetProject.Config.GameType}. Please select the game directory.",
                "Error",
                MessageBoxButtons.OK);

            while (true)
            {
                if (PlatformUtils.Instance.OpenFolderDialog(
                        $"Select game directory for {targetProject.Config.GameType}...",
                        out var path))
                {
                    targetProject.Config.GameRoot = path;
                    ProjectType gametype = GetProjectTypeFromDirectory(targetProject.Config.GameRoot);

                    if (gametype == targetProject.Config.GameType)
                    {
                        targetProject.Config.GameRoot = Path.GetDirectoryName(targetProject.Config.GameRoot);

                        WriteProjectConfig(targetProject);

                        break;
                    }

                    PlatformUtils.Instance.MessageBox(
                        $@"Selected directory was not for {CurrentProject.Config.GameType}. Please select the game directory.",
                        "Error",
                        MessageBoxButtons.OK);
                }
                else
                {
                    break;
                }
            }
        }
    }

    public ProjectType GetProjectTypeFromDirectory(string directory)
    {
        var type = ProjectType.Undefined;

        if (directory.Contains("Europa Universalis IV"))
        {
            type = ProjectType.EU4;
        }

        return type;
    }

    public void UpdateTimer()
    {
        if (AutomaticSaveTimer != null)
        {
            AutomaticSaveTimer.Close();
        }

        if (Current.System_EnableAutoSave)
        {
            var interval = Current.System_AutoSaveIntervalSeconds * 1000;
            if (interval < 10000)
                interval = 10000;

            AutomaticSaveTimer = new Timer(interval);
            AutomaticSaveTimer.Elapsed += OnAutomaticSave;
            AutomaticSaveTimer.AutoReset = true;
            AutomaticSaveTimer.Enabled = true;
        }
    }

    public void SaveCurrentProject()
    {
        WriteProjectConfig(CurrentProject);
    }

    public void OnAutomaticSave(object source, ElapsedEventArgs e)
    {
        if (Current.System_EnableAutoSave)
        {
            if (Warbox.ProjectType != ProjectType.Undefined)
            {
                if (Current.System_EnableAutoSave_Project)
                {
                    WriteProjectConfig(CurrentProject);
                }

                TaskLogs.AddLog($"Automatic Save occured at {e.SignalTime}");
            }
        }
    }

    public bool CreateRecoveryProject()
    {
        if (Warbox.GameRoot == null || Warbox.ProjectRoot == null)
            return false;

        try
        {
            var time = DateTime.Now.ToString("dd-MM-yyyy-(hh-mm-ss)", CultureInfo.InvariantCulture);

            Warbox.ProjectRoot = Warbox.ProjectRoot + $@"\recovery\{time}";

            if (!Directory.Exists(Warbox.ProjectRoot))
            {
                Directory.CreateDirectory(Warbox.ProjectRoot);
            }

            return true;
        }
        catch (Exception e)
        {
            return false;
        }
    }

    public void OpenProjectDialog()
    {
        var success = PlatformUtils.Instance.OpenFileDialog("Choose the project json file", new[] { "json" }, out var projectJsonPath);

        if (projectJsonPath != null)
        {
            if (projectJsonPath.Contains("project.json"))
            {
                if (LoadProjectFromJSON(projectJsonPath))
                {
                    Warbox.ProjectHandler.IsInitialLoad = false;
                }
            }
        }
    }

    public void LoadRecentProject()
    {
        // Only set this to false if recent project load is sucessful
        if (LoadProjectFromJSON(Current.LastProjectFile))
        {
            Warbox.ProjectHandler.IsInitialLoad = false;
        }
    }


    public void DisplayRecentProjects()
    {
        RecentProject = null;
        var id = 0;

        foreach (RecentProject p in Current.RecentProjects.ToArray())
        {
            // EU4
            if (p.GameType == ProjectType.EU4)
            {
                RecentProjectEntry(p, id);

                id++;
            }
        }
    }

    public void RecentProjectEntry(RecentProject p, int id)
    {
        // Just remove invalid recent projects immediately
        if (!File.Exists(p.ProjectFile))
        {
            RemoveRecentProject(p);
        }

        if (ImGui.MenuItem($@"{p.GameType}: {p.Name}##{id}"))
        {
            if (File.Exists(p.ProjectFile))
            {
                var path = p.ProjectFile;

                if (LoadProjectFromJSON(path))
                {
                    Warbox.ProjectHandler.IsInitialLoad = false;
                    UpdateProjectVariables();
                }
                else
                {
                    // Remove it if it failed
                    RemoveRecentProject(p);
                }
            }
            else
            {
                DialogResult result = PlatformUtils.Instance.MessageBox(
                    $"Project file at \"{p.ProjectFile}\" does not exist.\n\n" +
                    $"Remove project from list of recent projects?",
                    $"Project.json cannot be found", MessageBoxButtons.YesNo);
                if (result == DialogResult.Yes)
                {
                    RemoveRecentProject(p);
                }
            }
        }

        if (ImGui.BeginPopupContextItem())
        {
            if (ImGui.Selectable("Remove from list"))
            {
                RemoveRecentProject(p);
                Save();
            }

            ImGui.EndPopup();
        }
    }
}
