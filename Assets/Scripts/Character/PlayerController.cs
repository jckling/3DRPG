using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class PlayerController : MonoBehaviour
{
    private NavMeshAgent agent;
    private Animator anim;
    private CharacterStats characterStats;
    private GameObject attackTarget;
    private bool isDead;
    private float lastAttackTime;
    private float stopDistance;
    private readonly float hitForce = 20;

    #region Event

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        characterStats = GetComponent<CharacterStats>();
        stopDistance = agent.stoppingDistance;
    }

    private void Start()
    {
        SaveManager.Instance.LoadPlayerData();
    }

    private void OnEnable()
    {
        MouseManager.Instance.OnMouseClicked += MoveToTarget;
        MouseManager.Instance.OnEnemyClicked += EventAttack;
        GameManager.Instance.RegisterPlayer(characterStats);
    }

    private void OnDisable()
    {
        if (!MouseManager.isInitialized)
        {
            return;
        }

        MouseManager.Instance.OnMouseClicked -= MoveToTarget;
        MouseManager.Instance.OnEnemyClicked -= EventAttack;
    }

    private void Update()
    {
        isDead = characterStats.CurrentHealth == 0;

        if (isDead)
        {
            GameManager.Instance.NotifyObservers();
        }

        SwitchAnimation();

        lastAttackTime -= Time.deltaTime;
    }

    #endregion

    private void SwitchAnimation()
    {
        anim.SetFloat("Speed", agent.velocity.sqrMagnitude);
        anim.SetBool("Death", isDead);
    }

    private void MoveToTarget(Vector3 target)
    {
        StopAllCoroutines();
        if (isDead)
        {
            return;
        }

        agent.stoppingDistance = stopDistance;
        agent.isStopped = false;
        agent.destination = target;
    }

    private void EventAttack(GameObject target)
    {
        if (isDead || target == null)
        {
            return;
        }

        attackTarget = target;
        characterStats.isCritical = Random.value < characterStats.CriticalChance;
        StartCoroutine(MoveToAttackTarget());
    }

    private IEnumerator MoveToAttackTarget()
    {
        // move
        agent.isStopped = false;
        agent.stoppingDistance = characterStats.AttackRange;

        transform.LookAt(attackTarget.transform);

        if (attackTarget == null)
        {
            yield break;
        }

        while (Vector3.Distance(attackTarget.transform.position, transform.position) > characterStats.AttackRange)
        {
            agent.destination = attackTarget.transform.position;
            yield return null;
        }

        // attack
        agent.isStopped = true;
        if (lastAttackTime < 0)
        {
            anim.SetTrigger("Attack");
            anim.SetBool("Critical", characterStats.isCritical);
            lastAttackTime = characterStats.CoolDown;
        }
    }

    // Animation Event
    void Hit()
    {
        if (attackTarget.CompareTag("Attackable"))
        {
            if (attackTarget.GetComponent<Rock>() &&
                attackTarget.GetComponent<Rock>().rockStates == Rock.RockStates.HitNothing)
            {
                attackTarget.GetComponent<Rock>().rockStates = Rock.RockStates.HitEnemy;
                attackTarget.GetComponent<Rigidbody>().velocity = Vector3.one;
                attackTarget.GetComponent<Rigidbody>().AddForce(transform.forward * hitForce, ForceMode.Impulse);
            }
        }
        else
        {
            var targetStats = attackTarget.GetComponent<CharacterStats>();
            targetStats.TakeDamage(characterStats, targetStats);
        }
    }
}