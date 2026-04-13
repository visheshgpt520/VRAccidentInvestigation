using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class HUDController : MonoBehaviour
{
    public static HUDController Instance { get; private set; }

    [SerializeField] private TextMeshProUGUI progressText;
    [SerializeField] private List<EvidenceIcon> evidenceIcons;
    [SerializeField] private Button completeInvestigateButton;
    [SerializeField] private GameObject tooltipBox;
    [SerializeField] private GameObject causeSelectionPanel;

    private int maxEvidence = 5;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        if (completeInvestigateButton != null)
        {
            completeInvestigateButton.onClick.AddListener(OnCompleteButtonClicked);
        }
        UpdateProgress(0, 4); // Initialize UI state
        
        if (tooltipBox != null) tooltipBox.SetActive(false);
        if (causeSelectionPanel != null) causeSelectionPanel.SetActive(false);
    }

    public void UpdateProgress(int count, int required)
    {
        Debug.Log($"[Debug] Updating HUD Progress: {count}/{required}");
        if (progressText != null)
        {
            progressText.text = $"Evidence Found: {count} / {maxEvidence}";
        }

        for (int i = 0; i < evidenceIcons.Count; i++)
        {
            if (i < count)
            {
                if (evidenceIcons[i] != null) evidenceIcons[i].SetFound(true);
            }
        }

        if (count >= required)
        {
            Debug.Log("[Debug] Minimum required evidence found. Complete Button Enabled.");
            if (completeInvestigateButton != null) completeInvestigateButton.interactable = true;
            if (tooltipBox != null) tooltipBox.SetActive(false);
        }
        else
        {
            if (completeInvestigateButton != null) completeInvestigateButton.interactable = false;
        }
    }

    public void TryShowTooltip()
    {
        if (InvestigationManager.Instance != null && !InvestigationManager.Instance.CanCompleteInvestigation)
        {
            Debug.Log("[Debug] Prevented early completion. Showing Tooltip.");
            if (tooltipBox != null) tooltipBox.SetActive(true);
            CancelInvoke(nameof(HideTooltip));
            Invoke(nameof(HideTooltip), 3f); // Increased to 3s
        }
    }

    private void HideTooltip()
    {
        if (tooltipBox != null) tooltipBox.SetActive(false);
    }

    private void OnCompleteButtonClicked()
    {
        if (InvestigationManager.Instance != null && InvestigationManager.Instance.CanCompleteInvestigation)
        {
            Debug.Log("[Debug] Opening Cause Selection Panel");
            if (causeSelectionPanel != null)
            {
                causeSelectionPanel.SetActive(true);
            }
        }
    }
}
