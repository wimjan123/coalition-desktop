using System.Collections.Generic;
using UnityEngine;

namespace Coalition.Data
{
    public class DataManager : MonoBehaviour
    {
        private static DataManager instance;
        public static DataManager Instance => instance;

        private Dictionary<string, object> loadedData = new Dictionary<string, object>();

        [SerializeField] private bool loadDataOnStart = true;

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
            if (loadDataOnStart)
            {
                LoadAllData();
            }
        }

        public T GetData<T>(string filename) where T : class
        {
            if (loadedData.TryGetValue(filename, out var data))
            {
                return data as T;
            }

            // Try to load if not already loaded
            LoadDataFile(filename);

            if (loadedData.TryGetValue(filename, out var newData))
            {
                return newData as T;
            }

            Debug.LogWarning($"Data file '{filename}' not found or failed to load");
            return null;
        }

        public void LoadAllData()
        {
            string[] dataFiles = {
                "briefings",
                "messages",
                "polling",
                "calendar",
                "cabinet",
                "policies",
                "media",
                "settings"
            };

            int loadedCount = 0;

            foreach (string file in dataFiles)
            {
                if (LoadDataFile(file))
                {
                    loadedCount++;
                }
            }

            Debug.Log($"Data Manager: Loaded {loadedCount}/{dataFiles.Length} data files");
        }

        private bool LoadDataFile(string filename)
        {
            try
            {
                // Try to load from Resources folder
                var textAsset = Resources.Load<TextAsset>($"Data/{filename}");

                if (textAsset != null)
                {
                    // Parse JSON based on file type
                    object parsedData = ParseDataByType(filename, textAsset.text);

                    if (parsedData != null)
                    {
                        loadedData[filename] = parsedData;
                        Debug.Log($"Loaded data file: {filename}");
                        return true;
                    }
                }
                else
                {
                    // Create default data if file doesn't exist
                    object defaultData = CreateDefaultData(filename);
                    if (defaultData != null)
                    {
                        loadedData[filename] = defaultData;
                        Debug.Log($"Created default data for: {filename}");
                        return true;
                    }
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Failed to load data file '{filename}': {e.Message}");
            }

            return false;
        }

        private object ParseDataByType(string filename, string jsonText)
        {
            return filename switch
            {
                "briefings" => JsonUtility.FromJson<Applications.MailApp.BriefingData>(jsonText),
                "messages" => JsonUtility.FromJson<ConversationData>(jsonText),
                "polling" => JsonUtility.FromJson<PollingData>(jsonText),
                "calendar" => JsonUtility.FromJson<CalendarData>(jsonText),
                "cabinet" => JsonUtility.FromJson<CabinetData>(jsonText),
                "policies" => JsonUtility.FromJson<PolicyData>(jsonText),
                "media" => JsonUtility.FromJson<MediaData>(jsonText),
                "settings" => JsonUtility.FromJson<SettingsData>(jsonText),
                _ => JsonUtility.FromJson<object>(jsonText)
            };
        }

        private object CreateDefaultData(string filename)
        {
            return filename switch
            {
                "briefings" => new Applications.MailApp.BriefingData { messages = new List<Applications.MailApp.MessageData>() },
                "messages" => new ConversationData { conversations = new List<ConversationItem>() },
                "polling" => new PollingData(),
                "calendar" => new CalendarData { events = new List<CalendarEvent>() },
                "cabinet" => new CabinetData { ministers = new List<Minister>() },
                "policies" => new PolicyData { proposals = new List<PolicyProposal>() },
                "media" => new MediaData { headlines = new List<MediaHeadline>() },
                "settings" => new SettingsData { difficulty = "normal", showNotifications = true },
                _ => null
            };
        }

        public void SaveData<T>(string filename, T data) where T : class
        {
            try
            {
                string json = JsonUtility.ToJson(data, true);

                // In a real implementation, this would save to persistent storage
                // For now, we'll just update the in-memory cache
                loadedData[filename] = data;

                Debug.Log($"Saved data file: {filename}");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Failed to save data file '{filename}': {e.Message}");
            }
        }

        public bool HasData(string filename)
        {
            return loadedData.ContainsKey(filename);
        }

        public void ReloadData(string filename)
        {
            if (loadedData.ContainsKey(filename))
            {
                loadedData.Remove(filename);
            }
            LoadDataFile(filename);
        }

        public void ClearCache()
        {
            loadedData.Clear();
            Debug.Log("Data cache cleared");
        }

        // Data structure definitions
        [System.Serializable]
        public class ConversationData
        {
            public List<ConversationItem> conversations;
        }

        [System.Serializable]
        public class ConversationItem
        {
            public string contact;
            public string lastMessage;
            public string timestamp;
            public int unread;
        }

        [System.Serializable]
        public class PollingData
        {
            public ApprovalRating approval_rating = new ApprovalRating();
            public List<PartyStanding> party_standings = new List<PartyStanding>();
        }

        [System.Serializable]
        public class ApprovalRating
        {
            public int current = 47;
            public string trend = "up";
            public string change = "+2.3";
        }

        [System.Serializable]
        public class PartyStanding
        {
            public string party;
            public int percentage;
            public int seats;
        }

        [System.Serializable]
        public class CalendarData
        {
            public List<CalendarEvent> events;
        }

        [System.Serializable]
        public class CalendarEvent
        {
            public string title;
            public string time;
            public string duration;
            public string location;
            public string type;
        }

        [System.Serializable]
        public class CabinetData
        {
            public List<Minister> ministers;
        }

        [System.Serializable]
        public class Minister
        {
            public string name;
            public string portfolio;
            public int approval;
            public string status;
        }

        [System.Serializable]
        public class PolicyData
        {
            public List<PolicyProposal> proposals;
        }

        [System.Serializable]
        public class PolicyProposal
        {
            public string title;
            public string status;
            public int support;
            public string stage;
        }

        [System.Serializable]
        public class MediaData
        {
            public List<MediaHeadline> headlines;
        }

        [System.Serializable]
        public class MediaHeadline
        {
            public string outlet;
            public string headline;
            public string sentiment;
            public string timestamp;
        }

        [System.Serializable]
        public class SettingsData
        {
            public string difficulty;
            public bool showNotifications;
            public float failureMeter;
        }
    }
}