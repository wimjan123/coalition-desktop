using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UIElements;
using Coalition.UI;

namespace Coalition.Core
{
    public class WindowManager : MonoBehaviour
    {
        private static WindowManager instance;
        public static WindowManager Instance => instance;

        [SerializeField] private List<WindowElement> openWindows = new List<WindowElement>();
        [SerializeField] private int nextZIndex = 1000;

        private VisualElement windowContainer;

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            // Get window container reference
            var uiDocument = FindObjectOfType<UIDocument>();
            if (uiDocument != null)
            {
                windowContainer = uiDocument.rootVisualElement.Q("window-container");
            }

            // Load saved layout
            LoadLayout();
        }

        private void OnApplicationPause(bool pauseStatus)
        {
            if (pauseStatus)
            {
                SaveLayout();
            }
        }

        private void OnApplicationFocus(bool hasFocus)
        {
            if (!hasFocus)
            {
                SaveLayout();
            }
        }

        private void OnDestroy()
        {
            SaveLayout();
        }

        public void RegisterWindow(WindowElement window)
        {
            if (window == null || openWindows.Contains(window)) return;

            openWindows.Add(window);
            BringToFront(window);

            Debug.Log($"Registered window: {window.AppName}");
        }

        public void UnregisterWindow(WindowElement window)
        {
            if (window == null) return;

            openWindows.Remove(window);
            Debug.Log($"Unregistered window: {window.AppName}");
        }

        public void BringToFront(WindowElement window)
        {
            if (window == null) return;

            window.style.zIndex = nextZIndex++;
            window.AddToClassList("window--focused");

            // Remove focus class from other windows
            foreach (var otherWindow in openWindows)
            {
                if (otherWindow != window)
                {
                    otherWindow.RemoveFromClassList("window--focused");
                }
            }

            Debug.Log($"Brought window to front: {window.AppName}, z-index: {window.style.zIndex.value}");
        }

        public void CloseWindow(WindowElement window)
        {
            if (window == null) return;

            UnregisterWindow(window);
            window.RemoveFromHierarchy();

            Debug.Log($"Closed window: {window.AppName}");
        }

        public WindowElement GetWindowByApp(string appName)
        {
            return openWindows.Find(w => w.AppName == appName);
        }

        public bool HasOpenWindows => openWindows.Count > 0;

        public void MinimizeAllWindows()
        {
            foreach (var window in openWindows)
            {
                if (!window.IsMinimized)
                {
                    window.Minimize();
                }
            }
        }

        public void RestoreAllWindows()
        {
            foreach (var window in openWindows)
            {
                if (window.IsMinimized)
                {
                    window.Restore();
                }
            }
        }

        public void CloseAllWindows()
        {
            // Create a copy of the list to avoid modification during iteration
            var windowsToClose = new List<WindowElement>(openWindows);

            foreach (var window in windowsToClose)
            {
                window.Close();
            }

            openWindows.Clear();
        }

        public void SaveLayout()
        {
            try
            {
                var layout = new DesktopLayout();

                foreach (var window in openWindows)
                {
                    layout.windows.Add(window.GetLayoutData());
                }

                string json = JsonUtility.ToJson(layout, true);
                string path = Path.Combine(Application.persistentDataPath, "desktop_layout.json");
                File.WriteAllText(path, json);

                Debug.Log($"Saved desktop layout with {layout.windows.Count} windows to {path}");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Failed to save desktop layout: {e.Message}");
            }
        }

        public void LoadLayout()
        {
            try
            {
                string path = Path.Combine(Application.persistentDataPath, "desktop_layout.json");

                if (!File.Exists(path))
                {
                    Debug.Log("No saved desktop layout found");
                    return;
                }

                string json = File.ReadAllText(path);
                var layout = JsonUtility.FromJson<DesktopLayout>(json);

                if (layout?.windows != null)
                {
                    Debug.Log($"Loaded desktop layout with {layout.windows.Count} windows");

                    // Note: This would typically restore windows, but since we don't have
                    // applications initialized yet, we'll store this for later use
                    // The actual restoration would happen after applications are available
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Failed to load desktop layout: {e.Message}");
            }
        }

        public void CascadeWindows()
        {
            const float offsetX = 30f;
            const float offsetY = 30f;
            var startX = 50f;
            var startY = 50f;

            for (int i = 0; i < openWindows.Count; i++)
            {
                var window = openWindows[i];
                if (!window.IsMinimized)
                {
                    window.style.left = startX + (i * offsetX);
                    window.style.top = startY + (i * offsetY);
                    window.SnapToGrid();
                }
            }
        }

        public void TileWindows()
        {
            var visibleWindows = openWindows.FindAll(w => !w.IsMinimized);
            if (visibleWindows.Count == 0) return;

            var desktopBounds = DesktopManager.DesktopBounds;

            // Calculate grid layout
            int cols = Mathf.CeilToInt(Mathf.Sqrt(visibleWindows.Count));
            int rows = Mathf.CeilToInt((float)visibleWindows.Count / cols);

            float windowWidth = desktopBounds.width / cols;
            float windowHeight = desktopBounds.height / rows;

            for (int i = 0; i < visibleWindows.Count; i++)
            {
                var window = visibleWindows[i];
                int col = i % cols;
                int row = i / cols;

                window.style.left = col * windowWidth;
                window.style.top = row * windowHeight;
                window.style.width = windowWidth;
                window.style.height = windowHeight;
            }
        }

        // Performance monitoring
        private void Update()
        {
            // Clean up any null references
            openWindows.RemoveAll(w => w == null);
        }

#if UNITY_EDITOR
        [ContextMenu("Debug Window Count")]
        private void DebugWindowCount()
        {
            Debug.Log($"Open windows: {openWindows.Count}");
            foreach (var window in openWindows)
            {
                Debug.Log($"- {window.AppName}: Z-Index {window.style.zIndex.value}, Minimized: {window.IsMinimized}");
            }
        }
#endif
    }
}