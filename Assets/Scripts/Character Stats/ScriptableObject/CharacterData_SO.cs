using UnityEngine;

[CreateAssetMenu(fileName = "New Data", menuName = "3D RPG/Character Data")]
public class CharacterData_SO : ScriptableObject
{
    [Header("Stats Info")] public int maxHealth;
    public int currentHealth;
    public int baseDefence;
    public int currentDefence;

    [Header("Kill Point")] public int killPoint;

    [Header("Level")] public int currentLevel;
    public int maxLevel;
    public int baseExp;
    public int currentExp;
    public float levelBuff;
    public float levelMultiplier => 1 + (currentLevel - 1) * levelBuff;

    public void UpdateExp(int point)
    {
        currentExp += point;
        if (currentExp >= baseExp)
        {
            LevelUp();
        }
    }

    private void LevelUp()
    {
        currentLevel = Mathf.Clamp(currentLevel + 1, 0, maxLevel);
        baseExp += (int)(baseExp * levelMultiplier);
        maxHealth = (int)(maxHealth * levelBuff);
        currentHealth = maxHealth;
    }
}