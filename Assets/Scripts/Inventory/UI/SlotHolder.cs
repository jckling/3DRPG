using UnityEngine;
using UnityEngine.EventSystems;

public enum SlotType
{
    Bag,
    Weapon,
    Armor,
    Action
}

public class SlotHolder : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    public SlotType slotType;
    public ItemUI itemUI;

    #region Event Functions

    private void OnDisable()
    {
        InventoryManager.Instance.tooltip.gameObject.SetActive(false);
    }

    #endregion

    #region Implement Interfaces

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.clickCount % 2 == 0)
        {
            UseItem();
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (itemUI.GetItem() && itemUI.gameObject.GetComponent<DragItem>() &&
            !itemUI.gameObject.GetComponent<DragItem>().isDragging)
        {
            InventoryManager.Instance.tooltip.SetUpTooltip(itemUI.GetItem());
            InventoryManager.Instance.tooltip.gameObject.SetActive(true);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        InventoryManager.Instance.tooltip.gameObject.SetActive(false);
    }

    #endregion

    public void UpdateItem()
    {
        switch (slotType)
        {
            case SlotType.Bag:
                itemUI.Bag = InventoryManager.Instance.inventoryData;
                break;
            case SlotType.Weapon:
                itemUI.Bag = InventoryManager.Instance.equipmentData;
                if (itemUI.Bag.items[itemUI.Index].itemData != null)
                {
                    GameManager.Instance.playerStats.ChangeWeapon(itemUI.Bag.items[itemUI.Index].itemData);
                }
                else
                {
                    GameManager.Instance.playerStats.UnEquipWeapon();
                }

                break;
            case SlotType.Armor:
                itemUI.Bag = InventoryManager.Instance.equipmentData;
                break;
            case SlotType.Action:
                itemUI.Bag = InventoryManager.Instance.actionData;
                break;
        }

        var item = itemUI.Bag.items[itemUI.Index];
        itemUI.SetUpItemUI(item.itemData, item.amount);
    }

    public void UseItem()
    {
        if (itemUI.GetItem() &&
            itemUI.GetItem().itemType == ItemType.Usable &&
            itemUI.Bag.items[itemUI.Index].amount > 0)
        {
            GameManager.Instance.playerStats.ApplyHealth(itemUI.GetItem().usableItemData.healthPoint);
            itemUI.Bag.items[itemUI.Index].amount--;
            QuestManager.Instance.UpdateQuestProgress(itemUI.GetItem().itemName, -1);
        }

        UpdateItem();
    }
}