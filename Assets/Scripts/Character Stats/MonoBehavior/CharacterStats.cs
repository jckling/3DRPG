using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class CharacterStats : MonoBehaviour
{
    public event Action<int, int> UpdateHealthBar;
    public CharacterData_SO templateData;
    public CharacterData_SO characterData;
    public AttackData_SO attackData;
    [HideInInspector] public bool isCritical;

    private void Awake()
    {
        if (templateData != null)
        {
            characterData = Instantiate(templateData);
        }
    }

    #region Read from CharacterData_SO

    public int MaxHealth
    {
        get => characterData != null ? characterData.maxHealth : 0;
        set => characterData.maxHealth = value;
    }

    public int CurrentHealth
    {
        get => characterData != null ? characterData.currentHealth : 0;
        set => characterData.currentHealth = value;
    }

    public int BaseDefence
    {
        get => characterData != null ? characterData.baseDefence : 0;
        set => characterData.baseDefence = value;
    }

    public int CurrentDefence
    {
        get => characterData != null ? characterData.currentDefence : 0;
        set => characterData.currentDefence = value;
    }

    public int KillPoint
    {
        get => characterData != null ? characterData.killPoint : 0;
        set => characterData.currentDefence = value;
    }

    public int CurrentLevel
    {
        get => characterData != null ? characterData.currentLevel : 0;
        set => characterData.currentLevel = value;
    }

    public int MaxLevel
    {
        get => characterData != null ? characterData.maxLevel : 0;
        set => characterData.maxLevel = value;
    }

    public int BaseExp
    {
        get => characterData != null ? characterData.baseExp : 0;
        set => characterData.baseExp = value;
    }

    public int CurrentExp
    {
        get => characterData != null ? characterData.currentExp : 0;
        set => characterData.currentExp = value;
    }

    public float LevelBuff
    {
        get => characterData != null ? characterData.levelBuff : 0;
        set => characterData.levelBuff = value;
    }

    #endregion

    #region Read from AttackData_SO

    public float AttackRange => attackData != null ? attackData.attackRange : 0;
    public float SkillRange => attackData != null ? attackData.skillRange : 0;
    public float CoolDown => attackData != null ? attackData.coolDown : 0;
    public int MinDamage => attackData != null ? attackData.minDamage : 0;
    public int MaxDamage => attackData != null ? attackData.maxDamage : 0;
    public float CriticalMultiplier => attackData != null ? attackData.criticalMultiplier : 0;
    public float CriticalChance => attackData != null ? attackData.criticalChance : 0;

    #endregion

    #region Combat

    public void TakeDamage(CharacterStats attacker, CharacterStats defender)
    {
        int damage = Mathf.Max(attacker.CurrentDamage() - defender.CurrentDefence, 0);
        CurrentHealth = Mathf.Max(CurrentHealth - damage, 0);

        if (attacker.isCritical)
        {
            defender.GetComponent<Animator>().SetTrigger("Hit");
        }

        UpdateHealthBar?.Invoke(CurrentHealth, MaxHealth);
        if (CurrentHealth <= 0)
        {
            attacker.characterData.UpdateExp(characterData.killPoint);
        }
    }

    public void TakeDamage(int damage, CharacterStats defender)
    {
        int currentDamage = Mathf.Max(damage - defender.CurrentDefence, 0);
        CurrentHealth = Mathf.Max(CurrentHealth - currentDamage, 0);

        UpdateHealthBar?.Invoke(CurrentHealth, MaxHealth);
        if (CurrentHealth <= 0)
        {
            GameManager.Instance.playerStats.characterData.UpdateExp(characterData.killPoint);
        }
    }

    private int CurrentDamage()
    {
        float coreDamage = Random.Range(attackData.minDamage, attackData.maxDamage);
        if (isCritical)
        {
            coreDamage *= attackData.criticalMultiplier;
        }

        return (int)coreDamage;
    }

    #endregion
}