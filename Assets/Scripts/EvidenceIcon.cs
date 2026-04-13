using UnityEngine;
using UnityEngine.UI;

public class EvidenceIcon : MonoBehaviour
{
    [SerializeField] private Image iconImage;
    [SerializeField] private Color defaultColor = Color.gray;
    [SerializeField] private Color foundColor = Color.green;

    private void Awake()
    {
        if (iconImage == null)
            iconImage = GetComponent<Image>();
            
        SetFound(false);
    }

    public void SetFound(bool isFound)
    {
        if (iconImage != null)
        {
            iconImage.color = isFound ? foundColor : defaultColor;
        }
    }
}
