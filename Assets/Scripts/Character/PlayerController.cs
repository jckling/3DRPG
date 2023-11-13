using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class PlayerController : MonoBehaviour
{
    private NavMeshAgent agent;
    private Animator anim;
    private CharacterStats characterStats;
    private GameObject attackTarget;
    private float lastAttackTime;
    private bool isDead;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        characterStats = GetComponent<CharacterStats>();
    }

    private void Start()
    {
        MouseManager.Instance.OnMouseClicked += MoveToTarget;
        MouseManager.Instance.OnEnemyClicked += EventAttack;

        GameManager.Instance.RegisterPlayer(characterStats);
    }

    private void Update()
    {
        isDead = characterStats.CurrentHealth == 0;
        SwitchAnimation();

        lastAttackTime -= Time.deltaTime;
    }

    private void SwitchAnimation()
    {
        anim.SetFloat("Speed", agent.velocity.sqrMagnitude);
        anim.SetBool("Death", isDead);
    }

    private void MoveToTarget(Vector3 target)
    {
        StopAllCoroutines();
        agent.isStopped = false;
        agent.destination = target;
    }

    private void EventAttack(GameObject target)
    {
        if (target != null)
        {
            attackTarget = target;
            characterStats.isCritical = Random.value < characterStats.CriticalChance;
            StartCoroutine(MoveToAttackTarget());
        }
    }

    private IEnumerator MoveToAttackTarget()
    {
        // move
        agent.isStopped = false;
        transform.LookAt(attackTarget.transform);
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
        var targetStats = attackTarget.GetComponent<CharacterStats>();
        targetStats.TakeDamage(characterStats, targetStats);
    }
}