using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class LootSpawner : MonoBehaviour
{
    [Serializable]
    public class LootItem
    {
        public GameObject item;
        [Range(0, 1)] public float weight;
    }

    public LootItem[] lootItems;

    public void SpawnLoot()
    {
        float currentValue = Random.value;
        for (int i = 0; i < lootItems.Length; i++)
        {
            if (currentValue <= lootItems[i].weight)
            {
                GameObject item = Instantiate(lootItems[i].item);
                item.transform.position = transform.position + Vector3.up * 1.5f;
            }
        }
    }
}