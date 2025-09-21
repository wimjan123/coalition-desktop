using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Coalition.UI;
using Coalition.Applications;

namespace Coalition.Core
{
    public class DockController : MonoBehaviour
    {
        private static DockController instance;
        public static DockController Instance => instance;

        [SerializeField] private List<ApplicationInfo> applications = new List<ApplicationInfo>();

        private VisualElement dockPanel;
        private VisualElement appContainer;
        private VisualElement systemContainer;

        // App buttons
        private Button mailButton;
        private Button chatButton;
        private Button pollingButton;
        private Button calendarButton;
        private Button cabinetButton;
        private Button policyButton;
        private Button mediaButton;
        private Button settingsButton;

        // Minimized indicators
        private Dictionary<string, VisualElement> minimizedIndicators = new Dictionary<string, VisualElement>();

        [System.Serializable]
        public class ApplicationInfo
        {
            public string appName;
            public string displayName;
            public string icon;
            public BaseApplication applicationController;
        }

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
            InitializeDock();
        }

        private void InitializeDock()
        {
            // Get dock elements from UI
            var uiDocument = FindObjectOfType<UIDocument>();
            if (uiDocument == null)
            {
                Debug.LogError("No UIDocument found for dock initialization");
                return;
            }

            var root = uiDocument.rootVisualElement;
            dockPanel = root.Q("dock-panel");
            appContainer = root.Q("app-icons");
            systemContainer = root.Q("system-icons");

            if (dockPanel == null || appContainer == null)
            {
                Debug.LogError("Dock UI elements not found");
                return;
            }

            // Get button references
            GetButtonReferences(root);

            // Setup button callbacks
            SetupButtonCallbacks();

            // Initialize application controllers
            InitializeApplicationControllers();

            Debug.Log("Dock controller initialized successfully");
        }

        private void GetButtonReferences(VisualElement root)
        {
            mailButton = root.Q<Button>("mail-btn");
            chatButton = root.Q<Button>("chat-btn");
            pollingButton = root.Q<Button>("poll-btn");
            calendarButton = root.Q<Button>("calendar-btn");
            cabinetButton = root.Q<Button>("cabinet-btn");
            policyButton = root.Q<Button>("policy-btn");
            mediaButton = root.Q<Button>("media-btn");
            settingsButton = root.Q<Button>("settings-btn");
        }

        private void SetupButtonCallbacks()
        {
            if (mailButton != null)
                mailButton.clicked += () => OpenApplication("Mail");

            if (chatButton != null)
                chatButton.clicked += () => OpenApplication("Chat");

            if (pollingButton != null)
                pollingButton.clicked += () => OpenApplication("Polling");

            if (calendarButton != null)
                calendarButton.clicked += () => OpenApplication("Calendar");

            if (cabinetButton != null)
                cabinetButton.clicked += () => OpenApplication("Cabinet");

            if (policyButton != null)
                policyButton.clicked += () => OpenApplication("Policy");

            if (mediaButton != null)
                mediaButton.clicked += () => OpenApplication("Media");

            if (settingsButton != null)
                settingsButton.clicked += () => OpenSettings();
        }

        private void InitializeApplicationControllers()
        {
            // Initialize application controllers
            var mailApp = gameObject.AddComponent<MailApp>();
            var chatApp = gameObject.AddComponent<ChatApp>();
            var pollingApp = gameObject.AddComponent<PollingApp>();
            var calendarApp = gameObject.AddComponent<CalendarApp>();
            var cabinetApp = gameObject.AddComponent<CabinetApp>();
            var policyApp = gameObject.AddComponent<PolicyApp>();
            var mediaApp = gameObject.AddComponent<MediaApp>();

            // Add to applications list
            applications.Clear();
            applications.AddRange(new ApplicationInfo[]
            {
                new ApplicationInfo { appName = "Mail", displayName = "Mail & Briefings", icon = "📧", applicationController = mailApp },
                new ApplicationInfo { appName = "Chat", displayName = "Chat & DMs", icon = "💬", applicationController = chatApp },
                new ApplicationInfo { appName = "Polling", displayName = "Polling Dashboard", icon = "📊", applicationController = pollingApp },
                new ApplicationInfo { appName = "Calendar", displayName = "Calendar & Events", icon = "📅", applicationController = calendarApp },
                new ApplicationInfo { appName = "Cabinet", displayName = "Cabinet Management", icon = "🏛️", applicationController = cabinetApp },
                new ApplicationInfo { appName = "Policy", displayName = "Policy Builder", icon = "📋", applicationController = policyApp },
                new ApplicationInfo { appName = "Media", displayName = "Media Monitor", icon = "📺", applicationController = mediaApp }
            });
        }

        public void OpenApplication(string appName)
        {
            var appInfo = applications.Find(app => app.appName == appName);
            if (appInfo?.applicationController != null)
            {
                // Check if window is already open and minimized
                var existingWindow = WindowManager.Instance?.GetWindowByApp(appName);
                if (existingWindow != null)
                {
                    if (existingWindow.IsMinimized)
                    {
                        existingWindow.Restore();
                    }
                    else
                    {
                        WindowManager.Instance.BringToFront(existingWindow);
                    }
                }
                else
                {
                    // Open new window
                    appInfo.applicationController.OpenWindow();
                }

                // Visual feedback
                AnimateButtonPress(GetButtonForApp(appName));

                Debug.Log($"Opened application: {appName}");
            }
            else
            {
                Debug.LogWarning($"Application controller not found for: {appName}");
            }
        }

        public void OpenApp(int index)
        {
            if (index >= 0 && index < applications.Count)
            {
                OpenApplication(applications[index].appName);
            }
        }

        private void OpenSettings()
        {
            // TODO: Implement settings panel
            Debug.Log("Settings panel - Coming Soon!");

            // Visual feedback
            AnimateButtonPress(settingsButton);
        }

        private Button GetButtonForApp(string appName)
        {
            return appName switch
            {
                "Mail" => mailButton,
                "Chat" => chatButton,
                "Polling" => pollingButton,
                "Calendar" => calendarButton,
                "Cabinet" => cabinetButton,
                "Policy" => policyButton,
                "Media" => mediaButton,
                _ => null
            };
        }

        private void AnimateButtonPress(Button button)
        {
            if (button == null) return;

            // Simple press animation
            button.AddToClassList("pressed");

            // Remove class after short delay
            StartCoroutine(RemovePressedClass(button));
        }

        private System.Collections.IEnumerator RemovePressedClass(Button button)
        {
            yield return new WaitForSeconds(0.1f);
            button?.RemoveFromClassList("pressed");
        }

        public void ShowMinimizedIndicator(string appName)
        {
            var button = GetButtonForApp(appName);
            if (button != null && !minimizedIndicators.ContainsKey(appName))
            {
                var indicator = new VisualElement();
                indicator.AddToClassList("minimized-indicator");
                button.Add(indicator);
                minimizedIndicators[appName] = indicator;
            }
        }

        public void HideMinimizedIndicator(string appName)
        {
            if (minimizedIndicators.TryGetValue(appName, out var indicator))
            {
                indicator.RemoveFromHierarchy();
                minimizedIndicators.Remove(appName);
            }
        }

        public void UpdateAppStatus(string appName, bool hasNotification)
        {
            var button = GetButtonForApp(appName);
            if (button != null)
            {
                if (hasNotification)
                {
                    button.AddToClassList("has-notification");
                }
                else
                {
                    button.RemoveFromClassList("has-notification");
                }
            }
        }

        public List<string> GetOpenApplications()
        {
            var openApps = new List<string>();
            foreach (var app in applications)
            {
                var window = WindowManager.Instance?.GetWindowByApp(app.appName);
                if (window != null)
                {
                    openApps.Add(app.appName);
                }
            }
            return openApps;
        }

#if UNITY_EDITOR
        [ContextMenu("Test Open All Apps")]
        private void TestOpenAllApps()
        {
            foreach (var app in applications)
            {
                OpenApplication(app.appName);
            }
        }
#endif
    }
}