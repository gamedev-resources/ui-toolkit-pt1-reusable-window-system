using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Handles keyboard input to toggle individual windows or close all windows.
/// Uses inline InputActions — no .inputactions asset required.
/// </summary>
public class UIController : MonoBehaviour
{
    [SerializeField] private WindowManager _windowManager;

    private InputAction _toggleInventory = new InputAction("Inventory", binding: "<Keyboard>/i");
    private InputAction _toggleEquipment = new InputAction("Equipment", binding: "<Keyboard>/h");
    private InputAction _closeAll = new InputAction("CloseAll", binding: "<Keyboard>/escape");

    /// <summary>
    /// Creates the default inventory and equipment windows at their initial positions.
    /// </summary>
    private void Start()
    {
        _windowManager.CreateWindow("inventory", "INVENTORY", new Vector2(50, 50));
        _windowManager.CreateWindow("equipment", "EQUIPMENT", new Vector2(200, 100));
    }

    /// <summary>
    /// Enables all input actions so they begin listening for key presses.
    /// </summary>
    private void OnEnable()
    {
        _toggleInventory.Enable();
        _toggleEquipment.Enable();
        _closeAll.Enable();
    }

    /// <summary>
    /// Disables all input actions to stop listening for key presses.
    /// </summary>
    private void OnDisable()
    {
        _toggleInventory.Disable();
        _toggleEquipment.Disable();
        _closeAll.Disable();
    }

    /// <summary>
    /// Polls input actions each frame and toggles the corresponding window
    /// or closes all windows when the mapped key is pressed.
    /// </summary>
    private void Update()
    {
        if (_toggleInventory.WasPressedThisFrame())
        {
            _windowManager.ToggleWindow("inventory");
        }
        if (_toggleEquipment.WasPressedThisFrame())
        { 
            _windowManager.ToggleWindow("equipment");
        }
        if (_closeAll.WasPressedThisFrame())
        { 
            _windowManager.CloseAllWindows();
        }
    }
}
