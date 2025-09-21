using UnityEngine;
using UnityEngine.UIElements;

namespace Coalition.Utils
{
    public class FPSCounter : MonoBehaviour
    {
        [SerializeField] private KeyCode toggleKey = KeyCode.F3;
        [SerializeField] private bool showOnStart = false;

        private Label fpsLabel;
        private bool isVisible = false;
        private float deltaTime = 0.0f;
        private float updateInterval = 0.1f;
        private float lastUpdateTime = 0f;

        // Performance tracking
        private float minFPS = float.MaxValue;
        private float maxFPS = 0f;
        private float avgFPS = 0f;
        private int frameCount = 0;
        private float totalTime = 0f;

        private void Start()
        {
            CreateFPSDisplay();
            isVisible = showOnStart;
            UpdateVisibility();
        }

        private void CreateFPSDisplay()
        {
            var uiDocument = FindObjectOfType<UIDocument>();
            if (uiDocument == null) return;

            var root = uiDocument.rootVisualElement;

            fpsLabel = new Label("FPS: 60");
            fpsLabel.name = "fps-counter";
            fpsLabel.AddToClassList("fps-counter");

            // Styling
            fpsLabel.style.position = Position.Absolute;
            fpsLabel.style.top = 10;
            fpsLabel.style.right = 10;
            fpsLabel.style.color = Color.white;
            fpsLabel.style.backgroundColor = new Color(0, 0, 0, 0.7f);
            fpsLabel.style.borderTopWidth = 1;
            fpsLabel.style.borderBottomWidth = 1;
            fpsLabel.style.borderLeftWidth = 1;
            fpsLabel.style.borderRightWidth = 1;
            fpsLabel.style.borderTopColor = Color.gray;
            fpsLabel.style.borderBottomColor = Color.gray;
            fpsLabel.style.borderLeftColor = Color.gray;
            fpsLabel.style.borderRightColor = Color.gray;
            fpsLabel.style.borderTopLeftRadius = 4;
            fpsLabel.style.borderTopRightRadius = 4;
            fpsLabel.style.borderBottomLeftRadius = 4;
            fpsLabel.style.borderBottomRightRadius = 4;
            fpsLabel.style.paddingTop = 4;
            fpsLabel.style.paddingBottom = 4;
            fpsLabel.style.paddingLeft = 8;
            fpsLabel.style.paddingRight = 8;
            fpsLabel.style.fontSize = 12;
            fpsLabel.style.unityFontStyleAndWeight = FontStyle.Bold;

            root.Add(fpsLabel);
        }

        private void Update()
        {
            // Toggle visibility
            if (Input.GetKeyDown(toggleKey))
            {
                ToggleFPSCounter();
            }

            if (isVisible && fpsLabel != null)
            {
                UpdateFPSCalculation();

                // Update display at intervals
                if (Time.time - lastUpdateTime >= updateInterval)
                {
                    UpdateFPSDisplay();
                    lastUpdateTime = Time.time;
                }
            }
        }

        private void UpdateFPSCalculation()
        {
            deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;

            // Track performance stats
            float currentFPS = 1.0f / Time.unscaledDeltaTime;

            if (currentFPS < minFPS) minFPS = currentFPS;
            if (currentFPS > maxFPS) maxFPS = currentFPS;

            frameCount++;
            totalTime += Time.unscaledDeltaTime;
            avgFPS = frameCount / totalTime;
        }

        private void UpdateFPSDisplay()
        {
            if (fpsLabel == null) return;

            float fps = 1.0f / deltaTime;
            string fpsText = $"FPS: {Mathf.Ceil(fps):F0}";

            // Add detailed info on multiple lines
            fpsText += $"\nMin: {minFPS:F0} | Max: {maxFPS:F0}";
            fpsText += $"\nAvg: {avgFPS:F0}";

            // Add frame time
            float frameTime = deltaTime * 1000f;
            fpsText += $"\nFrame: {frameTime:F1}ms";

            fpsLabel.text = fpsText;

            // Color code based on performance
            if (fps >= 55f)
            {
                fpsLabel.style.color = Color.green;
            }
            else if (fps >= 30f)
            {
                fpsLabel.style.color = Color.yellow;
            }
            else
            {
                fpsLabel.style.color = Color.red;
            }
        }

        public void ToggleFPSCounter()
        {
            isVisible = !isVisible;
            UpdateVisibility();

            if (isVisible)
            {
                ResetStats();
            }
        }

        public void ShowFPSCounter()
        {
            isVisible = true;
            UpdateVisibility();
            ResetStats();
        }

        public void HideFPSCounter()
        {
            isVisible = false;
            UpdateVisibility();
        }

        private void UpdateVisibility()
        {
            if (fpsLabel != null)
            {
                fpsLabel.style.display = isVisible ? DisplayStyle.Flex : DisplayStyle.None;
            }
        }

        private void ResetStats()
        {
            minFPS = float.MaxValue;
            maxFPS = 0f;
            frameCount = 0;
            totalTime = 0f;
            avgFPS = 0f;
        }

        public float GetCurrentFPS()
        {
            return 1.0f / deltaTime;
        }

        public bool IsPerformanceGood()
        {
            return GetCurrentFPS() >= 55f;
        }

        public string GetPerformanceReport()
        {
            return $"FPS - Current: {GetCurrentFPS():F1}, Min: {minFPS:F1}, Max: {maxFPS:F1}, Avg: {avgFPS:F1}";
        }

        // Performance monitoring callbacks
        public System.Action<float> OnFPSChanged;
        public System.Action OnPerformanceDropped;

        private float lastReportedFPS = 0f;
        private bool hasReportedDrop = false;

        private void LateUpdate()
        {
            if (!isVisible) return;

            float currentFPS = GetCurrentFPS();

            // Report FPS changes
            if (Mathf.Abs(currentFPS - lastReportedFPS) > 5f)
            {
                OnFPSChanged?.Invoke(currentFPS);
                lastReportedFPS = currentFPS;
            }

            // Report performance drops
            if (currentFPS < 30f && !hasReportedDrop)
            {
                OnPerformanceDropped?.Invoke();
                hasReportedDrop = true;
                Debug.LogWarning($"Performance drop detected: {currentFPS:F1} FPS");
            }
            else if (currentFPS >= 50f)
            {
                hasReportedDrop = false;
            }
        }

#if UNITY_EDITOR
        [ContextMenu("Show Performance Report")]
        private void ShowPerformanceReport()
        {
            Debug.Log(GetPerformanceReport());
        }

        [ContextMenu("Reset FPS Stats")]
        private void ResetFPSStats()
        {
            ResetStats();
            Debug.Log("FPS statistics reset");
        }
#endif
    }
}