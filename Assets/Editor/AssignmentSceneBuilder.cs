using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.UI;
using TMPro;

public class AssignmentSceneBuilder : EditorWindow
{
    // [MenuItem("VR Tools/Generate Assignment Scene")]
    public static void GenerateScene()
    {
        // 1. Create a new Scene
        var newScene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);
        
        // 2. Setup the Environment
        GameObject envParent = new GameObject("Environment");
        
        GameObject floor = GameObject.CreatePrimitive(PrimitiveType.Plane);
        floor.name = "Floor";
        floor.transform.parent = envParent.transform;
        floor.transform.localScale = new Vector3(2, 1, 2);
        floor.AddComponent<UnityEngine.XR.Interaction.Toolkit.Locomotion.Teleportation.TeleportationArea>();

        // Create some simple walls for context
        GameObject wall1 = GameObject.CreatePrimitive(PrimitiveType.Cube);
        wall1.name = "Wall_North";
        wall1.transform.parent = envParent.transform;
        wall1.transform.position = new Vector3(0, 2.5f, 10);
        wall1.transform.localScale = new Vector3(20, 5, 1);

        GameObject wall2 = GameObject.CreatePrimitive(PrimitiveType.Cube);
        wall2.name = "Wall_South";
        wall2.transform.parent = envParent.transform;
        wall2.transform.position = new Vector3(0, 2.5f, -10);
        wall2.transform.localScale = new Vector3(20, 5, 1);

        // 3. System Managers
        GameObject managerList = new GameObject("Managers");
        var audioManager = managerList.AddComponent<AudioManager>();
        var investigationManager = managerList.AddComponent<InvestigationManager>();

        // 4. Investigation Points
        string[] idNames = { "A", "B", "C", "D", "E" };
        string[] names = { "Spilled Oil", "Exposed Wire", "Missing Guard", "Worn-out PPE", "Overloaded Socket" };
        string[] descriptions = {
            "Looks like machine oil. Highly slippery and flammable.",
            "Sparks are coming from this exposed wire near the puddle.",
            "The safety guard is completely missing from this gear.",
            "A torn protective glove, completely inappropriate for use.",
            "Too many devices plugged in here. It's overheating."
        };
        Vector3[] positions = { 
            new Vector3(2, 0.1f, 2), 
            new Vector3(2.5f, 0.1f, 2), 
            new Vector3(1, 1, 3), 
            new Vector3(-2, 0.1f, -1), 
            new Vector3(-1, 1.5f, -2) 
        };
        PrimitiveType[] shapes = { PrimitiveType.Cylinder, PrimitiveType.Capsule, PrimitiveType.Cube, PrimitiveType.Sphere, PrimitiveType.Cube };

        GameObject pointsParent = new GameObject("Investigation Points");

        for(int i = 0; i < 5; i++)
        {
            GameObject point = GameObject.CreatePrimitive(shapes[i]);
            point.name = names[i];
            point.transform.parent = pointsParent.transform;
            point.transform.position = positions[i];
            
            // Adjust scaling
            if (i == 0) point.transform.localScale = new Vector3(1, 0.05f, 1); // Flat puddle
            if (i == 1) point.transform.localScale = new Vector3(0.2f, 0.5f, 0.2f); // wire
            if (i == 3) point.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f); // ppe
            if (i == 4) point.transform.localScale = new Vector3(0.4f, 0.6f, 0.2f); // socket

            var ip = point.AddComponent<InvestigationPoint>();
            ip.pointID = idNames[i];
            ip.pointName = names[i];
            ip.description = descriptions[i];

            // Simple popup Canvas for the point
            GameObject pointCanvas = new GameObject("PopupCanvas");
            pointCanvas.transform.parent = point.transform;
            pointCanvas.transform.localPosition = new Vector3(0, 1.5f, 0);
            var pc = pointCanvas.AddComponent<Canvas>();
            pc.renderMode = RenderMode.WorldSpace;
            pc.GetComponent<RectTransform>().sizeDelta = new Vector2(2, 1);
            pointCanvas.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
            var txt = pointCanvas.AddComponent<TextMeshProUGUI>();
            txt.text = "";
            txt.alignment = TextAlignmentOptions.Center;
            txt.fontSize = 2; // For TMP world space
            
            // Re-assign references using reflection/serialized object for private/serialized fields if needed, 
            // but since we made infoPopupCanvas and text private serialized, let's use SerializedObject.
            SerializedObject so = new SerializedObject(ip);
            so.FindProperty("infoPopupCanvas").objectReferenceValue = pointCanvas;
            so.FindProperty("infoPopupText").objectReferenceValue = txt;
            so.ApplyModifiedProperties();

            investigationManager.allPoints.Add(ip);
        }

        // 5. Try to Auto-Instantiate XR Rig and Device Simulator from Starter Assets
        bool foundRig = TryInstantiatePrefab("XR Origin", "XR Origin");
        bool foundSimulator = TryInstantiatePrefab("XR Device Simulator", "XR Device Simulator");
        bool foundInteractionManager = TryInstantiatePrefab("XR Interaction Manager", "XR Interaction Manager");
        bool foundEventSystem = TryInstantiatePrefab("EventSystem", "XR UI Event System");

        if (!foundRig) Debug.LogWarning("Could not find 'XR Origin' prefab. Please drop it into the scene manually.");
        if (!foundSimulator) Debug.LogWarning("Could not find 'XR Device Simulator' prefab. Please drop it into the scene to test in Editor.");
        if (!foundInteractionManager) new GameObject("XR Interaction Manager").AddComponent<XRInteractionManager>();
        
        // 6. Basic UI (HUD and Cause Selection Panel)
        CreateUI();

        // 7. Save Scene
        string scenePath = "Assets/Scenes/Assignment.unity";
        
        // Ensure Scenes folder exists
        if (!AssetDatabase.IsValidFolder("Assets/Scenes"))
        {
            AssetDatabase.CreateFolder("Assets", "Scenes");
        }
        
        EditorSceneManager.SaveScene(newScene, scenePath);
        Debug.Log("Scene 'Assignment' generated and saved at " + scenePath + "!");
    }

    private static bool TryInstantiatePrefab(string searchName, string objectName = null)
    {
        string[] guids = AssetDatabase.FindAssets(searchName + " t:prefab");
        foreach(string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            if (path.Contains(searchName)) // ensure exact or close match
            {
                GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                if (prefab != null)
                {
                    GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
                    if (objectName != null) instance.name = objectName;
                    return true;
                }
            }
        }
        return false;
    }

    private static void CreateUI()
    {
        GameObject uiRoot = new GameObject("UI_Root");
        
        // HUD
        GameObject hudCanvasObj = new GameObject("HUD Canvas");
        hudCanvasObj.transform.parent = uiRoot.transform;
        hudCanvasObj.transform.position = new Vector3(0, 1.5f, 2f);
        
        Canvas hudCanvas = hudCanvasObj.AddComponent<Canvas>();
        hudCanvas.renderMode = RenderMode.WorldSpace;
        ((RectTransform)hudCanvas.transform).sizeDelta = new Vector2(800, 400);
        hudCanvasObj.transform.localScale = new Vector3(0.002f, 0.002f, 0.002f);
        
        hudCanvasObj.AddComponent<CanvasScaler>();
        // Using standard GraphicRaycaster for simplicity to avoid namespace issues, XR pointer should work if using TrackedDeviceGraphicRaycaster but GraphicRaycaster is fallback
        hudCanvasObj.AddComponent<GraphicRaycaster>();
        hudCanvasObj.AddComponent<UIFollowPlayer>();
        
        HUDController hudController = hudCanvasObj.AddComponent<HUDController>();
        
        // Progress Text
        GameObject progressTextObj = new GameObject("ProgressText");
        progressTextObj.transform.parent = hudCanvasObj.transform;
        progressTextObj.transform.localPosition = new Vector3(0, 150, 0);
        TextMeshProUGUI progressText = progressTextObj.AddComponent<TextMeshProUGUI>();
        progressText.text = "Evidence Found: 0 / 5";
        progressText.fontSize = 50;
        progressText.alignment = TextAlignmentOptions.Center;
        
        // Button
        GameObject buttonObj = new GameObject("CompleteButton");
        buttonObj.transform.parent = hudCanvasObj.transform;
        buttonObj.transform.localPosition = new Vector3(0, -100, 0);
        buttonObj.AddComponent<Image>().color = Color.white;
        Button btn = buttonObj.AddComponent<Button>();
        ((RectTransform)buttonObj.transform).sizeDelta = new Vector2(400, 100);
        GameObject btnText = new GameObject("Text");
        btnText.transform.parent = buttonObj.transform;
        btnText.transform.localPosition = Vector3.zero;
        var btnTmp = btnText.AddComponent<TextMeshProUGUI>();
        btnTmp.text = "Complete Investigation";
        btnTmp.fontSize = 30;
        btnTmp.color = Color.black;
        btnTmp.alignment = TextAlignmentOptions.Center;

        // Tooltip
        GameObject tooltipObj = new GameObject("Tooltip");
        tooltipObj.transform.parent = hudCanvasObj.transform;
        tooltipObj.transform.localPosition = new Vector3(0, -180, 0);
        var tltText = tooltipObj.AddComponent<TextMeshProUGUI>();
        tltText.text = "Find more evidence first!";
        tltText.color = Color.red;
        tltText.alignment = TextAlignmentOptions.Center;
        tooltipObj.SetActive(false);

        // Link HUD
        SerializedObject hudSO = new SerializedObject(hudController);
        hudSO.FindProperty("progressText").objectReferenceValue = progressText;
        hudSO.FindProperty("completeInvestigateButton").objectReferenceValue = btn;
        hudSO.FindProperty("tooltipBox").objectReferenceValue = tooltipObj;
        
        
        // Cause Selection Panel
        GameObject causePanelObj = new GameObject("Cause Selection Panel");
        causePanelObj.transform.parent = uiRoot.transform;
        causePanelObj.transform.position = new Vector3(0, 1.5f, 2.5f);
        Canvas causeCanvas = causePanelObj.AddComponent<Canvas>();
        causeCanvas.renderMode = RenderMode.WorldSpace;
        ((RectTransform)causePanelObj.transform).sizeDelta = new Vector2(1000, 600);
        causePanelObj.transform.localScale = new Vector3(0.002f, 0.002f, 0.002f);
        causePanelObj.AddComponent<GraphicRaycaster>();
        CauseSelectionPanel causeController = causePanelObj.AddComponent<CauseSelectionPanel>();

        GameObject titleTextObj = new GameObject("Title");
        titleTextObj.transform.parent = causePanelObj.transform;
        titleTextObj.transform.localPosition = new Vector3(0, 250, 0);
        var titleText = titleTextObj.AddComponent<TextMeshProUGUI>();
        titleText.text = "What caused the accident?";
        titleText.fontSize = 60;
        titleText.alignment = TextAlignmentOptions.Center;

        string[] causes = {
            "Exposed electrical wire caused ignition of spilled oil",
            "Worker slipped on wet floor",
            "Overloaded power socket overheated",
            "Broken ladder caused worker to fall"
        };
        
        SerializedObject causeSO = new SerializedObject(causeController);
        SerializedProperty causeOptionsProp = causeSO.FindProperty("causeOptions");
        causeOptionsProp.arraySize = 4;
        
        for(int i = 0; i < 4; i++)
        {
            GameObject optBtnObj = new GameObject("Option_" + i);
            optBtnObj.transform.parent = causePanelObj.transform;
            optBtnObj.transform.localPosition = new Vector3(0, 120 - (i * 90), 0);
            optBtnObj.AddComponent<Image>().color = new Color(0.2f, 0.2f, 0.2f);
            Button optBtn = optBtnObj.AddComponent<Button>();
            ((RectTransform)optBtnObj.transform).sizeDelta = new Vector2(800, 80);
            
            GameObject optTxtObj = new GameObject("Text");
            optTxtObj.transform.parent = optBtnObj.transform;
            optTxtObj.transform.localPosition = Vector3.zero;
            var optTxt = optTxtObj.AddComponent<TextMeshProUGUI>();
            optTxt.text = causes[i];
            optTxt.fontSize = 30;
            optTxt.alignment = TextAlignmentOptions.Center;

            SerializedProperty element = causeOptionsProp.GetArrayElementAtIndex(i);
            element.FindPropertyRelative("button").objectReferenceValue = optBtn;
            element.FindPropertyRelative("causeText").stringValue = causes[i];
        }

        // Result Panel
        GameObject resPanelObj = new GameObject("ResultPanel");
        resPanelObj.transform.parent = causePanelObj.transform;
        resPanelObj.transform.localPosition = Vector3.zero;
        Image resBackground = resPanelObj.AddComponent<Image>();
        ((RectTransform)resPanelObj.transform).sizeDelta = new Vector2(1000, 600);
        resBackground.color = Color.black;
        
        GameObject resTxtObj = new GameObject("ResultText");
        resTxtObj.transform.parent = resPanelObj.transform;
        resTxtObj.transform.localPosition = Vector3.zero;
        var resTxt = resTxtObj.AddComponent<TextMeshProUGUI>();
        resTxt.text = "Result";
        resTxt.fontSize = 50;
        resTxt.alignment = TextAlignmentOptions.Center;

        causeSO.FindProperty("resultPanel").objectReferenceValue = resPanelObj;
        causeSO.FindProperty("resultText").objectReferenceValue = resTxt;
        causeSO.FindProperty("resultPanelBackground").objectReferenceValue = resBackground;
        causeSO.ApplyModifiedProperties();

        hudSO.FindProperty("causeSelectionPanel").objectReferenceValue = causePanelObj;
        hudSO.ApplyModifiedProperties();
        
        causePanelObj.SetActive(false);
    }
}
