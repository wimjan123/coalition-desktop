using UnityEngine;
using UnityEngine.UIElements;
using Coalition.Data;

namespace Coalition.Applications
{
    public class MailApp : BaseApplication
    {
        public override string AppName => "Mail";
        public override string AppIcon => "📧";
        public override string DisplayName => "Coalition Briefings";

        protected override float GetDefaultWidth() => 700f;
        protected override float GetDefaultHeight() => 500f;

        protected override VisualElement CreateContent()
        {
            var content = new VisualElement();
            content.AddToClassList("mail-app");

            // Load mail data
            var mailData = DataManager.Instance?.GetData<BriefingData>("briefings");

            // Create message list
            var messageList = CreateMessageList(mailData);

            return CreateStandardLayout("📧 Coalition Briefings", messageList);
        }

        private VisualElement CreateMessageList(BriefingData data)
        {
            var container = new VisualElement();
            container.AddToClassList("message-list");

            if (data?.messages == null || data.messages.Count == 0)
            {
                // Show sample data if no data loaded
                CreateSampleMessages(container);
            }
            else
            {
                // Create messages from data
                foreach (var message in data.messages)
                {
                    var messageElement = CreateMessageItem(message);
                    container.Add(messageElement);
                }
            }

            return container;
        }

        private void CreateSampleMessages(VisualElement container)
        {
            var sampleMessages = new[]
            {
                new MessageData
                {
                    from = "Chief of Staff",
                    subject = "Daily Briefing - Coalition Stability",
                    preview = "Latest polling shows 3% uptick in approval ratings following yesterday's healthcare announcement...",
                    timestamp = "9:30 AM",
                    priority = "high"
                },
                new MessageData
                {
                    from = "Press Secretary",
                    subject = "Media Response Strategy",
                    preview = "Regarding yesterday's statement on healthcare reform. Several outlets are requesting clarification...",
                    timestamp = "8:45 AM",
                    priority = "normal"
                },
                new MessageData
                {
                    from = "Defense Minister",
                    subject = "Budget Review Meeting",
                    preview = "The quarterly defense budget review has been moved to next Wednesday at 14:00...",
                    timestamp = "Yesterday",
                    priority = "normal"
                },
                new MessageData
                {
                    from = "Economic Advisor",
                    subject = "Q3 Economic Indicators",
                    preview = "Preliminary Q3 data shows continued growth in manufacturing sector. Full report attached...",
                    timestamp = "Yesterday",
                    priority = "low"
                },
                new MessageData
                {
                    from = "International Relations",
                    subject = "EU Summit Preparation",
                    preview = "Briefing materials for next week's EU summit are ready for review. Key agenda items include...",
                    timestamp = "2 days ago",
                    priority = "high"
                }
            };

            foreach (var message in sampleMessages)
            {
                var messageElement = CreateMessageItem(message);
                container.Add(messageElement);
            }
        }

        private VisualElement CreateMessageItem(MessageData message)
        {
            var messageItem = new VisualElement();
            messageItem.AddToClassList("message-item");

            // Priority indicator
            if (message.priority == "high")
            {
                messageItem.AddToClassList("high-priority");
            }

            // Sender and timestamp row
            var headerRow = new VisualElement();
            headerRow.AddToClassList("message-header");

            var senderLabel = new Label(message.from);
            senderLabel.AddToClassList("message-sender");
            headerRow.Add(senderLabel);

            var timestampLabel = new Label(message.timestamp);
            timestampLabel.AddToClassList("message-timestamp");
            headerRow.Add(timestampLabel);

            messageItem.Add(headerRow);

            // Subject
            var subjectLabel = new Label(message.subject);
            subjectLabel.AddToClassList("message-subject");
            messageItem.Add(subjectLabel);

            // Preview
            var previewLabel = new Label(message.preview);
            previewLabel.AddToClassList("message-preview");
            messageItem.Add(previewLabel);

            // Click handler
            messageItem.RegisterCallback<ClickEvent>(evt =>
            {
                OpenMessage(message);
            });

            return messageItem;
        }

        private void OpenMessage(MessageData message)
        {
            ShowNotification("Message Opened", $"Opening: {message.subject}");
            Debug.Log($"Opened message: {message.subject} from {message.from}");
        }

        [System.Serializable]
        public class BriefingData
        {
            public System.Collections.Generic.List<MessageData> messages;
        }

        [System.Serializable]
        public class MessageData
        {
            public string from;
            public string subject;
            public string preview;
            public string timestamp;
            public string priority;
        }
    }
}