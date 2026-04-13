# Scene Setup Guide for VR Accident Investigation

Since you are assembling the scene in the Unity Editor, please follow these steps to integrate the provided C# scripts and build your assignment.

## 1. Project Configuration & VR Support
Ensure you have the following packages installed via **Window > Package Manager**:
- **XR Plugin Management**
- **OpenXR Plugin** (or Oculus XR Plugin depending on your headset)
- **XR Interaction Toolkit** (XRIT)

Go to **Edit > Project Settings > XR Plug-in Management** and initialize the XR provider for PC and Android (if deploying to Quest standalone).

## 2. Basic Scene Setup
1. **Create an Environment:** Add a plane for the floor, some walls, and lighting. Make it look like a workshop or industrial storage room.
2. **Add VR Player Rig:**
   - In the Hierarchy, right-click and go to **XR > XR Origin (VR)**.
   - This sets up the Camera Offset, Main Camera, and Left/Right Controllers with Ray Interactors.
   - Ensure an **XR Interaction Manager** is created in the scene (usually automatic when adding the XR Origin).

## 3. Manager Setup (Logic & UI)
Create an empty GameObject named **`GameManager`** in your scene.
1. Add the **`InvestigationManager`** script to it.
   - Set **Total Evidence Required** to `4` or `5`.
   - Leave the **Correct Cause** as `"Electrical Short Circuit"` (or change it to match your custom accident).
2. Create a Canvas for your UI (GameObject > UI > Canvas).
   - Change the Canvas Render Mode to `World Space` so it can be viewed in VR. Size it down (e.g., Scale `0.002`) and place it somewhere the player can see it.
   - Add a `Text - TextMeshPro` for **Progress Text** ("Clues Found: 0 / 5").
   - Create a Panel and Text for **Message UI** (to show evidence notes when found).
   - Create a **Decision Panel** with a Title (`Text - TextMeshPro`) and 4 `Button - TextMeshPro` underneath representing the answer options.
   - Add the **`UIManager`** script to the Canvas or GameManager. Assign all the UI references to the script fields (Progress Text, Message Panel, Message Text, Decision Panel, Result Text, and the 4 Option Buttons).

## 4. Setting up Clues (Evidence)
Create 4 to 5 objects around the room to serve as clues (e.g., a burnt fuse box, a tipped-over water bucket, a faulty wire).
For each object:
1. Make sure it has a Collider (e.g., BoxCollider).
2. Right-click the object > **XR > XR Simple Interactable** (if you just want players to point and click) OR **XR Grab Interactable** (if they should pick it up).
3. Attach the **`EvidenceItem`** script to the object.
   - Set the `Evidence Name`.
   - Set the `Investigation Notes` (e.g., "The safety switch was bypassed").
   - Optionally, assign the MeshRenderer and a Highlight Material (so the color changes when a player observes it).

## 5. Connecting Everything Using Unity Events
*Note: Our scripts handle most of the linkages automatically via Singletons (`InvestigationManager.Instance` and `UIManager.Instance`).*
1. Double-check your UI buttons. The `UIManager.cs` script will *automatically* assign exactly what each button does and its label text based on the `Cause Options` list in `InvestigationManager`.
2. Make sure the EventSystem in the hierarchy has an **XR UI Input Module** (if prompted, replace the Standalone Input Module by clicking the button in the inspector).

## 6. Testing & Demo Recording
1. Put on your VR Headset and hit **Play**.
2. Walk/teleport around and point your laser (or grab) the 4-5 evidence objects.
3. Watch the Progress UI update. Notice you cannot interact with the Final Decision panel until all items are found.
4. Go to the Decision panel, pick a wrong answer to see the error, and then pick the correct answer to complete it.
5. Record 2-3 minutes of this flow for your demo video submission!
