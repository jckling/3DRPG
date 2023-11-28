using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(ItemUI))]
public class DragItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private ItemUI currentItemUI;
    private SlotHolder currentHolder;
    private SlotHolder targetHolder;
    public bool isDragging = false;

    #region Event Functions

    private void Awake()
    {
        currentItemUI = GetComponent<ItemUI>();
        currentHolder = GetComponentInParent<SlotHolder>();
    }

    #endregion

    #region Impelement Interfaces

    public void OnBeginDrag(PointerEventData eventData)
    {
        InventoryManager.Instance.currentDrag = new InventoryManager.DragData
        {
            originSlotHolder = GetComponentInParent<SlotHolder>(),
            originalParent = (RectTransform)transform.parent
        };

        transform.SetParent(InventoryManager.Instance.dragCanvas.transform, true);
        isDragging = true;
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (EventSystem.current.IsPointerOverGameObject())
        {
            if (InventoryManager.Instance.CheckInInventoryUI(eventData.position) ||
                InventoryManager.Instance.CheckInActionUI(eventData.position) ||
                InventoryManager.Instance.CheckInEquipmentUI(eventData.position))
            {
                if (eventData.pointerEnter.gameObject.GetComponent<SlotHolder>())
                {
                    targetHolder = eventData.pointerEnter.gameObject.GetComponent<SlotHolder>();
                }
                else
                {
                    targetHolder = eventData.pointerEnter.gameObject.GetComponentInParent<SlotHolder>();
                }

                if (targetHolder != InventoryManager.Instance.currentDrag.originSlotHolder)
                {
                    switch (targetHolder.slotType)
                    {
                        case SlotType.Bag:
                            SwapItem();
                            break;
                        case SlotType.Weapon:
                            if (currentHolder.itemUI.Bag.items[currentHolder.itemUI.Index].itemData.itemType ==
                                ItemType.Weapon)
                            {
                                SwapItem();
                            }

                            break;
                        case SlotType.Armor:
                            if (currentHolder.itemUI.Bag.items[currentHolder.itemUI.Index].itemData.itemType ==
                                ItemType.Armor)
                            {
                                SwapItem();
                            }

                            break;
                        case SlotType.Action:
                            if (currentHolder.itemUI.Bag.items[currentHolder.itemUI.Index].itemData.itemType ==
                                ItemType.Usable)
                            {
                                SwapItem();
                            }

                            break;
                    }
                }

                currentHolder.UpdateItem();
                targetHolder.UpdateItem();
            }
        }
        else
        {
            // TODO: 丢弃到地面或销毁
        }

        transform.SetParent(InventoryManager.Instance.currentDrag.originalParent);
        RectTransform rectTransform = transform as RectTransform;
        rectTransform.offsetMax = -Vector2.one * 6;
        rectTransform.offsetMin = Vector2.one * 6;

        isDragging = false;
    }

    #endregion

    private void SwapItem()
    {
        var targetItem = targetHolder.itemUI.Bag.items[targetHolder.itemUI.Index];
        var tempItem = currentHolder.itemUI.Bag.items[currentHolder.itemUI.Index];
        bool isSameItem = tempItem.itemData == targetItem.itemData;

        if (isSameItem && targetItem.itemData.stackable)
        {
            targetItem.amount += tempItem.amount;
            tempItem.itemData = null;
            tempItem.amount = 0;
        }
        else
        {
            currentHolder.itemUI.Bag.items[currentHolder.itemUI.Index] = targetItem;
            targetHolder.itemUI.Bag.items[targetHolder.itemUI.Index] = tempItem;
        }
    }
}