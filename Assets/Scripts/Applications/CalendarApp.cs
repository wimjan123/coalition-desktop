using UnityEngine;
using UnityEngine.UIElements;

namespace Coalition.Applications
{
    public class CalendarApp : BaseApplication
    {
        public override string AppName => "Calendar";
        public override string AppIcon => "📅";
        public override string DisplayName => "Coalition Calendar";

        protected override VisualElement CreateContent()
        {
            var eventList = new VisualElement();
            eventList.AddToClassList("event-list");

            var events = new[]
            {
                new { title = "Cabinet Meeting", time = "10:00 AM", location = "Conference Room A", type = "meeting" },
                new { title = "Press Conference", time = "2:30 PM", location = "Press Room", type = "public" },
                new { title = "Budget Review", time = "4:00 PM", location = "Finance Wing", type = "meeting" }
            };

            foreach (var evt in events)
            {
                var eventItem = new VisualElement();
                eventItem.AddToClassList("event-item");

                eventItem.Add(new Label($"{evt.time} - {evt.title}"));
                eventItem.Add(new Label($"📍 {evt.location}"));

                eventList.Add(eventItem);
            }

            return CreateStandardLayout("📅 Today's Schedule", eventList);
        }
    }

    public class CabinetApp : BaseApplication
    {
        public override string AppName => "Cabinet";
        public override string AppIcon => "🏛️";
        public override string DisplayName => "Cabinet Management";

        protected override VisualElement CreateContent()
        {
            var ministerList = new VisualElement();
            ministerList.AddToClassList("minister-list");

            var ministers = new[]
            {
                new { name = "Dr. Sarah Chen", portfolio = "Defense", approval = 78, status = "stable" },
                new { name = "Marcus Rodriguez", portfolio = "Economics", approval = 65, status = "under_pressure" },
                new { name = "Emma Thompson", portfolio = "Health", approval = 82, status = "stable" }
            };

            foreach (var minister in ministers)
            {
                var ministerItem = new VisualElement();
                ministerItem.AddToClassList("minister-item");

                ministerItem.Add(new Label($"{minister.name} - {minister.portfolio}"));
                ministerItem.Add(new Label($"Approval: {minister.approval}% ({minister.status})"));

                ministerList.Add(ministerItem);
            }

            return CreateStandardLayout("🏛️ Cabinet Overview", ministerList);
        }
    }

    public class PolicyApp : BaseApplication
    {
        public override string AppName => "Policy";
        public override string AppIcon => "📋";
        public override string DisplayName => "Policy Builder";

        protected override VisualElement CreateContent()
        {
            var policyList = new VisualElement();
            policyList.AddToClassList("policy-list");

            var policies = new[]
            {
                new { title = "Healthcare Reform Act", status = "in_review", support = 67, stage = "committee" },
                new { title = "Climate Action Plan", status = "drafted", support = 82, stage = "proposal" },
                new { title = "Education Budget Increase", status = "implemented", support = 74, stage = "active" }
            };

            foreach (var policy in policies)
            {
                var policyItem = new VisualElement();
                policyItem.AddToClassList("policy-item");

                policyItem.Add(new Label(policy.title));
                policyItem.Add(new Label($"Status: {policy.status} | Support: {policy.support}%"));

                policyList.Add(policyItem);
            }

            return CreateStandardLayout("📋 Policy Pipeline", policyList);
        }
    }

    public class MediaApp : BaseApplication
    {
        public override string AppName => "Media";
        public override string AppIcon => "📺";
        public override string DisplayName => "Media Monitor";

        protected override VisualElement CreateContent()
        {
            var newsList = new VisualElement();
            newsList.AddToClassList("news-list");

            var headlines = new[]
            {
                new { outlet = "National Post", headline = "Coalition Announces New Healthcare Initiative", sentiment = "positive", time = "2 hours ago" },
                new { outlet = "Public Broadcasting", headline = "Economic Indicators Show Mixed Results", sentiment = "neutral", time = "4 hours ago" },
                new { outlet = "Independent Tribune", headline = "Opposition Questions Budget Priorities", sentiment = "negative", time = "6 hours ago" }
            };

            foreach (var news in headlines)
            {
                var newsItem = new VisualElement();
                newsItem.AddToClassList("news-item");
                newsItem.AddToClassList($"sentiment-{news.sentiment}");

                newsItem.Add(new Label($"{news.outlet} - {news.time}"));
                newsItem.Add(new Label(news.headline));

                newsList.Add(newsItem);
            }

            return CreateStandardLayout("📺 Media Coverage", newsList);
        }
    }
}