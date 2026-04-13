using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CauseSelectionPanel : MonoBehaviour
{
    [System.Serializable]
    public struct CauseOption
    {
        public Button button;
        public string causeText;
    }

    [SerializeField] private CauseOption[] causeOptions;

    [SerializeField] private GameObject resultPanel;
    [SerializeField] private TextMeshProUGUI resultText;
    [SerializeField] private Image resultPanelBackground;
    [SerializeField] private Color correctColor = new Color(0.2f, 0.8f, 0.2f, 1f); // Green ish
    [SerializeField] private Color incorrectColor = new Color(0.8f, 0.2f, 0.2f, 1f); // Red ish

    private void Start()
    {
        foreach (var option in causeOptions)
        {
            // Capture loop variable
            string cText = option.causeText;
            if (option.button != null)
            {
                option.button.onClick.AddListener(() => OnCauseSelected(cText));
            }
        }
        if (resultPanel != null) resultPanel.SetActive(false);
    }

    private void OnCauseSelected(string cause)
    {
        Debug.Log($"[Debug] Selected Cause: '{cause}'");
        if (InvestigationManager.Instance == null) return;

        bool isCorrect = InvestigationManager.Instance.SubmitCause(cause);

        if (isCorrect)
        {
            Debug.Log("[Debug] User successfully completed the investigation.");
            ShowResult(true, "Correct! Investigation Complete. Root cause identified.");
            if (AudioManager.Instance != null) AudioManager.Instance.PlaySuccess();
            DisableAllButtons();
        }
        else
        {
            Debug.Log("[Debug] User failed the investigation. Allowing retry after delay.");
            ShowResult(false, "Incorrect. Re-examine the evidence and try again.");
            if (AudioManager.Instance != null) AudioManager.Instance.PlayError();
            DisableAllButtons();
            Invoke(nameof(ResetPanel), 3f);
        }
    }

    private void ShowResult(bool correct, string message)
    {
        if (resultPanel != null)
        {
            resultPanel.SetActive(true);
        }

        if (resultText != null)
        {
            resultText.text = message;
        }

        if (resultPanelBackground != null)
        {
            resultPanelBackground.color = correct ? correctColor : incorrectColor;
        }
    }

    private void ResetPanel()
    {
        Debug.Log("[Debug] Retrying Cause Selection...");
        if (resultPanel != null) resultPanel.SetActive(false);
        EnableAllButtons();
    }

    private void DisableAllButtons()
    {
        foreach (var option in causeOptions)
        {
            if (option.button != null) option.button.interactable = false;
        }
    }

    private void EnableAllButtons()
    {
        foreach (var option in causeOptions)
        {
            if (option.button != null) option.button.interactable = true;
        }
    }
}
