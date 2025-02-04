using System;
using StudioCore.UserProject;

namespace StudioCore.Core.Project;

/// <summary>
/// Core class representing a loaded project.
/// </summary>
public class Project
{
    /// <summary>
    /// The game interroot where all the game assets are
    /// </summary>
    public string GameRootDirectory { get; set; }

    /// <summary>
    /// An optional override mod directory where modded files are stored
    /// </summary>
    public string GameModDirectory { get; set; }

    /// Holds the configuration parameters from the project.json
    /// </summary>
    public ProjectConfiguration Config;

    /// <summary>
    /// Current project.json path.
    /// </summary>
    public string ProjectJsonPath;

    public Project()
    {
        GameRootDirectory = "";
        GameModDirectory = "";
        ProjectJsonPath = AppContext.BaseDirectory;
        Config = new ProjectConfiguration();
    }
}

