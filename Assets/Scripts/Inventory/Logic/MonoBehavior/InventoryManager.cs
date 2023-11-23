using TMPro;
using UnityEngine;

public class InventoryManager : Singleton<InventoryManager>
{
    public class DragData
    {
        public SlotHolder originSlotHolder;
        public RectTransform originalParent;
    }

    [Header("Inventory Data")] public InventoryData_SO templateData;
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

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this);
    }

    private void Start()
    {
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

        // TODO: 切换武器时更新，进入游戏时设置
        var playerStats = GameManager.Instance.playerStats;
        UpdateStatsText(playerStats.MaxHealth, playerStats.MinDamage, playerStats.MaxDamage);
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