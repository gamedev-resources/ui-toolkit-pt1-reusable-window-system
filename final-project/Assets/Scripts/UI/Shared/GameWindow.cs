using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// Represents a draggable game window UI element.
/// </summary>
public class GameWindow
{
    private VisualElement _windowRoot;
    private Label _titleLabel;
    private VisualElement _contentArea;
    private WindowDragManipulator _dragManipulator;

    /// <summary>
    /// Gets the root visual element of the window.
    /// </summary>
    public VisualElement Root => _windowRoot;
    /// <summary>
    /// Gets the content area where child elements can be added.
    /// </summary>
    public VisualElement ContentArea => _contentArea;
    /// <summary>
    /// Gets a value indicating whether the window is currently visible.
    /// </summary>
    public bool IsVisible => _windowRoot.resolvedStyle.display == DisplayStyle.Flex;

    /// <summary>
    /// Initializes a new instance of the <see cref="GameWindow"/> class.
    /// </summary>
    /// <param name="template">The visual tree asset template for the window.</param>
    /// <param name="parent">The parent element to attach the window to.</param>
    /// <param name="title">The title text displayed in the window's title bar.</param>
    public GameWindow(VisualTreeAsset template, VisualElement parent, string title)
    {
        var templateContainer = template.Instantiate();
        _windowRoot = templateContainer.Q<VisualElement>("game-window");

        // The <Style> element attaches the stylesheet to the TemplateContainer.
        // Copy it to the window root before reparenting so styles apply correctly.
        for (int i = 0; i < templateContainer.styleSheets.count; i++)
            _windowRoot.styleSheets.Add(templateContainer.styleSheets[i]);

        //Get References
        _titleLabel = _windowRoot.Q<Label>("title-label");
        _contentArea = _windowRoot.Q<VisualElement>("content-area");

        // Set the title
        _titleLabel.text = title;

        // Attach drag manipulator to the title bar
        var _titleBar = _windowRoot.Q<VisualElement>("title-bar");

        _dragManipulator = new WindowDragManipulator(_titleBar, _windowRoot);
        _titleBar.AddManipulator(_dragManipulator);

        // Clicking anywhere on the window brings it to front (z-order)
        _windowRoot.RegisterCallback<PointerDownEvent>(evt => _windowRoot.BringToFront());

        // Add the window root directly to the parent (not the TemplateContainer)
        parent.Add(_windowRoot);
    }

    /// <summary>
    /// Shows the window and brings it to the front.
    /// </summary>
    public void Show()
    {
        _windowRoot.style.display = DisplayStyle.Flex;
        _windowRoot.BringToFront();
    }

    /// <summary>
    /// Hides the window.
    /// </summary>
    public void Hide() => _windowRoot.style.display = DisplayStyle.None;

    /// <summary>
    /// Sets the position of the window.
    /// </summary>
    /// <param name="x">The x-coordinate position.</param>
    /// <param name="y">The y-coordinate position.</param>
    public void SetPosition(float x, float y)
    {
        _windowRoot.style.left = x;
        _windowRoot.style.top = y;
    }

    /// <summary>
    /// Gets the current position of the window.
    /// </summary>
    /// <returns>A <see cref="Vector2"/> containing the window's x and y coordinates.</returns>
    public Vector2 GetPosition()
    {
        return new Vector2(
            _windowRoot.resolvedStyle.left,
            _windowRoot.resolvedStyle.top
        );
    }

}
