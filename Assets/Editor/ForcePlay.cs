using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using System.IO;

public class ForcePlay : EditorWindow
{
    // [MenuItem("VR Tools/🔥 FIX AND PLAY! (Click Here)")]
    public static void PlayNow()
    {
        // 1. Switch to the correct scene automatically
        if (EditorSceneManager.GetActiveScene().name != "Assignment")
        {
            if (File.Exists("Assets/Scenes/Assignment.unity"))
            {
                EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
                EditorSceneManager.OpenScene("Assets/Scenes/Assignment.unity");
                Debug.Log("Switched to the 'Assignment' Scene!");
            }
            else
            {
                Debug.LogError("The Assignment scene doesn't exist yet! Click 'Generate Assignment Scene' in the menu first.");
                return;
            }
        }

        // 2. Fix the XRI 3.x Device Simulator Settings automatically
        var guids = AssetDatabase.FindAssets("t:XRInteractionToolkitSettings");
        if (guids.Length > 0)
        {
            var settings = AssetDatabase.LoadAssetAtPath<ScriptableObject>(AssetDatabase.GUIDToAssetPath(guids[0]));
            if (settings != null)
            {
                var so = new SerializedObject(settings);
                var simSettings = so.FindProperty("m_DeviceSimulatorSettings");
                if (simSettings != null)
                {
                    // For Unity XRI 3.0+
                    var enableProp = simSettings.FindPropertyRelative("m_InstantiateInEditorPlayMode");
                    if (enableProp == null) enableProp = simSettings.FindPropertyRelative("m_EnableXRDeviceSimulator");
                    
                    if (enableProp != null)
                    {
                        enableProp.boolValue = true;
                        so.ApplyModifiedProperties();
                        Debug.Log("Device Simulator forced ON in settings!");
                    }
                }
            }
        }

        // 3. Make sure the Input System is set correctly so Keyboard works
        SerializedObject playerSettings = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/ProjectSettings.asset")[0]);
        SerializedProperty activeInputHandler = playerSettings.FindProperty("activeInputHandler");
        if (activeInputHandler != null && activeInputHandler.intValue == 0) // If it's pure "Old"
        {
            activeInputHandler.intValue = 2; // Set to "Both"
            playerSettings.ApplyModifiedProperties();
            Debug.Log("Input System switched to Both so the Keyboard works!");
        }

        // 4. Force Start Play Mode!
        EditorApplication.isPlaying = true;
    }
}
