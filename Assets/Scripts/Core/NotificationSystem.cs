using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace Coalition.Core
{
    public class NotificationSystem : MonoBehaviour
    {
        private static NotificationSystem instance;
        public static NotificationSystem Instance => instance;

        [SerializeField] private float defaultDuration = 3f;
        [SerializeField] private int maxNotifications = 5;

        private VisualElement notificationContainer;
        private Queue<VisualElement> notificationPool = new Queue<VisualElement>();
        private List<VisualElement> activeNotifications = new List<VisualElement>();

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
            InitializeNotificationSystem();
        }

        private void InitializeNotificationSystem()
        {
            var uiDocument = FindFirstObjectByType<UIDocument>();
            if (uiDocument != null)
            {
                notificationContainer = uiDocument.rootVisualElement.Q("notification-overlay");
                if (notificationContainer == null)
                {
                    Debug.LogError("Notification overlay not found in UI");
                }
                else
                {
                    Debug.Log("Notification system initialized");
                }
            }
        }

        public void ShowNotification(string title, string message, float duration = -1f)
        {
            if (notificationContainer == null) return;

            if (duration < 0) duration = defaultDuration;

            var notification = CreateNotification(title, message);
            ShowNotificationElement(notification, duration);
        }

        public void ShowNotification(string title, string message, NotificationType type, float duration = -1f)
        {
            if (notificationContainer == null) return;

            if (duration < 0) duration = defaultDuration;

            var notification = CreateNotification(title, message, type);
            ShowNotificationElement(notification, duration);
        }

        private VisualElement CreateNotification(string title, string message, NotificationType type = NotificationType.Info)
        {
            var notification = GetPooledNotification();

            notification.Clear();
            notification.RemoveFromClassList("notification-success");
            notification.RemoveFromClassList("notification-warning");
            notification.RemoveFromClassList("notification-error");
            notification.RemoveFromClassList("notification-info");

            notification.AddToClassList("notification-toast");
            notification.AddToClassList($"notification-{type.ToString().ToLower()}");

            // Icon based on type
            var icon = new Label(GetNotificationIcon(type));
            icon.AddToClassList("notification-icon");
            notification.Add(icon);

            // Content container
            var content = new VisualElement();
            content.AddToClassList("notification-content");

            // Title
            if (!string.IsNullOrEmpty(title))
            {
                var titleLabel = new Label(title);
                titleLabel.AddToClassList("notification-title");
                content.Add(titleLabel);
            }

            // Message
            var messageLabel = new Label(message);
            messageLabel.AddToClassList("notification-message");
            content.Add(messageLabel);

            notification.Add(content);

            // Close button
            var closeButton = new Button(() => HideNotification(notification));
            closeButton.text = "×";
            closeButton.AddToClassList("notification-close");
            notification.Add(closeButton);

            return notification;
        }

        private void ShowNotificationElement(VisualElement notification, float duration)
        {
            // Remove oldest notification if at max limit
            if (activeNotifications.Count >= maxNotifications && activeNotifications.Count > 0)
            {
                HideNotification(activeNotifications[0]);
            }

            notificationContainer.Add(notification);
            activeNotifications.Add(notification);

            // Add animation class
            notification.AddToClassList("notification-show");

            // Auto-hide after duration
            StartCoroutine(HideNotificationAfterDelay(notification, duration));

            Debug.Log($"Showed notification: {notification.Q<Label>("notification-title")?.text}");
        }

        private IEnumerator HideNotificationAfterDelay(VisualElement notification, float delay)
        {
            yield return new WaitForSeconds(delay);

            if (notification.parent != null)
            {
                HideNotification(notification);
            }
        }

        public void HideNotification(VisualElement notification)
        {
            if (notification == null || notification.parent == null) return;

            notification.AddToClassList("notification-hide");
            activeNotifications.Remove(notification);

            // Remove from UI after animation
            StartCoroutine(RemoveNotificationAfterAnimation(notification));
        }

        private IEnumerator RemoveNotificationAfterAnimation(VisualElement notification)
        {
            yield return new WaitForSeconds(0.3f); // Animation duration

            if (notification.parent != null)
            {
                notification.RemoveFromHierarchy();
                ReturnToPool(notification);
            }
        }

        private VisualElement GetPooledNotification()
        {
            if (notificationPool.Count > 0)
            {
                return notificationPool.Dequeue();
            }

            // Create new notification element
            var notification = new VisualElement();
            return notification;
        }

        private void ReturnToPool(VisualElement notification)
        {
            notification.RemoveFromClassList("notification-show");
            notification.RemoveFromClassList("notification-hide");
            notificationPool.Enqueue(notification);
        }

        private string GetNotificationIcon(NotificationType type)
        {
            return type switch
            {
                NotificationType.Success => "✅",
                NotificationType.Warning => "⚠️",
                NotificationType.Error => "❌",
                NotificationType.Info => "ℹ️",
                _ => "ℹ️"
            };
        }

        public void ClearAllNotifications()
        {
            var notificationsToHide = new List<VisualElement>(activeNotifications);
            foreach (var notification in notificationsToHide)
            {
                HideNotification(notification);
            }
        }

        // Predefined notification methods
        public void ShowSuccess(string message, float duration = -1f)
        {
            ShowNotification("Success", message, NotificationType.Success, duration);
        }

        public void ShowWarning(string message, float duration = -1f)
        {
            ShowNotification("Warning", message, NotificationType.Warning, duration);
        }

        public void ShowError(string message, float duration = -1f)
        {
            ShowNotification("Error", message, NotificationType.Error, duration);
        }

        public void ShowInfo(string message, float duration = -1f)
        {
            ShowNotification("Info", message, NotificationType.Info, duration);
        }

        // Sample notifications for testing
        public void ShowSampleNotifications()
        {
            ShowInfo("Welcome to Coalition Desktop");
            ShowSuccess("System initialized successfully");
            ShowWarning("Budget deadline approaching");
            ShowError("Connection to polling service failed");
        }

        public enum NotificationType
        {
            Info,
            Success,
            Warning,
            Error
        }
    }
}