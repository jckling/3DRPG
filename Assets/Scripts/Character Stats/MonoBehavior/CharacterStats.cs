using UnityEngine;
using Random = UnityEngine.Random;

public class CharacterStats : MonoBehaviour
{
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
        int damage = Mathf.Max(attacker.CurrentDamage() - defender.CurrentDamage(), 0);
        CurrentHealth = Mathf.Max(CurrentHealth - damage, 0);

        if (attacker.isCritical)
        {
            defender.GetComponent<Animator>().SetTrigger("Hit");
        }
    }

    private int CurrentDamage()
    {
        float coreDamage = Random.Range(attackData.minDamage, attackData.maxDamage);
        if (isCritical) coreDamage *= attackData.criticalMultiplier;
        return (int)coreDamage;
    }

    #endregion
}