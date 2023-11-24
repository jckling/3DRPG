using TMPro;
using UnityEngine;

public class InventoryManager : Singleton<InventoryManager>
{
    public class DragData
    {
        public SlotHolder originSlotHolder;
        public RectTransform originalParent;
    }

    [Header("Inventory Data")] public InventoryData_SO inventoryTemplate;
    public InventoryData_SO actionTemplate;
    public InventoryData_SO equipmentTemplate;

    // TODO: HideInInspector
    public InventoryData_SO inventoryData;
    public InventoryData_SO actionData;
    public InventoryData_SO equipmentData;

    [Header("Container")] public ContainerUI inventoryUI;
    public ContainerUI actionUI;
    public ContainerUI equipmentUI;

    [Header("Drag Canvas")] public Canvas dragCanvas;
    public DragData currentDrag;

    // panel
    public GameObject bagPanel;
    public GameObject statsPanel;
    private bool isBagOpen = false;
    private bool isStatsOpen = false;

    [Header("Stats")] public TextMeshProUGUI healthText;
    public TextMeshProUGUI attackText;

    [Header("Tooltip")] public ItemTooltip tooltip;

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
        inventoryUI.RefreshUI();
        actionUI.RefreshUI();
        equipmentUI.RefreshUI();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            isBagOpen = !isBagOpen;
            bagPanel.SetActive(isBagOpen);
        }
        else if (Input.GetKeyDown(KeyCode.C))
        {
            isStatsOpen = !isStatsOpen;
            statsPanel.SetActive(isStatsOpen);
        }

        // TODO: 切换武器时更新，进入游戏时设置，就不用在 Update 中一直更新
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
}