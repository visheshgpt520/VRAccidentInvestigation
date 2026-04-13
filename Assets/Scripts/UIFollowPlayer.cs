using UnityEngine;

public class UIFollowPlayer : MonoBehaviour
{
    [SerializeField] private Transform targetHead; 
    [SerializeField] private float smoothSpeed = 5f;
    [SerializeField] private float followDistance = 1.5f;
    [SerializeField] private float heightOffset = -0.2f;

    private void Update()
    {
        if (targetHead == null)
        {
            Camera mainCam = Camera.main;
            if (mainCam == null)
            {
                mainCam = FindObjectOfType<Camera>();
                if (mainCam != null) Debug.Log("[UIFollowPlayer] Found camera automatically: " + mainCam.name);
                else Debug.LogWarning("[UIFollowPlayer] Could not find ANY camera in the scene! UI will not move.");
            }
            if (mainCam != null) targetHead = mainCam.transform;
            return;
        }

        // Calculate target position
        Vector3 targetPosition = targetHead.position + new Vector3(targetHead.forward.x, 0, targetHead.forward.z).normalized * followDistance;
        targetPosition.y += heightOffset;

        // Smoothly move UI to target position
        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * smoothSpeed);

        // Make UI face player (but keep it level)
        Vector3 lookAtPoint = targetHead.position;
        lookAtPoint.y = transform.position.y; // Keep vertical axis straight
        transform.LookAt(lookAtPoint);
        
        // LookAt makes the forward vector point at the target, meaning the canvas will be reversed. Rotate 180 degrees.
        transform.Rotate(0, 180, 0);
    }
}
