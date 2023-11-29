using TMPro;
using UnityEngine;

public class InventoryManager : Singleton<InventoryManager>
{
    public class DragData
    {
        public SlotHolder originSlotHolder;
        public RectTransform originalParent;
    }

    // Template Data
    [Header("Inventory Data")] public InventoryData_SO inventoryTemplate;
    public InventoryData_SO actionTemplate;
    public InventoryData_SO equipmentTemplate;

    // Inventory Data
    [HideInInspector] public InventoryData_SO inventoryData;
    [HideInInspector] public InventoryData_SO actionData;
    [HideInInspector] public InventoryData_SO equipmentData;

    // Containers
    [Header("Container")] public ContainerUI inventoryUI;
    public ContainerUI actionUI;
    public ContainerUI equipmentUI;

    // Drag
    [Header("Drag Canvas")] public Canvas dragCanvas;
    public DragData currentDrag;

    // Panel
    public GameObject bagPanel;
    public GameObject statsPanel;
    private bool isBagOpen = false;
    private bool isStatsOpen = false;

    // Player Stats Text
    [Header("Stats")] public TextMeshProUGUI healthText;
    public TextMeshProUGUI attackText;

    // Tooltip
    [Header("Tooltip")] public ItemTooltip tooltip;

    #region Event Functions

    protected override void Awake()
    {
        base.Awake();

        if (inventoryTemplate != null)
        {
            inventoryData = Instantiate(inventoryTemplate);
        }

        if (actionTemplate != null)
        {
            actionData = Instantiate(actionTemplate);
        }

        if (equipmentTemplate != null)
        {
            equipmentData = Instantiate(equipmentTemplate);
        }
    }

    private void Start()
    {
        LoadData();
        RefreshUI();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            isBagOpen = !isBagOpen;
            bagPanel.SetActive(isBagOpen);
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            isStatsOpen = !isStatsOpen;
            statsPanel.SetActive(isStatsOpen);
        }
    }

    #endregion

    public void RefreshUI()
    {
        inventoryUI.RefreshUI();
        actionUI.RefreshUI();
        equipmentUI.RefreshUI();

        var playerStats = GameManager.Instance.playerStats;
        UpdateStatsText(playerStats.MaxHealth, playerStats.MinDamage, playerStats.MaxDamage);
    }

    public void SaveData()
    {
        SaveManager.Instance.Save(inventoryData, inventoryData.name);
        SaveManager.Instance.Save(actionData, actionData.name);
        SaveManager.Instance.Save(equipmentData, equipmentData.name);
    }

    public void LoadData()
    {
        SaveManager.Instance.Load(inventoryData, inventoryData.name);
        SaveManager.Instance.Load(actionData, actionData.name);
        SaveManager.Instance.Load(equipmentData, equipmentData.name);
    }

    public void UpdateStatsText(int health, int min, int max)
    {
        healthText.text = health.ToString();
        attackText.text = min + " - " + max;
    }

    #region Drag

    public bool CheckInInventoryUI(Vector3 position)
    {
        foreach (var slotHolder in inventoryUI.slotHolders)
        {
            RectTransform rectTransform = slotHolder.transform as RectTransform;
            if (RectTransformUtility.RectangleContainsScreenPoint(rectTransform, position))
            {
                return true;
            }
        }

        return false;
    }

    public bool CheckInActionUI(Vector3 position)
    {
        foreach (var slotHolder in actionUI.slotHolders)
        {
            RectTransform rectTransform = slotHolder.transform as RectTransform;
            if (RectTransformUtility.RectangleContainsScreenPoint(rectTransform, position))
            {
                return true;
            }
        }

        return false;
    }

    public bool CheckInEquipmentUI(Vector3 position)
    {
        foreach (var slotHolder in equipmentUI.slotHolders)
        {
            RectTransform rectTransform = slotHolder.transform as RectTransform;
            if (RectTransformUtility.RectangleContainsScreenPoint(rectTransform, position))
            {
                return true;
            }
        }

        return false;
    }

    #endregion

    #region Quest

    public void CheckQuestItem(string questItemName)
    {
        foreach (var item in inventoryData.items)
        {
            if (item.itemData != null)
            {
                if (item.itemData.itemName == questItemName)
                {
                    QuestManager.Instance.UpdateQuestProgress(item.itemData.itemName, item.amount);
                }
            }
        }

        foreach (var item in actionData.items)
        {
            if (item.itemData != null)
            {
                if (item.itemData.itemName == questItemName)
                {
                    QuestManager.Instance.UpdateQuestProgress(item.itemData.itemName, item.itemData.itemAmount);
                }
            }
        }
    }

    public InventoryItem QuestItemInBag(ItemData_SO questItem)
    {
        return inventoryData.items.Find(i => i.itemData == questItem);
    }

    public InventoryItem QuestItemInAction(ItemData_SO questItem)
    {
        return actionData.items.Find(i => i.itemData == questItem);
    }

    #endregion
}