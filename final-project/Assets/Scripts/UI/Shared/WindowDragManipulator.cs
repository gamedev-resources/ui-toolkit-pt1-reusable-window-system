using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// A PointerManipulator that enables dragging a window by its title bar.
/// Clamps the window position so at least 40px remains visible on screen.
/// </summary>
public class WindowDragManipulator : PointerManipulator
{
    private Vector2 _startPointerPosition;
    private Vector2 _startWindowPosition;
    private bool _isDragging;
    private VisualElement _windowRoot;

    private Vector2 _panelSize;
    private float _windowWidth;

    /// <summary>
    /// Initializes a new instance of the <see cref="WindowDragManipulator"/> class.
    /// </summary>
    /// <param name="dragHandle">The element that serves as the drag handle.</param>
    /// <param name="windowRoot">The root window element to be moved.</param>
    public WindowDragManipulator(VisualElement dragHandle, VisualElement windowRoot)
    {
        target = dragHandle;
        _windowRoot = windowRoot;
    }

    /// <summary>
    /// Handles pointer movement during a drag operation, updating the window position.
    /// </summary>
    /// <param name="evt">The pointer move event.</param>
    private void OnPointerMove(PointerMoveEvent evt)
    {
        if (!_isDragging) 
        {
            return;
        }

        Vector2 delta = (Vector2)evt.position - _startPointerPosition;
        Vector2 newPosition = _startWindowPosition + delta;

        // Clamp to screen bounds — keep at least 40px visible
        newPosition.x = Mathf.Clamp(newPosition.x, -(_windowWidth - 40), _panelSize.x - 40);
        newPosition.y = Mathf.Clamp(newPosition.y, 0, _panelSize.y - 40);

        _windowRoot.style.left = newPosition.x;
        _windowRoot.style.top = newPosition.y;
    }

    /// <summary>
    /// Handles pointer down to initiate a drag operation.
    /// </summary>
    /// <param name="evt">The pointer down event.</param>
    private void OnPointerDown(PointerDownEvent evt)
    {
        if (evt.button != 0)
        {
            return;
        }

        _startPointerPosition = evt.position;
        _startWindowPosition = new Vector2(_windowRoot.resolvedStyle.left, _windowRoot.resolvedStyle.top);

        // Resolve panel size and window width here (not in the constructor)
        // because the element isn't attached to a panel yet at construction time.
        _panelSize = _windowRoot.panel.visualTree.worldBound.size;
        _windowWidth = _windowRoot.resolvedStyle.width;

        _isDragging = true;
        _windowRoot.BringToFront();
        target.CapturePointer(evt.pointerId);
        evt.StopPropagation();
    }

    /// <summary>
    /// Handles pointer up to end the drag operation.
    /// </summary>
    /// <param name="evt">The pointer up event.</param>
    private void OnPointerUp(PointerUpEvent evt)
    {
        if (!_isDragging)
        {
            return;
        }

        _isDragging = false;
        target.ReleasePointer(evt.pointerId);
        evt.StopPropagation();
    }

    /// <summary>
    /// Registers pointer event callbacks on the target element.
    /// </summary>
    protected override void RegisterCallbacksOnTarget()
    {
        target.RegisterCallback<PointerDownEvent>(OnPointerDown);
        target.RegisterCallback<PointerMoveEvent>(OnPointerMove);
        target.RegisterCallback<PointerUpEvent>(OnPointerUp);
    }

    /// <summary>
    /// Unregisters pointer event callbacks from the target element.
    /// </summary>
    protected override void UnregisterCallbacksFromTarget()
    {
        target.UnregisterCallback<PointerDownEvent>(OnPointerDown);
        target.UnregisterCallback<PointerMoveEvent>(OnPointerMove);
        target.UnregisterCallback<PointerUpEvent>(OnPointerUp);
    }

}
