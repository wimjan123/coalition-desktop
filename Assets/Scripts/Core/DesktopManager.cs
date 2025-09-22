using UnityEngine;
using UnityEngine.UIElements;

namespace Coalition.Core
{
    public class DesktopManager : MonoBehaviour
    {
        private static DesktopManager instance;
        public static DesktopManager Instance => instance;

        public static Rect DesktopBounds { get; private set; }

        [SerializeField] private UIDocument uiDocument;
        private VisualElement desktopRoot;
        private VisualElement desktopBackground;

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
            InitializeDesktop();
        }

        private void InitializeDesktop()
        {
            // Get or create UIDocument
            if (uiDocument == null)
            {
                uiDocument = FindFirstObjectByType<UIDocument>();
            }

            if (uiDocument == null)
            {
                // Create UIDocument if it doesn't exist
                var uiDocumentGO = new GameObject("UI Document");
                uiDocument = uiDocumentGO.AddComponent<UIDocument>();

                // Load the desktop UXML
                var visualTree = Resources.Load<VisualTreeAsset>("UI/Desktop/Desktop");
                if (visualTree != null)
                {
                    uiDocument.visualTreeAsset = visualTree;
                }
            }

            // Get root element
            desktopRoot = uiDocument.rootVisualElement;
            if (desktopRoot == null)
            {
                Debug.LogError("Failed to get desktop root element");
                return;
            }

            // Load and apply desktop styles
            var styleSheet = Resources.Load<StyleSheet>("UI/Desktop/Desktop");
            if (styleSheet != null)
            {
                desktopRoot.styleSheets.Add(styleSheet);
            }

            // Get desktop background
            desktopBackground = desktopRoot.Q("desktop-background");

            // Calculate desktop bounds (full screen minus dock height)
            UpdateDesktopBounds();

            // Set up resize listener
            desktopRoot.RegisterCallback<GeometryChangedEvent>(OnDesktopResized);

            Debug.Log($"Desktop initialized. Bounds: {DesktopBounds}");
        }

        private void OnDesktopResized(GeometryChangedEvent evt)
        {
            UpdateDesktopBounds();
        }

        private void UpdateDesktopBounds()
        {
            if (desktopRoot == null) return;

            var rootRect = desktopRoot.contentRect;

            // Desktop bounds are full screen minus dock height (70px)
            DesktopBounds = new Rect(0, 0, rootRect.width, rootRect.height - 70f);

            Debug.Log($"Desktop bounds updated: {DesktopBounds}");
        }

        public void SetWallpaper(Texture2D wallpaper)
        {
            if (desktopBackground != null && wallpaper != null)
            {
                desktopBackground.style.backgroundImage = new StyleBackground(wallpaper);
                Debug.Log("Wallpaper updated");
            }
        }

        public void SetBackgroundColor(Color color)
        {
            if (desktopBackground != null)
            {
                desktopBackground.style.backgroundColor = color;
                Debug.Log($"Background color updated to {color}");
            }
        }

        public Vector2 GetScreenCenter()
        {
            return new Vector2(DesktopBounds.width / 2f, DesktopBounds.height / 2f);
        }

        public bool IsPositionValid(Vector2 position, Vector2 size)
        {
            return position.x >= 0 &&
                   position.y >= 0 &&
                   position.x + size.x <= DesktopBounds.width &&
                   position.y + size.y <= DesktopBounds.height;
        }

        public Vector2 ClampPositionToBounds(Vector2 position, Vector2 size)
        {
            return new Vector2(
                Mathf.Clamp(position.x, 0, DesktopBounds.width - size.x),
                Mathf.Clamp(position.y, 0, DesktopBounds.height - size.y)
            );
        }

        public void ShowDesktopContextMenu(Vector2 position)
        {
            // Future implementation for right-click desktop menu
            Debug.Log($"Desktop context menu at {position}");
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            // Visualize desktop bounds in Scene view
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireCube(
                new Vector3(DesktopBounds.center.x, DesktopBounds.center.y, 0),
                new Vector3(DesktopBounds.width, DesktopBounds.height, 1)
            );
        }
#endif
    }
}