using UnityEngine;
using UnityEngine.EventSystems;

public class RewardTooltip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private ItemUI currentItemUI;

    #region Event Functions

    private void Awake()
    {
        currentItemUI = GetComponent<ItemUI>();
    }

    #endregion

    #region Implement Interfaces

    public void OnPointerEnter(PointerEventData eventData)
    {
        QuestUI.Instance.tooltip.gameObject.SetActive(true);
        QuestUI.Instance.tooltip.SetUpTooltip(currentItemUI.currentItemData);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        QuestUI.Instance.tooltip.gameObject.SetActive(false);
    }

    #endregion
}