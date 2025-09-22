using UnityEngine;
using UnityEngine.UIElements;
using Coalition.UI;
using Coalition.Utils;
using Coalition.Data;

namespace Coalition.Core
{
    /// <summary>
    /// Main controller for the Coalition Desktop Shell
    /// Coordinates all systems and handles initialization
    /// </summary>
    public class CoalitionDesktop : MonoBehaviour
    {
        [Header("System Components")]
        [SerializeField] private UIDocument uiDocument;
        [SerializeField] private bool initializeOnStart = true;
        [SerializeField] private bool showWelcomeNotifications = true;

        [Header("Performance")]
        [SerializeField] private bool enableFPSCounter = true;
        [SerializeField] private bool logPerformanceWarnings = true;

        // System references
        private DesktopManager desktopManager;
        private WindowManager windowManager;
        private DockController dockController;
        private NotificationSystem notificationSystem;
        private DataManager dataManager;
        private FPSCounter fpsCounter;

        // Initialization state
        private bool isInitialized = false;

        private void Awake()
        {
            // Ensure we have required components
            EnsureComponents();
        }

        private void Start()
        {
            if (initializeOnStart)
            {
                InitializeDesktop();
            }
        }

        private void EnsureComponents()
        {
            // Get or create UIDocument
            if (uiDocument == null)
            {
                uiDocument = GetComponent<UIDocument>();
                if (uiDocument == null)
                {
                    uiDocument = gameObject.AddComponent<UIDocument>();
                }
            }

            // Load the main desktop UI if not set
            if (uiDocument.visualTreeAsset == null)
            {
                var desktopTree = Resources.Load<VisualTreeAsset>("UI/Desktop/Desktop");
                if (desktopTree != null)
                {
                    uiDocument.visualTreeAsset = desktopTree;
                }
            }

            // Ensure core managers exist
            EnsureManager<DesktopManager>(ref desktopManager);
            EnsureManager<WindowManager>(ref windowManager);
            EnsureManager<DockController>(ref dockController);
            EnsureManager<NotificationSystem>(ref notificationSystem);
            EnsureManager<DataManager>(ref dataManager);

            if (enableFPSCounter)
            {
                EnsureManager<FPSCounter>(ref fpsCounter);
            }
        }

        private void EnsureManager<T>(ref T manager) where T : MonoBehaviour
        {
            if (manager == null)
            {
                manager = FindFirstObjectByType<T>();
                if (manager == null)
                {
                    var managerObj = new GameObject(typeof(T).Name);
                    manager = managerObj.AddComponent<T>();
                }
            }
        }

        public void InitializeDesktop()
        {
            if (isInitialized)
            {
                Debug.LogWarning("Desktop already initialized");
                return;
            }

            Debug.Log("Initializing Coalition Desktop Shell...");

            // Initialize in correct order
            InitializeDataSystem();
            InitializeDesktopSystem();
            InitializeWindowSystem();
            InitializeDockSystem();
            InitializeNotificationSystem();
            InitializePerformanceMonitoring();

            // Setup input handling
            SetupInputHandling();

            // Show welcome notifications
            if (showWelcomeNotifications)
            {
                ShowWelcomeMessages();
            }

            isInitialized = true;
            Debug.Log("Coalition Desktop Shell initialized successfully");
        }

        private void InitializeDataSystem()
        {
            if (dataManager != null)
            {
                // Data manager will load its data on Start()
                Debug.Log("Data system initialized");
            }
        }

        private void InitializeDesktopSystem()
        {
            if (desktopManager != null)
            {
                // Desktop manager handles its own initialization
                Debug.Log("Desktop system initialized");
            }
        }

        private void InitializeWindowSystem()
        {
            if (windowManager != null)
            {
                Debug.Log("Window system initialized");
            }
        }

        private void InitializeDockSystem()
        {
            if (dockController != null)
            {
                Debug.Log("Dock system initialized");
            }
        }

        private void InitializeNotificationSystem()
        {
            if (notificationSystem != null)
            {
                Debug.Log("Notification system initialized");
            }
        }

        private void InitializePerformanceMonitoring()
        {
            if (fpsCounter != null && logPerformanceWarnings)
            {
                fpsCounter.OnPerformanceDropped += OnPerformanceDrop;
                Debug.Log("Performance monitoring initialized");
            }
        }

        private void SetupInputHandling()
        {
            // Global keyboard shortcuts
            var root = uiDocument?.rootVisualElement;
            if (root != null)
            {
                root.RegisterCallback<KeyDownEvent>(OnGlobalKeyDown);
            }
        }

        private void OnGlobalKeyDown(KeyDownEvent evt)
        {
            // Handle global shortcuts
            if (evt.ctrlKey || evt.commandKey)
            {
                switch (evt.keyCode)
                {
                    case KeyCode.Tab:
                        // Cycle through open windows
                        CycleWindows();
                        evt.StopPropagation();
                        break;

                    case KeyCode.M:
                        // Minimize active window
                        MinimizeActiveWindow();
                        evt.StopPropagation();
                        break;

                    case KeyCode.W:
                        // Close active window
                        CloseActiveWindow();
                        evt.StopPropagation();
                        break;
                }
            }

            // Other shortcuts
            switch (evt.keyCode)
            {
                case KeyCode.Escape:
                    // Clear all notifications or cancel operations
                    notificationSystem?.ClearAllNotifications();
                    break;
            }
        }

        private void ShowWelcomeMessages()
        {
            if (notificationSystem == null) return;

            // Stagger welcome notifications
            Invoke(nameof(ShowWelcomeMessage1), 0.5f);
            Invoke(nameof(ShowWelcomeMessage2), 2.0f);
            Invoke(nameof(ShowWelcomeMessage3), 3.5f);
        }

        private void ShowWelcomeMessage1()
        {
            notificationSystem?.ShowInfo("Welcome to Coalition Desktop Shell", 4f);
        }

        private void ShowWelcomeMessage2()
        {
            notificationSystem?.ShowInfo("Press F3 to toggle FPS counter", 3f);
        }

        private void ShowWelcomeMessage3()
        {
            notificationSystem?.ShowSuccess("All systems operational", 3f);
        }

        private void OnPerformanceDrop()
        {
            if (logPerformanceWarnings)
            {
                Debug.LogWarning("Performance drop detected in Coalition Desktop");
                notificationSystem?.ShowWarning("Performance warning: FPS below 30", 2f);
            }
        }

        private void CycleWindows()
        {
            // TODO: Implement window cycling
            Debug.Log("Window cycling - Coming Soon!");
        }

        private void MinimizeActiveWindow()
        {
            // TODO: Get active window and minimize it
            Debug.Log("Minimize active window - Coming Soon!");
        }

        private void CloseActiveWindow()
        {
            // TODO: Get active window and close it
            Debug.Log("Close active window - Coming Soon!");
        }

        // Public API for external control
        public void OpenApplication(string appName)
        {
            dockController?.OpenApplication(appName);
        }

        public void ShowNotification(string title, string message)
        {
            notificationSystem?.ShowNotification(title, message);
        }

        public bool IsSystemReady => isInitialized;

        public DesktopManager Desktop => desktopManager;
        public WindowManager Windows => windowManager;
        public DockController Dock => dockController;
        public NotificationSystem Notifications => notificationSystem;
        public DataManager Data => dataManager;

        // Cleanup
        private void OnDestroy()
        {
            if (fpsCounter != null)
            {
                fpsCounter.OnPerformanceDropped -= OnPerformanceDrop;
            }

            var root = uiDocument?.rootVisualElement;
            if (root != null)
            {
                root.UnregisterCallback<KeyDownEvent>(OnGlobalKeyDown);
            }
        }

        private void OnApplicationPause(bool pauseStatus)
        {
            if (pauseStatus && windowManager != null)
            {
                windowManager.SaveLayout();
            }
        }

        private void OnApplicationFocus(bool hasFocus)
        {
            if (!hasFocus && windowManager != null)
            {
                windowManager.SaveLayout();
            }
        }

#if UNITY_EDITOR
        [ContextMenu("Force Initialize")]
        private void ForceInitialize()
        {
            isInitialized = false;
            InitializeDesktop();
        }

        [ContextMenu("Show System Status")]
        private void ShowSystemStatus()
        {
            Debug.Log($"Coalition Desktop Status:");
            Debug.Log($"- Initialized: {isInitialized}");
            Debug.Log($"- Desktop Manager: {(desktopManager != null ? "✓" : "✗")}");
            Debug.Log($"- Window Manager: {(windowManager != null ? "✓" : "✗")}");
            Debug.Log($"- Dock Controller: {(dockController != null ? "✓" : "✗")}");
            Debug.Log($"- Notification System: {(notificationSystem != null ? "✓" : "✗")}");
            Debug.Log($"- Data Manager: {(dataManager != null ? "✓" : "✗")}");
            Debug.Log($"- FPS Counter: {(fpsCounter != null ? "✓" : "✗")}");

            if (fpsCounter != null)
            {
                Debug.Log($"- Performance: {fpsCounter.GetPerformanceReport()}");
            }
        }

        [ContextMenu("Test All Applications")]
        private void TestAllApplications()
        {
            if (dockController != null)
            {
                string[] apps = { "Mail", "Chat", "Polling", "Calendar", "Cabinet", "Policy", "Media" };
                foreach (string app in apps)
                {
                    dockController.OpenApplication(app);
                }
            }
        }
#endif
    }
}