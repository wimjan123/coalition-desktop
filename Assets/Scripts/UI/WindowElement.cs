using UnityEngine;
using UnityEngine.UIElements;

namespace Coalition.UI
{
    public class WindowElement : VisualElement
    {
        public new class UxmlFactory : UxmlFactory<WindowElement, UxmlTraits> { }

        public string AppName { get; set; }
        public bool IsMinimized { get; private set; }
        public bool IsMaximized { get; private set; }

        private VisualElement titleBar;
        private VisualElement windowContent;
        private Label windowTitle;
        private Button closeButton;
        private Button minimizeButton;
        private Button maximizeButton;

        private DragManipulator dragManipulator;
        private ResizeManipulator resizeManipulator;

        private Vector2 originalSize;
        private Vector2 originalPosition;

        public WindowElement()
        {
            SetupWindow();
        }

        public WindowElement(string appName, string title) : this()
        {
            AppName = appName;
            SetTitle(title);
        }

        private void SetupWindow()
        {
            // Load base window structure
            var visualTree = Resources.Load<VisualTreeAsset>("UI/Windows/BaseWindow");
            if (visualTree != null)
            {
                visualTree.CloneTree(this);
            }

            // Load styles
            var styleSheet = Resources.Load<StyleSheet>("UI/Windows/BaseWindow");
            if (styleSheet != null)
            {
                styleSheets.Add(styleSheet);
            }

            // Get references to UI elements
            titleBar = this.Q("title-bar");
            windowContent = this.Q("window-content");
            windowTitle = this.Q<Label>("window-title");
            closeButton = this.Q<Button>("close-btn");
            minimizeButton = this.Q<Button>("minimize-btn");
            maximizeButton = this.Q<Button>("maximize-btn");

            // Setup manipulators
            if (titleBar != null)
            {
                dragManipulator = new DragManipulator(this);
                titleBar.AddManipulator(dragManipulator);
            }

            resizeManipulator = new ResizeManipulator(this);
            this.AddManipulator(resizeManipulator);

            // Setup button callbacks
            SetupButtons();

            // Default positioning and sizing
            style.position = Position.Absolute;
            style.left = 100;
            style.top = 100;
            style.width = 600;
            style.height = 400;

            // Register for focus events
            RegisterCallback<PointerDownEvent>(OnPointerDown);

            focusable = true;
            pickingMode = PickingMode.Position;
        }

        private void SetupButtons()
        {
            if (closeButton != null)
            {
                closeButton.clicked += Close;
            }

            if (minimizeButton != null)
            {
                minimizeButton.clicked += Minimize;
            }

            if (maximizeButton != null)
            {
                maximizeButton.clicked += ToggleMaximize;
            }
        }

        private void OnPointerDown(PointerDownEvent evt)
        {
            Focus();
            WindowManager.Instance?.BringToFront(this);
        }

        public void SetTitle(string title)
        {
            if (windowTitle != null)
            {
                windowTitle.text = title;
            }
        }

        public void SetContent(VisualElement content)
        {
            if (windowContent != null)
            {
                windowContent.Clear();
                windowContent.Add(content);
            }
        }

        public void Close()
        {
            WindowManager.Instance?.CloseWindow(this);
            RemoveFromHierarchy();
        }

        public void Minimize()
        {
            if (IsMinimized) return;

            IsMinimized = true;
            style.display = DisplayStyle.None;

            // Notify dock controller
            var dockController = Object.FindObjectOfType<Coalition.Core.DockController>();
            dockController?.ShowMinimizedIndicator(AppName);
        }

        public void Restore()
        {
            if (!IsMinimized) return;

            IsMinimized = false;
            style.display = DisplayStyle.Flex;

            WindowManager.Instance?.BringToFront(this);

            // Notify dock controller
            var dockController = Object.FindObjectOfType<Coalition.Core.DockController>();
            dockController?.HideMinimizedIndicator(AppName);
        }

        public void ToggleMaximize()
        {
            if (IsMaximized)
            {
                RestoreSize();
            }
            else
            {
                MaximizeWindow();
            }
        }

        private void MaximizeWindow()
        {
            if (IsMaximized) return;

            // Store original size and position
            originalSize = new Vector2(resolvedStyle.width, resolvedStyle.height);
            originalPosition = new Vector2(resolvedStyle.left, resolvedStyle.top);

            // Get desktop bounds
            var desktopBounds = Coalition.Core.DesktopManager.DesktopBounds;

            // Set to full desktop size minus dock
            style.left = 0;
            style.top = 0;
            style.width = desktopBounds.width;
            style.height = desktopBounds.height;

            IsMaximized = true;
            AddToClassList("window--maximized");
        }

        private void RestoreSize()
        {
            if (!IsMaximized) return;

            // Restore original size and position
            style.left = originalPosition.x;
            style.top = originalPosition.y;
            style.width = originalSize.x;
            style.height = originalSize.y;

            IsMaximized = false;
            RemoveFromClassList("window--maximized");
        }

        public void SnapToGrid()
        {
            var snappedPosition = SnapToGridSystem.SnapToGrid(new Vector2(resolvedStyle.left, resolvedStyle.top));
            style.left = snappedPosition.x;
            style.top = snappedPosition.y;
        }

        public void SnapToHalf(bool isLeft)
        {
            var desktopBounds = Coalition.Core.DesktopManager.DesktopBounds;
            var halfPosition = SnapToGridSystem.SnapToHalf(new Vector2(desktopBounds.width, desktopBounds.height));

            style.left = isLeft ? 0 : halfPosition.x;
            style.top = 0;
            style.width = halfPosition.x;
            style.height = desktopBounds.height;
        }

        public void SnapToQuarter(SnapToGridSystem.QuadrantPosition quadrant)
        {
            var desktopBounds = Coalition.Core.DesktopManager.DesktopBounds;
            var quarterPosition = SnapToGridSystem.SnapToQuarter(new Vector2(desktopBounds.width, desktopBounds.height), quadrant);

            style.left = quarterPosition.x;
            style.top = quarterPosition.y;
            style.width = desktopBounds.width / 2f;
            style.height = desktopBounds.height / 2f;
        }

        public WindowLayoutData GetLayoutData()
        {
            return new WindowLayoutData
            {
                appName = AppName,
                x = resolvedStyle.left,
                y = resolvedStyle.top,
                width = resolvedStyle.width,
                height = resolvedStyle.height,
                isMinimized = IsMinimized,
                zIndex = (int)resolvedStyle.zIndex
            };
        }

        public void ApplyLayoutData(WindowLayoutData data)
        {
            style.left = data.x;
            style.top = data.y;
            style.width = data.width;
            style.height = data.height;
            style.zIndex = data.zIndex;

            if (data.isMinimized)
            {
                Minimize();
            }
        }
    }

    [System.Serializable]
    public class WindowLayoutData
    {
        public string appName;
        public float x, y, width, height;
        public bool isMinimized;
        public int zIndex;
    }

    [System.Serializable]
    public class DesktopLayout
    {
        public System.Collections.Generic.List<WindowLayoutData> windows = new System.Collections.Generic.List<WindowLayoutData>();
    }
}