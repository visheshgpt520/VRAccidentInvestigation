using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.UI;
using TMPro;
using System.Collections;

[RequireComponent(typeof(UnityEngine.XR.Interaction.Toolkit.Interactables.XRSimpleInteractable))]
public class InvestigationPoint : MonoBehaviour
{
    public string pointID;
    public string pointName;
    [TextArea] public string description;

    public bool isIdentified = false;

    [SerializeField] private GameObject highlightObject;
    [SerializeField] private GameObject questionMarkIcon;
    [SerializeField] private GameObject infoPopupCanvas;
    [SerializeField] private TextMeshProUGUI infoPopupText;

    private UnityEngine.XR.Interaction.Toolkit.Interactables.XRSimpleInteractable interactable;

    private void Awake()
    {
        interactable = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRSimpleInteractable>();
        if (infoPopupCanvas != null) infoPopupCanvas.SetActive(false);
    }

    private void Start()
    {
        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            Color pointColor = Color.white;
            switch (pointID)
            {
                case "A": pointColor = Color.red; break;
                case "B": pointColor = Color.blue; break;
                case "C": pointColor = Color.green; break;
                case "D": pointColor = Color.yellow; break;
                case "E": pointColor = new Color(1f, 0.5f, 0f); break; // Orange
            }
            
            // Create a material instance so we don't modify the shared default material
            Material mat = new Material(renderer.sharedMaterial);
            mat.color = pointColor;
            renderer.material = mat;
        }
    }

    private void OnEnable()
    {
        if (interactable != null)
        {
            interactable.selectEntered.AddListener(OnSelectEntered);
            interactable.hoverEntered.AddListener(OnHoverEntered);
        }
    }

    private void OnDisable()
    {
        if (interactable != null)
        {
            interactable.selectEntered.RemoveListener(OnSelectEntered);
            interactable.hoverEntered.RemoveListener(OnHoverEntered);
        }
    }

    private void OnSelectEntered(SelectEnterEventArgs args)
    {
        TriggerInteraction();
    }

    // Also allow 'Hover' to just trigger it if they are having a hard time clicking in the simulator!
    private void OnHoverEntered(HoverEnterEventArgs args)
    {
        TriggerInteraction();
    }

    public void TriggerInteraction()
    {
        if (isIdentified) return;

        Debug.Log($"<color=green>[Debug] Evidence interacted: {pointName} (ID: {pointID})</color>");

        isIdentified = true;

        if (highlightObject != null) highlightObject.SetActive(false);
        if (questionMarkIcon != null) questionMarkIcon.SetActive(false);
        
        if (interactable != null)
        {
            interactable.enabled = false;
        }

        ShowPopup();

        if (InvestigationManager.Instance != null)
        {
            InvestigationManager.Instance.OnPointIdentified(pointID);
        }
    }

    private void ShowPopup()
    {
        if (infoPopupCanvas != null && infoPopupText != null)
        {
            infoPopupText.text = description;
            infoPopupCanvas.SetActive(true);
            StartCoroutine(HidePopupRoutine());
        }
    }

    private IEnumerator HidePopupRoutine()
    {
        yield return new WaitForSeconds(5f); // Increased time to allow enough reading
        if (infoPopupCanvas != null)
        {
            infoPopupCanvas.SetActive(false);
        }
    }
}
