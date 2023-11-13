using UnityEngine;

[CreateAssetMenu(fileName = "New Data", menuName = "Character Stats/Character Data")]
public class CharacterData_SO : ScriptableObject
{
    [Header("Stats Info")] public int maxHealth;
    public int currentHealth;
    public int baseDefence;
    public int currentDefence;
}