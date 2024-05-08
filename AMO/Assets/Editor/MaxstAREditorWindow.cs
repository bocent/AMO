using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
[InitializeOnLoad]
public class MaxstAREditorWindow : EditorWindow
{
    private static readonly string SAMPLE_SCENE_PATH = "Assets/MaxstARSamples/Scenes";
    private static readonly string TRACKABLE_PATH = "Assets/MaxstAR/Prefabs";

    static MaxstAREditorWindow()
    {
        EditorApplication.delayCall += DelayedAction;
    }

    private static void DelayedAction()
    {
        EditorApplication.delayCall -= DelayedAction;
    }

    #region BuildSetting
    [MenuItem("Help/MaxstAR/Add All SampleScene to BuildSetting", false, 51)]
    static void AddScenesFromFolderToBuildSettings()
    {
        var allScenes = Directory.GetFiles(SAMPLE_SCENE_PATH, "*.unity", SearchOption.AllDirectories)
            .Select(path => path.Replace("\\", "/"))
            .ToList();

        var homeScenePath = allScenes.FirstOrDefault(scene => scene.EndsWith("Home.unity", System.StringComparison.OrdinalIgnoreCase));
        var otherScenePaths = allScenes.Where(scene => !scene.EndsWith("Home.unity")).ToList();
        var otherScene = otherScenePaths.Select(scene => new EditorBuildSettingsScene(scene, true)).ToArray();

        var currentScenes = EditorBuildSettings.scenes.ToList();
        var sceneGuids = currentScenes.Select(s => s.guid).ToList();

        if (!string.IsNullOrEmpty(homeScenePath))
        {
            var homeScene = new EditorBuildSettingsScene(homeScenePath, true);
            if (!sceneGuids.Contains(homeScene.guid))
            {
                currentScenes.Insert(0, homeScene);
            }
        }

        var filteringScene = otherScene.Where(addWaitScene => !sceneGuids.Contains(addWaitScene.guid));

        currentScenes.AddRange(filteringScene);
        EditorBuildSettings.scenes = currentScenes.Distinct().ToArray();
        EditorWindow.GetWindow(typeof(BuildPlayerWindow), true, "Build Settings");
    }

    [MenuItem("Help/MaxstAR/Remove All SampleScene to BuildSetting", false, 52)]
    static void RemoveScenesFromFolderFromBuildSettings()
    {
        var scenes = Directory.GetFiles(SAMPLE_SCENE_PATH, "*.unity", SearchOption.AllDirectories)
            .Select(path => path.Replace("\\", "/"))
            .ToList();

        var currentScenes = EditorBuildSettings.scenes.ToList();

        currentScenes.RemoveAll(scene => scenes.Contains(scene.path));
        EditorBuildSettings.scenes = currentScenes.ToArray();
        EditorWindow.GetWindow(typeof(BuildPlayerWindow), true, "Build Settings");
    }
    #endregion

    #region Add Trackage to Scene
    private static void AddTrackable(string prefabPath)
    {
        var currentActiveScnene = SceneManager.GetActiveScene();
        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
        GameObject instance = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
        instance.name = prefab.name;
        EditorSceneManager.MarkSceneDirty(currentActiveScnene);
    }

    [MenuItem("GameObject/MaxstAR/Add Trackable to Scene/ARCamera", false, 1001)]
    [MenuItem("Help/MaxstAR/Add Trackable to Scene/ARCamera", false, 1)]
    public static void AddARCameraPrefab()
    {
        string prefabPath = $"{TRACKABLE_PATH}/ARCamera.prefab";
        AddTrackable(prefabPath);
    }

    [MenuItem("GameObject/MaxstAR/Add Trackable to Scene/CloudTrackable", false, 1001)]
    [MenuItem("Help/MaxstAR/Add Trackable to Scene/CloudTrackable", false, 1)]
    public static void AddCloudTrackablePrefab()
    {
        string prefabPath = $"{TRACKABLE_PATH}/CloudTrackable.prefab";
        AddTrackable(prefabPath);
    }

    [MenuItem("GameObject/MaxstAR/Add Trackable to Scene/CodeTrackable", false, 1001)]
    [MenuItem("Help/MaxstAR/Add Trackable to Scene/CodeTrackable", false, 1)]
    public static void AddCodeTrackablePrefab()
    {
        string prefabPath = $"{TRACKABLE_PATH}/CodeTrackable.prefab";
        AddTrackable(prefabPath);
    }

    [MenuItem("GameObject/MaxstAR/Add Trackable to Scene/ImageTrackable", false, 1001)]
    [MenuItem("Help/MaxstAR/Add Trackable to Scene/ImageTrackable", false, 1)]
    public static void AddImageTrackablePrefab()
    {
        string prefabPath = $"{TRACKABLE_PATH}/ImageTrackable.prefab";
        AddTrackable(prefabPath);
    }

    [MenuItem("GameObject/MaxstAR/Add Trackable to Scene/InstantTrackable", false, 1001)]
    [MenuItem("Help/MaxstAR/Add Trackable to Scene/InstantTrackable", false, 1)]
    public static void AddInstantTrackablePrefab()
    {
        string prefabPath = $"{TRACKABLE_PATH}/InstantTrackable.prefab";
        AddTrackable(prefabPath);
    }

    [MenuItem("GameObject/MaxstAR/Add Trackable to Scene/MarkerGroup", false, 1001)]
    [MenuItem("Help/MaxstAR/Add Trackable to Scene/MarkerGroup", false, 1)]
    public static void AddMarkerGroupPrefab()
    {
        string prefabPath = $"{TRACKABLE_PATH}/MarkerGroup.prefab";
        AddTrackable(prefabPath);
    }

    [MenuItem("GameObject/MaxstAR/Add Trackable to Scene/ObjectTrackable", false, 1001)]
    [MenuItem("Help/MaxstAR/Add Trackable to Scene/ObjectTrackable", false, 1)]
    public static void AddObjectTrackablePrefab()
    {
        string prefabPath = $"{TRACKABLE_PATH}/ObjectTrackable.prefab";
        AddTrackable(prefabPath);
    }

    [MenuItem("GameObject/MaxstAR/Add Trackable to Scene/QrCodeTrackable", false, 1001)]
    [MenuItem("Help/MaxstAR/Add Trackable to Scene/QrCodeTrackable", false, 1)]
    public static void AddQrCodeTrackablePrefab()
    {
        string prefabPath = $"{TRACKABLE_PATH}/QrCodeTrackable.prefab";
        AddTrackable(prefabPath);
    }

    [MenuItem("GameObject/MaxstAR/Add Trackable to Scene/SpaceTrackable", false, 1001)]
    [MenuItem("Help/MaxstAR/Add Trackable to Scene/SpaceTrackable", false, 1)]
    public static void AddSpaceTrackablePrefab()
    {
        string prefabPath = $"{TRACKABLE_PATH}/SpaceTrackable.prefab";
        AddTrackable(prefabPath);
    }
    #endregion

    #region OpenSampleScene
    [MenuItem("Assets/MaxstAR/Open SampleScene/Home Scene", false, 1)]
    [MenuItem("Help/MaxstAR/Open SampleScene/Home Scene", false, 1)]
    public static void AddHomeScene()
    {
        string sceneFolderPath = $"{SAMPLE_SCENE_PATH}/Home.unity";
        AddSampleScene(sceneFolderPath);
    }

    [MenuItem("Assets/MaxstAR/Open SampleScene/ImageTracker Scene", false, 2)]
    [MenuItem("Help/MaxstAR/Open SampleScene/ImageTracker Scene", false, 2)]
    public static void AddImageTrackerScene()
    {
        string sceneFolderPath = $"{SAMPLE_SCENE_PATH}/ImageTracker.unity";
        AddSampleScene(sceneFolderPath);
    }

    [MenuItem("Assets/MaxstAR/Open SampleScene/InstantTracker Scene", false, 3)]
    [MenuItem("Help/MaxstAR/Open SampleScene/InstantTracker Scene", false, 3)]
    public static void AddInstantTrackerScene()
    {
        string sceneFolderPath = $"{SAMPLE_SCENE_PATH}/InstantTracker.unity";
        AddSampleScene(sceneFolderPath);
    }

    [MenuItem("Assets/MaxstAR/Open SampleScene/MarkerTracker Scene", false, 4)]
    [MenuItem("Help/MaxstAR/Open SampleScene/MarkerTracker Scene", false, 4)]
    public static void AddMarkerTrackerScene()
    {
        string sceneFolderPath = $"{SAMPLE_SCENE_PATH}/MarkerTracker.unity";
        AddSampleScene(sceneFolderPath);
    }

    [MenuItem("Assets/MaxstAR/Open SampleScene/ObjectTracker Scene", false, 5)]
    [MenuItem("Help/MaxstAR/Open SampleScene/ObjectTracker Scene", false, 5)]
    public static void AddObjectTrackerScene()
    {
        string sceneFolderPath = $"{SAMPLE_SCENE_PATH}/ObjectTracker.unity";
        AddSampleScene(sceneFolderPath);
    }

    [MenuItem("Assets/MaxstAR/Open SampleScene/QrCodeTracker Scene", false, 6)]
    [MenuItem("Help/MaxstAR/Open SampleScene/QrCodeTracker Scene", false, 6)]
    public static void AddQrCodeTrackerScene()
    {
        string sceneFolderPath = $"{SAMPLE_SCENE_PATH}/QrCodeTracker.unity";
        AddSampleScene(sceneFolderPath);
    }

    [MenuItem("Assets/MaxstAR/Open SampleScene/SpaceTracker Scene", false, 7)]
    [MenuItem("Help/MaxstAR/Open SampleScene/SpaceTracker Scene", false, 7)]
    public static void AddSpaceTrackerScene()
    {
        string sceneFolderPath = $"{SAMPLE_SCENE_PATH}/SpaceTracker.unity";
        AddSampleScene(sceneFolderPath);
    }

    [MenuItem("Assets/MaxstAR/Open SampleScene/VideoTracker Scene", false, 8)]
    [MenuItem("Help/MaxstAR/Open SampleScene/VideoTracker Scene", false, 8)]
    public static void AddVideoTrackerScene()
    {
        string sceneFolderPath = $"{SAMPLE_SCENE_PATH}/VideoTracker.unity";
        AddSampleScene(sceneFolderPath);
    }

    [MenuItem("Assets/MaxstAR/Open SampleScene/CameraConfiguration Scene", false, 10)]
    [MenuItem("Help/MaxstAR/Open SampleScene/CameraConfiguration Scene", false, 10)]
    public static void AddCameraConfigurationScene()
    {
        string sceneFolderPath = $"{SAMPLE_SCENE_PATH}/CameraConfiguration.unity";
        AddSampleScene(sceneFolderPath);
    }
    [MenuItem("Assets/MaxstAR/Open SampleScene/CloudRecognizer Scene", false, 10)]
    [MenuItem("Help/MaxstAR/Open SampleScene/CloudRecognizer Scene", false, 10)]
    public static void AddCloudRecognizerScene()
    {
        string sceneFolderPath = $"{SAMPLE_SCENE_PATH}/CloudRecognizer.unity";
        AddSampleScene(sceneFolderPath);
    }
    [MenuItem("Assets/MaxstAR/Open SampleScene/CodeReader Scene", false, 10)]
    [MenuItem("Help/MaxstAR/Open SampleScene/CodeReader Scene", false, 10)]
    public static void AddCodeReaderScene()
    {
        string sceneFolderPath = $"{SAMPLE_SCENE_PATH}/CodeReader.unity";
        AddSampleScene(sceneFolderPath);
    }

    [MenuItem("Assets/MaxstAR/Open SampleScene/ImageFusionTracker Scene", false, 10)]
    [MenuItem("Help/MaxstAR/Open SampleScene/ImageFusionTracker Scene", false, 10)]
    public static void AddImageFusionTrackerScene()
    {
        string sceneFolderPath = $"{SAMPLE_SCENE_PATH}/ImageFusionTracker.unity";
        AddSampleScene(sceneFolderPath);
    }

    [MenuItem("Assets/MaxstAR/Open SampleScene/InstantFusionTracker Scene", false, 10)]
    [MenuItem("Help/MaxstAR/Open SampleScene/InstantFusionTracker Scene", false, 10)]
    public static void AddInstantFusionTrackerScene()
    {
        string sceneFolderPath = $"{SAMPLE_SCENE_PATH}/InstantFusionTracker.unity";
        AddSampleScene(sceneFolderPath);
    }

    [MenuItem("Assets/MaxstAR/Open SampleScene/MarkerFusionTracker Scene", false, 10)]
    [MenuItem("Help/MaxstAR/Open SampleScene/MarkerFusionTracker Scene", false, 10)]
    public static void AddMarkerFusionTrackerScene()
    {
        string sceneFolderPath = $"{SAMPLE_SCENE_PATH}/MarkerFusionTracker.unity";
        AddSampleScene(sceneFolderPath);
    }

    [MenuItem("Assets/MaxstAR/Open SampleScene/ObjectFusionTracker Scene", false, 10)]
    [MenuItem("Help/MaxstAR/Open SampleScene/ObjectFusionTracker Scene", false, 10)]
    public static void AddObjectFusionTrackerScene()
    {
        string sceneFolderPath = $"{SAMPLE_SCENE_PATH}/ObjectFusionTracker.unity";
        AddSampleScene(sceneFolderPath);
    }

    [MenuItem("Assets/MaxstAR/Open SampleScene/QrCodeFusionTracker Scene", false, 10)]
    [MenuItem("Help/MaxstAR/Open SampleScene/QrCodeFusionTracker Scene", false, 10)]
    public static void AddQrCodeFusionTrackerScene()
    {
        string sceneFolderPath = $"{SAMPLE_SCENE_PATH}/QrCodeFusionTracker.unity";
        AddSampleScene(sceneFolderPath);
    }

    private static void AddSampleScene(string sceneFolderPath)
    {
        Object scenePath = AssetDatabase.LoadAssetAtPath<Object>(sceneFolderPath);
        Selection.activeObject = scenePath;
        EditorGUIUtility.PingObject(scenePath);

        Scene openedScene = EditorSceneManager.OpenScene(sceneFolderPath, OpenSceneMode.Additive);
        SceneManager.SetActiveScene(openedScene);
        //DeActiveScenes(openedScene);
    }

    private static void DeActiveScenes(Scene openedScene)
    {
        for (int i = 0; i < SceneManager.sceneCount; ++i)
        {
            Scene scene = SceneManager.GetSceneAt(i);
            if (scene != openedScene && scene.isLoaded)
            {
                SceneManager.UnloadSceneAsync(scene);
            }
        }
    }
    #endregion

    #region Enter License
    [MenuItem("Help/MaxstAR/License/Enter License Key")]
    public static void SetLicenseKey()
    {
        string sceneFolderPath = $"Assets/Resources/MaxstAR/Configuration.asset";

        maxstAR.ConfigurationScriptableObject scenePath = AssetDatabase.LoadAssetAtPath<maxstAR.ConfigurationScriptableObject>(sceneFolderPath);
        Selection.activeObject = scenePath;
        EditorGUIUtility.PingObject(scenePath);
        SettingsService.OpenProjectSettings("Project/Player");
    }
    #endregion

    #region ETC

    [MenuItem("Help/MaxstAR/Document", false, 10000)]
    static void ShowDocument()
    {
        Application.OpenURL("https://developer.maxst.com/MD/doc/6_2_x/intro");
    }

    [MenuItem("Help/MaxstAR/Forum", false, 10001)]
    static void ShowHelp()
    {
        Application.OpenURL("https://developer.maxst.com/BoardQuestions/");
    }

    [MenuItem("Help/MaxstAR/Tutorial", false, 10002)]
    static void ShowTutorial()
    {
        Application.OpenURL("https://developer.maxst.com/MD/tutorial");
    }

    [MenuItem("Help/MaxstAR/License Manager", false, 10003)]
    static void ShowLicenseManager()
    {
        Application.OpenURL("https://developer.maxst.com/MD/doc/6_2_x/tools/licensem");
    }

    [MenuItem("Help/MaxstAR/Release Note", false, 10004)]
    static void ShowReleaseNote()
    {
        Application.OpenURL("https://developer.maxst.com/MD/doc/g/release");
    }
    #endregion
}
#endif
