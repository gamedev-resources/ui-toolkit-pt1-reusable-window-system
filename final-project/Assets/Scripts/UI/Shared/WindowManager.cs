using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// Manages creation, toggling, and persistence of all game windows.
/// Provides a full-screen container for windows and handles position save/restore via PlayerPrefs.
/// </summary>
public class WindowManager : MonoBehaviour
{
    [SerializeField] private UIDocument _uiDocument;
    [SerializeField] private VisualTreeAsset _gameWindowTemplate;

    private VisualElement _windowContainer;
    private Dictionary<string, GameWindow> _windows = new();

    private void Awake()
    {
        var root = _uiDocument.rootVisualElement;

        // Full-screen container for all windows — ignores clicks so they pass through
        _windowContainer = new VisualElement();
        _windowContainer.name = "window-container";
        _windowContainer.style.position = Position.Absolute;
        _windowContainer.style.width = Length.Percent(100);
        _windowContainer.style.height = Length.Percent(100);
        _windowContainer.pickingMode = PickingMode.Ignore;
        root.Add(_windowContainer);
    }

    /// <summary>
    /// Creates a new window, restoring its position from PlayerPrefs if available.
    /// </summary>
    /// <param name="id">Unique identifier used for persistence and toggle lookups.</param>
    /// <param name="title">Display title shown in the window's title bar.</param>
    /// <param name="defaultPosition">Fallback position if no saved position exists.</param>
    public GameWindow CreateWindow(string id, string title, Vector2 defaultPosition)
    {
        float x = PlayerPrefs.GetFloat($"window_{id}_x", defaultPosition.x);
        float y = PlayerPrefs.GetFloat($"window_{id}_y", defaultPosition.y);

        var window = new GameWindow(_gameWindowTemplate, _windowContainer, title);
        window.SetPosition(x, y);

        _windows[id] = window;
        return window;
    }

    /// <summary>
    /// Toggles a window's visibility. Saves position when hiding.
    /// </summary>
    public void ToggleWindow(string id)
    {
        if (!_windows.TryGetValue(id, out var window)) return;

        if (window.IsVisible)
        {
            SaveWindowPosition(id, window);
            window.Hide();
        }
        else
        {
            window.Show();
        }
    }

    /// <summary>
    /// Hides all visible windows, saving each one's position.
    /// </summary>
    public void CloseAllWindows()
    {
        foreach (var kvp in _windows)
        {
            if (kvp.Value.IsVisible)
            {
                SaveWindowPosition(kvp.Key, kvp.Value);
                kvp.Value.Hide();
            }
        }
    }

    /// <summary>
    /// Persists a single window's position to PlayerPrefs.
    /// </summary>
    private void SaveWindowPosition(string id, GameWindow window)
    {
        var pos = window.GetPosition();
        PlayerPrefs.SetFloat($"window_{id}_x", pos.x);
        PlayerPrefs.SetFloat($"window_{id}_y", pos.y);
    }

    /// <summary>
    /// Saves all visible window positions and flushes PlayerPrefs to disk.
    /// </summary>
    private void SaveAllPositions()
    {
        foreach (var kvp in _windows)
        {
            if (kvp.Value.IsVisible)
                SaveWindowPosition(kvp.Key, kvp.Value);
        }
        PlayerPrefs.Save();
    }

    private void OnApplicationQuit()
    {
        SaveAllPositions();
    }
}