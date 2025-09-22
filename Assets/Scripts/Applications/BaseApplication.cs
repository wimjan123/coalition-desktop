using UnityEngine;
using UnityEngine.UIElements;
using Coalition.UI;
using Coalition.Core;

namespace Coalition.Applications
{
    public abstract class BaseApplication : MonoBehaviour
    {
        protected WindowElement window;
        protected VisualElement contentContainer;

        public abstract string AppName { get; }
        public abstract string AppIcon { get; }
        public abstract string DisplayName { get; }

        public virtual void OpenWindow()
        {
            if (window == null)
            {
                window = CreateWindow();
                if (window != null)
                {
                    var windowContainer = FindFirstObjectByType<UIDocument>()?.rootVisualElement.Q("window-container");
                    if (windowContainer != null)
                    {
                        windowContainer.Add(window);
                        WindowManager.Instance?.RegisterWindow(window);

                        // Position new windows with slight offset
                        PositionNewWindow();

                        Debug.Log($"Opened {AppName} window");
                    }
                    else
                    {
                        Debug.LogError("Window container not found");
                    }
                }
            }
            else
            {
                // Window exists, bring to front or restore if minimized
                if (window.IsMinimized)
                {
                    window.Restore();
                }
                else
                {
                    WindowManager.Instance?.BringToFront(window);
                }
            }
        }

        protected virtual WindowElement CreateWindow()
        {
            var windowElement = new WindowElement(AppName, DisplayName);

            // Set default window properties
            windowElement.style.width = GetDefaultWidth();
            windowElement.style.height = GetDefaultHeight();

            // Create and add content
            contentContainer = CreateContent();
            if (contentContainer != null)
            {
                windowElement.SetContent(contentContainer);
            }

            return windowElement;
        }

        protected abstract VisualElement CreateContent();

        protected virtual float GetDefaultWidth() => 600f;
        protected virtual float GetDefaultHeight() => 400f;

        private void PositionNewWindow()
        {
            if (window == null) return;

            // Get existing windows count for offset
            var openWindows = WindowManager.Instance?.GetOpenWindowsCount() ?? 0;
            var offset = openWindows * 30f;

            var desktopBounds = DesktopManager.DesktopBounds;
            var startX = Mathf.Min(100f + offset, desktopBounds.width - window.resolvedStyle.width);
            var startY = Mathf.Min(100f + offset, desktopBounds.height - window.resolvedStyle.height);

            window.style.left = startX;
            window.style.top = startY;
        }

        protected VisualElement CreateStandardLayout(string title, VisualElement content)
        {
            var container = new VisualElement();
            container.AddToClassList("app-container");

            // Add header
            var header = new VisualElement();
            header.AddToClassList("app-header");

            var titleLabel = new Label(title);
            titleLabel.AddToClassList("app-title");
            header.Add(titleLabel);

            container.Add(header);

            // Add content with scrolling
            var scrollView = new ScrollView();
            scrollView.AddToClassList("app-content");
            scrollView.Add(content);

            container.Add(scrollView);

            return container;
        }

        protected void ShowNotification(string title, string message)
        {
            NotificationSystem.Instance?.ShowNotification(title, message);
        }

        protected void UpdateDockNotification(bool hasNotification)
        {
            DockController.Instance?.UpdateAppStatus(AppName, hasNotification);
        }

        public virtual void OnWindowClosed()
        {
            window = null;
            contentContainer = null;
        }

        protected virtual void OnDestroy()
        {
            if (window != null)
            {
                WindowManager.Instance?.UnregisterWindow(window);
            }
        }
    }

    // Extension to WindowManager for getting window count
    public static class WindowManagerExtensions
    {
        public static int GetOpenWindowsCount(this WindowManager manager)
        {
            if (manager?.HasOpenWindows != true) return 0;

            // Use reflection to access private openWindows field
            var field = manager.GetType().GetField("openWindows",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            if (field?.GetValue(manager) is System.Collections.Generic.List<WindowElement> windows)
            {
                return windows.Count;
            }

            return 0;
        }
    }
}