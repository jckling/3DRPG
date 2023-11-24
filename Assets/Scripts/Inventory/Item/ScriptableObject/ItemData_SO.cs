using UnityEngine;

public enum ItemType
{
    Weapon,
    Armor,
    Usable
}

[CreateAssetMenu(fileName = "New Item", menuName = "3D RPG/Item Data")]
public class ItemData_SO : ScriptableObject
{
    public ItemType itemType;
    public string itemName;
    public Sprite itemIcon;
    public int itemAmount;
    [TextArea] public string description = "";
    public bool stackable;

    [Header("Usable Item")] public UsableItemData_SO usableItemData;

    [Header("Weapon")] public GameObject weaponPrefab;
    public AttackData_SO weaponData;
    public AnimatorOverrideController weaponAnimator;
}