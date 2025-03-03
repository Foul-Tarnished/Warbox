﻿using ImGuiNET;
using StudioCore.Configuration;
using StudioCore.Configuration.Keybinds;
using StudioCore.Configuration.Settings;
using StudioCore.Graphics;
using StudioCore.Interface;
using StudioCore.Tools.Development;
using StudioCore.Utilities;

namespace StudioCore.Core;

/// <summary>
/// Handler class that holds all of the floating windows and related window state for access elsewhere.
/// </summary>
public class WindowHandler
{
    public SettingsWindow SettingsWindow;
    public DebugWindow DebugWindow;
    public KeybindWindow KeybindWindow;

    public WindowHandler(IGraphicsContext _context)
    {
        SettingsWindow = new SettingsWindow();
        DebugWindow = new DebugWindow();
        KeybindWindow = new KeybindWindow();
    }

    public void OnGui()
    {
        SettingsWindow.Display();
        KeybindWindow.Display();
        DebugWindow.Display();
    }

    public void HandleWindowShortcuts()
    {
        // Shortcut: Open Settings Window
        if (InputTracker.GetKeyDown(KeyBindings.Current.CORE_ConfigurationWindow))
        {
            SettingsWindow.ToggleMenuVisibility();
        }

        // Shortcut: Open Keybind Window
        if (InputTracker.GetKeyDown(KeyBindings.Current.CORE_KeybindConfigWindow))
        {
            KeybindWindow.ToggleMenuVisibility();
        }
    }

    public void HandleWindowIconBar()
    {
        ImGui.Separator();

        if (ImGui.Button($"Keybinds##KeybindWindow"))
        {
            KeybindWindow.ToggleMenuVisibility();
        }
        UIHelper.ShowHoverTooltip($"Keybinds\n{KeyBindings.Current.CORE_KeybindConfigWindow.HintText}");

        ImGui.Separator();

        if (ImGui.Button($"Settings##SettingsWindow"))
        {
            SettingsWindow.ToggleMenuVisibility();
        }
        UIHelper.ShowHoverTooltip($"Configuration\n{KeyBindings.Current.CORE_ConfigurationWindow.HintText}");

        if (CFG.Current.DisplayDebugTools)
        {
            ImGui.Separator();

            if (ImGui.Button($"Debugging##DebugWindow"))
            {
                DebugWindow.ToggleMenuVisibility();
            }
            UIHelper.ShowHoverTooltip($"Debug Tools");
        }
    }
}
