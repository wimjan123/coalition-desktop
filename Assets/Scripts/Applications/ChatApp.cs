using UnityEngine;
using UnityEngine.UIElements;

namespace Coalition.Applications
{
    public class ChatApp : BaseApplication
    {
        public override string AppName => "Chat";
        public override string AppIcon => "💬";
        public override string DisplayName => "Coalition Chat";

        protected override float GetDefaultWidth() => 500f;
        protected override float GetDefaultHeight() => 600f;

        protected override VisualElement CreateContent()
        {
            var content = new VisualElement();
            content.AddToClassList("chat-app");

            var conversationList = CreateConversationList();

            return CreateStandardLayout("💬 Coalition Chat", conversationList);
        }

        private VisualElement CreateConversationList()
        {
            var container = new VisualElement();
            container.AddToClassList("conversation-list");

            var sampleConversations = new[]
            {
                new ConversationData
                {
                    contact = "Defense Minister",
                    lastMessage = "Budget meeting moved to 3 PM",
                    timestamp = "10:15 AM",
                    unread = 2,
                    status = "online"
                },
                new ConversationData
                {
                    contact = "Economic Advisor",
                    lastMessage = "Inflation report ready for review",
                    timestamp = "9:50 AM",
                    unread = 0,
                    status = "busy"
                },
                new ConversationData
                {
                    contact = "Press Secretary",
                    lastMessage = "Media briefing at 2 PM today",
                    timestamp = "9:30 AM",
                    unread = 1,
                    status = "online"
                },
                new ConversationData
                {
                    contact = "Health Minister",
                    lastMessage = "Thanks for the update",
                    timestamp = "Yesterday",
                    unread = 0,
                    status = "offline"
                },
                new ConversationData
                {
                    contact = "Campaign Team",
                    lastMessage = "Great work everyone! 👏",
                    timestamp = "Yesterday",
                    unread = 3,
                    status = "group"
                }
            };

            foreach (var conversation in sampleConversations)
            {
                var conversationElement = CreateConversationItem(conversation);
                container.Add(conversationElement);
            }

            return container;
        }

        private VisualElement CreateConversationItem(ConversationData conversation)
        {
            var conversationItem = new VisualElement();
            conversationItem.AddToClassList("conversation-item");

            if (conversation.unread > 0)
            {
                conversationItem.AddToClassList("has-unread");
            }

            // Header with contact name and status
            var headerRow = new VisualElement();
            headerRow.AddToClassList("conversation-header");

            var contactLabel = new Label(conversation.contact);
            contactLabel.AddToClassList("contact-name");
            headerRow.Add(contactLabel);

            var statusIndicator = new VisualElement();
            statusIndicator.AddToClassList("status-indicator");
            statusIndicator.AddToClassList($"status-{conversation.status}");
            headerRow.Add(statusIndicator);

            conversationItem.Add(headerRow);

            // Last message and timestamp
            var messageRow = new VisualElement();
            messageRow.AddToClassList("message-row");

            var lastMessageLabel = new Label(conversation.lastMessage);
            lastMessageLabel.AddToClassList("last-message");
            messageRow.Add(lastMessageLabel);

            var timestampLabel = new Label(conversation.timestamp);
            timestampLabel.AddToClassList("timestamp");
            messageRow.Add(timestampLabel);

            conversationItem.Add(messageRow);

            // Unread indicator
            if (conversation.unread > 0)
            {
                var unreadBadge = new Label(conversation.unread.ToString());
                unreadBadge.AddToClassList("unread-badge");
                conversationItem.Add(unreadBadge);
            }

            // Click handler
            conversationItem.RegisterCallback<ClickEvent>(evt =>
            {
                OpenConversation(conversation);
            });

            return conversationItem;
        }

        private void OpenConversation(ConversationData conversation)
        {
            ShowNotification("Chat Opened", $"Opening chat with {conversation.contact}");
            Debug.Log($"Opened conversation with: {conversation.contact}");

            // Mark as read
            UpdateDockNotification(false);
        }

        [System.Serializable]
        public class ConversationData
        {
            public string contact;
            public string lastMessage;
            public string timestamp;
            public int unread;
            public string status; // online, offline, busy, group
        }
    }
}