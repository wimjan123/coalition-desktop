using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using Coalition.Core;

namespace Coalition.Editor
{
    /// <summary>
    /// Editor setup utility for Coalition Desktop Shell
    /// Provides menu items and automated setup for the desktop shell
    /// </summary>
    public static class CoalitionDesktopSetup
    {
        [MenuItem("Coalition/Setup Desktop Shell")]
        public static void SetupDesktopShell()
        {
            Debug.Log("Setting up Coalition Desktop Shell...");

            // Create necessary folders
            CreateFolders();

            // Setup main scene
            SetupMainScene();

            // Configure build settings
            ConfigureBuildSettings();

            Debug.Log("Coalition Desktop Shell setup complete!");
        }

        [MenuItem("Coalition/Validate Setup")]
        public static void ValidateSetup()
        {
            Debug.Log("Validating Coalition Desktop Shell setup...");

            var issues = 0;

            // Check for main scene
            var desktopScene = AssetDatabase.LoadAssetAtPath<UnityEditor.SceneAsset>("Assets/Scenes/Desktop.unity");
            if (desktopScene == null)
            {
                Debug.LogError("Missing Desktop.unity scene in Assets/Scenes/");
                issues++;
            }

            // Check for UI assets
            var desktopUxml = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/UI/Desktop/Desktop.uxml");
            if (desktopUxml == null)
            {
                Debug.LogError("Missing Desktop.uxml in Assets/UI/Desktop/");
                issues++;
            }

            // Check for data files
            string[] dataFiles = {
                "briefings", "messages", "polling", "calendar",
                "cabinet", "policies", "media", "settings"
            };

            foreach (string file in dataFiles)
            {
                var asset = Resources.Load<TextAsset>($"Data/{file}");
                if (asset == null)
                {
                    Debug.LogError($"Missing data file: Assets/Resources/Data/{file}.json");
                    issues++;
                }
            }

            // Check for scripts
            if (!DoesScriptExist("CoalitionDesktop"))
            {
                Debug.LogError("Missing CoalitionDesktop script");
                issues++;
            }

            if (issues == 0)
            {
                Debug.Log("✅ Coalition Desktop Shell validation passed - all components found!");
            }
            else
            {
                Debug.LogError($"❌ Coalition Desktop Shell validation failed - {issues} issues found");
            }
        }

        [MenuItem("Coalition/Create Test Scene")]
        public static void CreateTestScene()
        {
            // Create a new scene for testing
            var scene = UnityEditor.SceneManagement.EditorSceneManager.NewScene(
                UnityEditor.SceneManagement.NewSceneSetup.EmptyScene);

            // Add main camera
            var cameraGO = new GameObject("Main Camera");
            var camera = cameraGO.AddComponent<Camera>();
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = new Color(0.2f, 0.25f, 0.3f, 1f);
            cameraGO.AddComponent<AudioListener>();
            cameraGO.tag = "MainCamera";

            // Add Coalition Desktop
            var desktopGO = new GameObject("Coalition Desktop");
            var uiDoc = desktopGO.AddComponent<UIDocument>();
            var coalitionDesktop = desktopGO.AddComponent<CoalitionDesktop>();

            // Try to assign the UI assets
            var desktopUxml = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/UI/Desktop/Desktop.uxml");
            var panelSettings = AssetDatabase.LoadAssetAtPath<PanelSettings>("Assets/UI/Settings/PanelSettings.asset");

            if (desktopUxml != null)
            {
                uiDoc.visualTreeAsset = desktopUxml;
            }

            if (panelSettings != null)
            {
                uiDoc.panelSettings = panelSettings;
            }

            // Save the scene
            UnityEditor.SceneManagement.EditorSceneManager.SaveScene(scene, "Assets/Scenes/CoalitionTest.unity");

            Debug.Log("Created test scene: Assets/Scenes/CoalitionTest.unity");
        }

        [MenuItem("Coalition/Build for macOS")]
        public static void BuildForMacOS()
        {
            Debug.Log("Building Coalition Desktop Shell for macOS...");

            // Configure build settings
            EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Standalone, BuildTarget.StandaloneOSX);

            // Set player settings for optimal performance
            PlayerSettings.SetScriptingBackend(BuildTargetGroup.Standalone, ScriptingImplementation.IL2CPP);
            PlayerSettings.SetArchitecture(BuildTargetGroup.Standalone, 1); // ARM64

            // Build options
            BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
            buildPlayerOptions.scenes = new[] { "Assets/Scenes/Desktop.unity" };
            buildPlayerOptions.locationPathName = "Builds/CoalitionDesktop.app";
            buildPlayerOptions.target = BuildTarget.StandaloneOSX;
            buildPlayerOptions.options = BuildOptions.None;

            var report = BuildPipeline.BuildPlayer(buildPlayerOptions);

            if (report.summary.result == UnityEditor.Build.Reporting.BuildResult.Succeeded)
            {
                Debug.Log($"✅ Build succeeded: {report.summary.totalSize} bytes");
                EditorUtility.RevealInFinder("Builds/CoalitionDesktop.app");
            }
            else
            {
                Debug.LogError($"❌ Build failed: {report.summary.result}");
            }
        }

        private static void CreateFolders()
        {
            string[] folders = {
                "Assets/Prefabs",
                "Assets/Prefabs/Applications",
                "Assets/Materials",
                "Assets/Textures",
                "Builds"
            };

            foreach (string folder in folders)
            {
                if (!AssetDatabase.IsValidFolder(folder))
                {
                    var pathParts = folder.Split('/');
                    var currentPath = pathParts[0];

                    for (int i = 1; i < pathParts.Length; i++)
                    {
                        var nextPath = currentPath + "/" + pathParts[i];
                        if (!AssetDatabase.IsValidFolder(nextPath))
                        {
                            AssetDatabase.CreateFolder(currentPath, pathParts[i]);
                        }
                        currentPath = nextPath;
                    }
                }
            }

            AssetDatabase.Refresh();
        }

        private static void SetupMainScene()
        {
            // Load the desktop scene
            var scenePath = "Assets/Scenes/Desktop.unity";
            var scene = UnityEditor.SceneManagement.EditorSceneManager.OpenScene(scenePath);

            // Make sure the scene is marked as dirty so it gets saved
            UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(scene);
        }

        private static void ConfigureBuildSettings()
        {
            // Add scenes to build settings
            var scenesToAdd = new[] {
                "Assets/Scenes/Desktop.unity"
            };

            var originalScenes = EditorBuildSettings.scenes;
            var sceneList = new System.Collections.Generic.List<EditorBuildSettingsScene>();

            foreach (string scenePath in scenesToAdd)
            {
                var scene = AssetDatabase.LoadAssetAtPath<UnityEditor.SceneAsset>(scenePath);
                if (scene != null)
                {
                    sceneList.Add(new EditorBuildSettingsScene(scenePath, true));
                }
            }

            EditorBuildSettings.scenes = sceneList.ToArray();
        }

        private static bool DoesScriptExist(string scriptName)
        {
            string[] guids = AssetDatabase.FindAssets($"t:MonoScript {scriptName}");
            return guids.Length > 0;
        }
    }
}