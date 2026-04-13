using UnityEngine;
using UnityEngine.InputSystem;
#if UNITY_EDITOR
using UnityEditor;
#endif

// This script will automatically attach itself to the scene and log explicitly what is broken
#if UNITY_EDITOR
[InitializeOnLoad]
#endif
public class DebugDiagnostics : MonoBehaviour
{
#if UNITY_EDITOR
    static DebugDiagnostics()
    {
        EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
    }

    private static void OnPlayModeStateChanged(PlayModeStateChange state)
    {
        if (state == PlayModeStateChange.EnteredPlayMode)
        {
            // Auto inject into scene
            var obj = new GameObject("DEBUG_DIAGNOSTICS_LOGGER");
            obj.AddComponent<DebugDiagnostics>();
        }
    }
#endif

    private void Start()
    {
        Debug.Log("<color=cyan>--- 🔥 VR ACCIDENT INVESTIGATION DIAGNOSTICS 🔥 ---</color>");

        // 1. Check Simulator
        var simulator = Object.FindObjectOfType<UnityEngine.XR.Interaction.Toolkit.Inputs.Simulation.XRDeviceSimulator>();
        if (simulator == null)
            Debug.LogError("<color=red>❌ CRITICAL: XR Device Simulator is entirely missing from the hierarchy.</color>\nWithout this, your Keyboard and Mouse clicks are completely ignored! (Try dragging the XR Device Simulator prefab directly into the scene list).");
        else
            Debug.Log("<color=green>✅ XR Device Simulator is ACTIVE in the scene.</color>");

        // 2. Check XR Origin
        var origin = Object.FindObjectOfType<Unity.XR.CoreUtils.XROrigin>();
        if (origin == null)
            Debug.LogError("<color=red>❌ CRITICAL: XR Origin is missing. You have no camera or hands.</color>");
        else
            Debug.Log("<color=green>✅ XR Origin (Player) is present.</color>");
            
        // 3. Check Input System
        var eventSystem = Object.FindObjectOfType<UnityEngine.EventSystems.EventSystem>();
        if (eventSystem == null)
            Debug.LogError("<color=red>❌ CRITICAL: No EventSystem found. Your UI clicks will not register.</color>");
        else
        {
            var oldInputModule = eventSystem.GetComponent<UnityEngine.EventSystems.StandaloneInputModule>();
            if (oldInputModule != null)
                Debug.LogWarning("<color=yellow>⚠️ WARNING: EventSystem is using the Old Input System module. VR clicking might fail. Replace it with UI Input Module.</color>");
            else
                Debug.Log("<color=green>✅ EventSystem is healthy.</color>");
        }

        // 4. Check Keyboards
        if (Keyboard.current == null)
            Debug.LogError("<color=red>❌ CRITICAL: Unity does not detect your Keyboard! Game Window might not have focus, or Input System is disabled.</color>");
        else
            Debug.Log("<color=green>✅ Keyboard mapped correctly.</color>");

        Debug.Log("<color=cyan>-----------------------------------------------------</color>");
    }

    private void Update()
    {
        if (Keyboard.current != null)
        {
            if (Keyboard.current.spaceKey.wasPressedThisFrame)
                Debug.Log("👉 <color=orange>[DEBUG]: Spacebar pressed!</color> If you don't see your right hand move, the Device Simulator is ignoring the input.");
                
            if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
                Debug.Log("👉 <color=orange>[DEBUG]: Left Mouse Button Clicked!</color> If you were aiming at a clue and it didn't trigger, the Raycaster or interactable script is blocked.");
        }
    }
}
