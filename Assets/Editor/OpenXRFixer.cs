using UnityEditor;
using UnityEngine;
using UnityEditor.XR.Management;
using UnityEngine.XR.Management;

public class OpenXRFixer : EditorWindow
{
    // [MenuItem("VR Tools/Fix OpenXR For Editor Play")]
    public static void FixOpenXRForEditor()
    {
        bool fixedSomething = false;

        // 1. Disable XR Initialization on Startup for PC (Prevents OpenXR Validation blocking the Play button)
        XRGeneralSettingsPerBuildTarget buildTargetSettings = null;
        EditorBuildSettings.TryGetConfigObject(XRGeneralSettings.k_SettingsKey, out buildTargetSettings);
        if (buildTargetSettings != null)
        {
            XRGeneralSettings standaloneSettings = buildTargetSettings.SettingsForBuildTarget(BuildTargetGroup.Standalone);
            if (standaloneSettings != null && standaloneSettings.InitManagerOnStart)
            {
                standaloneSettings.InitManagerOnStart = false;
                EditorUtility.SetDirty(standaloneSettings);
                Debug.Log("✅ Fixed: Disabled 'Initialize XR on Startup' for PC. The Editor will now use the Device Simulator instead of forcing OpenXR validation.");
                fixedSomething = true;
            }
        }

        // 2. Disable Hand Tracking on the Modality Manager to stop console spam
        #if UNITY_2021_3_OR_NEWER
        var modalityManager = FindObjectOfType<UnityEngine.XR.Interaction.Toolkit.Inputs.XRInputModalityManager>();
        if (modalityManager != null)
        {
            // Reflection used to bypass potential specific version layout
            var so = new SerializedObject(modalityManager);
            var trackHandsProp = so.FindProperty("m_TrackHands");
            if (trackHandsProp != null && trackHandsProp.boolValue == true)
            {
                trackHandsProp.boolValue = false;
                so.ApplyModifiedProperties();
                Debug.Log("✅ Fixed: Disabled Hand Tracking auto-checks on your XR Origin.");
                fixedSomething = true;
            }
        }
        #endif

        if (fixedSomething)
        {
            AssetDatabase.SaveAssets();
            Debug.Log("🎉 All OpenXR settings patched! You can now press the Play button to run the scene.");
        }
        else
        {
            Debug.Log("Settings are already patched. Just press Play!");
        }
    }
}
