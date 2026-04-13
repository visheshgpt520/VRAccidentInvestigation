using System.Collections.Generic;
using UnityEngine;

public class InvestigationManager : MonoBehaviour
{
    public static InvestigationManager Instance { get; private set; }

    [SerializeField] private int minimumRequired = 4;
    private int identifiedCount = 0;

    public List<InvestigationPoint> allPoints = new List<InvestigationPoint>();

    public bool CanCompleteInvestigation => identifiedCount >= minimumRequired;

    private const string CORRECT_CAUSE = "Exposed electrical wire caused ignition of spilled oil";

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void OnPointIdentified(string id)
    {
        identifiedCount++;
        Debug.Log($"[Debug] Point Identified: {id}. Total identified: {identifiedCount}/{minimumRequired}");
        if (HUDController.Instance != null)
        {
            HUDController.Instance.UpdateProgress(identifiedCount, minimumRequired);
        }
    }

    public bool SubmitCause(string cause)
    {
        bool isCorrect = cause == CORRECT_CAUSE;
        Debug.Log($"[Debug] Cause submitted: '{cause}'. IsCorrect: {isCorrect}");
        return isCorrect;
    }

    public string GetProgress()
    {
        return $"{identifiedCount} / 5 evidence found";
    }
}
