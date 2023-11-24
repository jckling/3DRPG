using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemTooltip : MonoBehaviour
{
    public TextMeshProUGUI itemName;
    public TextMeshProUGUI itemInfo;
    private RectTransform rectTransform;

    #region Event Functions

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    private void OnEnable()
    {
        LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);
        UpdatePosition();
    }

    private void Update()
    {
        UpdatePosition();
    }

    #endregion

    public void SetUpTooltip(ItemData_SO itemData)
    {
        itemName.text = itemData.itemName;
        itemInfo.text = itemData.description;
    }

    public void UpdatePosition()
    {
        // pivot center
        Vector3[] corners = new Vector3[4];
        rectTransform.GetWorldCorners(corners);
        float width = corners[3].x - corners[0].x;
        float height = corners[1].y - corners[0].y;

        // screen left bottom (0, 0)
        Vector3 mousePosition = Input.mousePosition;
        if (mousePosition.y < height)
        {
            rectTransform.position = mousePosition + Vector3.up * (height * 0.6f);
        }
        else if (Screen.width - mousePosition.x > width)
        {
            rectTransform.position = mousePosition + Vector3.right * (width * 0.6f);
        }
        else
        {
            rectTransform.position = mousePosition + Vector3.left * (width * 0.6f);
        }
    }
}