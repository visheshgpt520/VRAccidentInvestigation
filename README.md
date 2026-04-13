# VR Accident Investigation Scenario

## Objective
A complete VR-based accident investigation experience. The player explores an industrial/workshop scene, identifies evidence and safety hazards, and completes an investigation to deduce the root cause of the accident.

## Requirements
- **Unity Version**: 2022.3.62f2 or 6000.2.10f1
- **Required Packages**:
  - XR Interaction Toolkit (latest stable, e.g., 2.5.x or 3.x)
  - XR Plugin Management
  - OpenXR Plugin (or Oculus XR Plugin for Meta headsets)
  - TextMeshPro (Essential for UI text)

## Scene Setup Guide

### 1. Project Initialization
1. Create a new 3D project.
2. Open **Package Manager** and install **XR Interaction Toolkit**, **XR Plugin Management**, and **OpenXR** (or Oculus) plugins.
3. Import the **Starter Assets** sample from the XR Interaction Toolkit in the Package Manager.
4. Enable OpenXR or your target plugin in **Edit > Project Settings > XR Plugin Management**.

### 2. Player Rig Setup
1. Drag the **XR Origin (Action-Based)** prefab into the scene.
2. Ensure Left and Right controllers have **XR Ray Interactor** and **XR Direct Interactor** active.
3. Add a **Locomotion System**, **Teleportation Provider**, and **Snap Turn Provider**. Add **Continuous Move Provider** as an alternative option.

### 3. Environment Layout (Industrial Area)
1. **Floor**: A 20x20 plane colored grey (Concrete). Add a **TeleportationArea** to the floor.
2. **Walls & Ceiling**: Define the room boundary using primitive cubes. Keep the lighting relatively dim to add an investigative mood.
3. **Props**: Setup some simple primitives to simulate a workbench, tools, and a broken ladder in the corner.
4. **Electrical Panel**: Use a box primitive on the wall, perhaps with a flickering Point Light near it.

### 4. Investigation Points
Create 5 GameObjects matching these scenarios. For each, attach an **XR Simple Interactable** and the `InvestigationPoint` script:
- **A. Spilled Oil**: Flat cylinder on the floor.
- **B. Exposed Wire**: Small dark cylinder. This is the critical piece of evidence.
- **C. Missing Guard**: Highlighted gap on a machine.
- **D. Worn-out PPE**: A glove/cube on the floor.
- **E. Overloaded Socket**: Small box near the electrical panel.
- Inside `InvestigationPoint`, assign its UI references (e.g., a World Space Canvas showing a pulsing glow, a floating "?" icon, and a TextMeshPro text for the description popup).

### 5. UI Setup
1. **HUD**: Attach a Canvas to the XR Origin Camera (or an empty object running `UIFollowPlayer.cs`). Add `HUDController.cs`. Setup the evidence counter, row of 5 `EvidenceIcon` elements, and a Complete Button.
2. **Cause Selection Panel**: Create a separate World Space Canvas with `CauseSelectionPanel.cs`. Add 4 buttons mapping to the possible causes. Set the Correct cause string exactly to: `"Exposed electrical wire caused ignition of spilled oil"`.

### 6. Managers
1. Add an empty GameObject named **Managers**.
2. Attach `InvestigationManager.cs` and link the 5 Investigation Point objects into its list.
3. Attach `AudioManager.cs` and assign success/error SFX.

## VR Controls Guide
- **Teleportation**: Hold Right Thumbstick forward to aim, release to teleport.
- **Continuous Movement**: Left Thumbstick (forward/backward/strafe).
- **Turning**: Right Thumbstick left/right for Snap Turn (45 degrees).
- **Interaction**:
  - **Select/Scan Evidence**: Point Ray and hit the Right Trigger (or use Direct Interactor by reaching out and pressing Grip/Trigger based on XR setup).
  - **UI/Menus**: Point your Controller Ray at the UI and pull the Right Trigger.

## Investigation Flow
1. **Start**: The user looks around and uses locomotion to explore the workshop.
2. **Find Evidence**: The user spots glowing "?" icons above 5 points of interest.
3. **Analyze**: Selecting an evidence spot reveals a 3-second popup describing the clue. The HUD updates progress (e.g., "1/5 Evidence Found").
4. **Completion Lock**: The "Complete Investigation" button on the HUD is grayed out until at least 4 out of 5 clues are found.
5. **Final Decision**: Clicking the Complete Button opens the Final Analysis Panel.
6. **Submit Cause**: The user reads 4 possible causes. Choosing correctly yields a success chime and a green confirmation. Choosing wrong displays a red error and permits unlimited retries.

## How to Build and Run
**For Meta Quest:**
1. In `Build Settings`, switch the platform to **Android**.
2. In Player Settings, set Texture Compression to ASTC.
3. Ensure the Oculus / OpenXR Plugin is initialized for Android.
4. Plug in your Meta Quest and click **Build and Run**.

**For PC VR:**
1. In `Build Settings`, keep the platform on **PC, Mac & Linux Standalone** (Windows).
2. Ensure SteamVR or Oculus App is running on your PC.
3. Click Play in the Unity Editor to test via Link, or **Build** to generate an `.exe`.
