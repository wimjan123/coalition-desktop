using UnityEngine;
using UnityEngine.UIElements;

namespace Coalition.Applications
{
    public class PollingApp : BaseApplication
    {
        public override string AppName => "Polling";
        public override string AppIcon => "📊";
        public override string DisplayName => "Polling Dashboard";

        protected override float GetDefaultWidth() => 800f;
        protected override float GetDefaultHeight() => 600f;

        protected override VisualElement CreateContent()
        {
            var content = new VisualElement();
            content.AddToClassList("polling-app");

            var dashboard = CreatePollingDashboard();

            return CreateStandardLayout("📊 Coalition Polling Dashboard", dashboard);
        }

        private VisualElement CreatePollingDashboard()
        {
            var container = new VisualElement();
            container.AddToClassList("polling-dashboard");

            // Approval Rating Section
            var approvalSection = CreateApprovalRatingSection();
            container.Add(approvalSection);

            // Party Standings Section
            var standingsSection = CreatePartyStandingsSection();
            container.Add(standingsSection);

            // Key Issues Section
            var issuesSection = CreateKeyIssuesSection();
            container.Add(issuesSection);

            return container;
        }

        private VisualElement CreateApprovalRatingSection()
        {
            var section = new VisualElement();
            section.AddToClassList("polling-section");

            var header = new Label("Approval Rating");
            header.AddToClassList("section-header");
            section.Add(header);

            var ratingContainer = new VisualElement();
            ratingContainer.AddToClassList("rating-container");

            // Current rating with trend
            var currentRating = new VisualElement();
            currentRating.AddToClassList("current-rating");

            var ratingValue = new Label("47%");
            ratingValue.AddToClassList("rating-value");
            currentRating.Add(ratingValue);

            var trendIndicator = new Label("↑ +2.3%");
            trendIndicator.AddToClassList("trend-indicator positive");
            currentRating.Add(trendIndicator);

            ratingContainer.Add(currentRating);

            // Rating breakdown
            var breakdown = new VisualElement();
            breakdown.AddToClassList("rating-breakdown");

            var approve = CreateRatingItem("Approve", "47%", "positive");
            var disapprove = CreateRatingItem("Disapprove", "38%", "negative");
            var undecided = CreateRatingItem("Undecided", "15%", "neutral");

            breakdown.Add(approve);
            breakdown.Add(disapprove);
            breakdown.Add(undecided);

            ratingContainer.Add(breakdown);
            section.Add(ratingContainer);

            return section;
        }

        private VisualElement CreatePartyStandingsSection()
        {
            var section = new VisualElement();
            section.AddToClassList("polling-section");

            var header = new Label("Party Standings");
            header.AddToClassList("section-header");
            section.Add(header);

            var standingsContainer = new VisualElement();
            standingsContainer.AddToClassList("standings-container");

            var parties = new[]
            {
                new PartyData { name = "Progressive Coalition", percentage = 34, seats = 68, trend = "up" },
                new PartyData { name = "Democratic Opposition", percentage = 28, seats = 56, trend = "down" },
                new PartyData { name = "Centrist Alliance", percentage = 18, seats = 36, trend = "stable" },
                new PartyData { name = "Future Movement", percentage = 12, seats = 24, trend = "up" },
                new PartyData { name = "Others", percentage = 8, seats = 16, trend = "stable" }
            };

            foreach (var party in parties)
            {
                var partyElement = CreatePartyItem(party);
                standingsContainer.Add(partyElement);
            }

            section.Add(standingsContainer);
            return section;
        }

        private VisualElement CreateKeyIssuesSection()
        {
            var section = new VisualElement();
            section.AddToClassList("polling-section");

            var header = new Label("Key Issues Support");
            header.AddToClassList("section-header");
            section.Add(header);

            var issuesContainer = new VisualElement();
            issuesContainer.AddToClassList("issues-container");

            var issues = new[]
            {
                new IssueData { name = "Healthcare Reform", support = 67, opposition = 23, undecided = 10 },
                new IssueData { name = "Climate Action", support = 72, opposition = 18, undecided = 10 },
                new IssueData { name = "Education Budget", support = 58, opposition = 31, undecided = 11 },
                new IssueData { name = "Tax Reform", support = 45, opposition = 42, undecided = 13 }
            };

            foreach (var issue in issues)
            {
                var issueElement = CreateIssueItem(issue);
                issuesContainer.Add(issueElement);
            }

            section.Add(issuesContainer);
            return section;
        }

        private VisualElement CreateRatingItem(string label, string value, string type)
        {
            var item = new VisualElement();
            item.AddToClassList("rating-item");

            var labelElement = new Label(label);
            labelElement.AddToClassList("rating-label");
            item.Add(labelElement);

            var valueElement = new Label(value);
            valueElement.AddToClassList($"rating-value {type}");
            item.Add(valueElement);

            return item;
        }

        private VisualElement CreatePartyItem(PartyData party)
        {
            var item = new VisualElement();
            item.AddToClassList("party-item");

            var nameLabel = new Label(party.name);
            nameLabel.AddToClassList("party-name");
            item.Add(nameLabel);

            var statsContainer = new VisualElement();
            statsContainer.AddToClassList("party-stats");

            var percentageLabel = new Label($"{party.percentage}%");
            percentageLabel.AddToClassList("party-percentage");
            statsContainer.Add(percentageLabel);

            var seatsLabel = new Label($"{party.seats} seats");
            seatsLabel.AddToClassList("party-seats");
            statsContainer.Add(seatsLabel);

            var trendIcon = new Label(GetTrendIcon(party.trend));
            trendIcon.AddToClassList($"trend-icon {party.trend}");
            statsContainer.Add(trendIcon);

            item.Add(statsContainer);

            return item;
        }

        private VisualElement CreateIssueItem(IssueData issue)
        {
            var item = new VisualElement();
            item.AddToClassList("issue-item");

            var nameLabel = new Label(issue.name);
            nameLabel.AddToClassList("issue-name");
            item.Add(nameLabel);

            var barContainer = new VisualElement();
            barContainer.AddToClassList("issue-bar");

            var supportBar = new VisualElement();
            supportBar.AddToClassList("support-bar");
            supportBar.style.width = Length.Percent(issue.support);
            barContainer.Add(supportBar);

            item.Add(barContainer);

            var statsLabel = new Label($"Support: {issue.support}% | Oppose: {issue.opposition}% | Undecided: {issue.undecided}%");
            statsLabel.AddToClassList("issue-stats");
            item.Add(statsLabel);

            return item;
        }

        private string GetTrendIcon(string trend)
        {
            return trend switch
            {
                "up" => "↗",
                "down" => "↘",
                "stable" => "→",
                _ => "→"
            };
        }

        [System.Serializable]
        public class PartyData
        {
            public string name;
            public int percentage;
            public int seats;
            public string trend;
        }

        [System.Serializable]
        public class IssueData
        {
            public string name;
            public int support;
            public int opposition;
            public int undecided;
        }
    }
}