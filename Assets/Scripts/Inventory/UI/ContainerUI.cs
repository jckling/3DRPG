using UnityEngine;

public class ContainerUI : MonoBehaviour
{
    public SlotHolder[] slotHolders;

    public void RefreshUI()
    {
        // 避免前面装备的武器，被后面空栏给卸载
        for (var i = slotHolders.Length - 1; i >= 0; i--)
        {
            slotHolders[i].itemUI.Index = i;
            slotHolders[i].UpdateItem();
        }
    }
}