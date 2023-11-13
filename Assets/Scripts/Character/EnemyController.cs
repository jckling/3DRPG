using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public enum EnemyStates
{
    GUARD,
    PATROL,
    CHASE,
    DEAD
}

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyController : MonoBehaviour
{
    private EnemyStates enemyStates;
    private NavMeshAgent agent;
    private Animator anim;
    private CharacterStats characterStats;
    private Collider collider;

    [Header("Basic Settings")] public float sightRadius;
    public bool isGuard;
    private float speed;
    private GameObject attackTarget;
    public float lookAtTime;
    private float remainLookAtTime;
    private float lastAttackTime;

    [Header("Patrol State")] public float patrolRange;
    private Vector3 wayPoint;
    private Vector3 guardPosition;
    private Quaternion guardRotation;

    // animation
    private bool isWalk;
    private bool isChase;
    private bool isFollow;
    private bool isDead;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        characterStats = GetComponent<CharacterStats>();
        collider = GetComponent<Collider>();
        speed = agent.speed;
        guardPosition = transform.position;
        guardRotation = transform.rotation;
        remainLookAtTime = lookAtTime;
    }

    private void Start()
    {
        if (isGuard)
        {
            enemyStates = EnemyStates.GUARD;
        }
        else
        {
            enemyStates = EnemyStates.PATROL;
            GetNewWayPoint();
        }
    }

    private void Update()
    {
        isDead = characterStats.CurrentHealth == 0;
        SwitchState();
        SwitchAnimation();
        lastAttackTime -= Time.deltaTime;
    }

    private void SwitchAnimation()
    {
        anim.SetBool("Walk", isWalk);
        anim.SetBool("Chase", isChase);
        anim.SetBool("Follow", isFollow);
        anim.SetBool("Critical", characterStats.isCritical);
        anim.SetBool("Death", isDead);
    }

    private void SwitchState()
    {
        if (isDead)
        {
            enemyStates = EnemyStates.DEAD;
        }
        else if (FoundPlayer())
        {
            enemyStates = EnemyStates.CHASE;
        }

        switch (enemyStates)
        {
            case EnemyStates.GUARD:
                Guard();
                break;
            case EnemyStates.PATROL:
                Patrol();
                break;
            case EnemyStates.CHASE:
                Chase();
                break;
            case EnemyStates.DEAD:
                Die();
                break;
        }
    }

    private void Guard()
    {
        agent.speed = speed * 0.5f;
        isChase = false;
        if (transform.position != guardPosition)
        {
            isWalk = true;
            agent.isStopped = false;
            agent.destination = guardPosition;

            if (Vector3.SqrMagnitude(guardPosition - transform.position) <= agent.stoppingDistance)
            {
                isWalk = false;
                transform.rotation = Quaternion.Lerp(transform.rotation, guardRotation, 0.05f);
            }
        }
    }

    private void Patrol()
    {
        isChase = false;
        agent.speed = speed * 0.5f;
        if (Vector3.Distance(wayPoint, transform.position) <= agent.stoppingDistance)
        {
            isWalk = false;
            if (remainLookAtTime > 0)
            {
                remainLookAtTime -= Time.deltaTime;
            }
            else
            {
                GetNewWayPoint();
            }
        }
        else
        {
            isWalk = true;
            agent.destination = wayPoint;
        }
    }

    private void Chase()
    {
        isWalk = false;
        isChase = true;

        agent.speed = speed;
        if (!FoundPlayer())
        {
            isFollow = false;
            if (remainLookAtTime > 0)
            {
                agent.destination = transform.position;
                remainLookAtTime -= Time.deltaTime;
            }
            else
            {
                enemyStates = isGuard ? EnemyStates.GUARD : EnemyStates.PATROL;
            }
        }
        else
        {
            isFollow = true;
            agent.isStopped = false;
            agent.destination = attackTarget.transform.position;
        }

        // attack
        if (TargetInAttackRange() || TargetInSkillRange())
        {
            isFollow = false;
            agent.isStopped = true;
            if (lastAttackTime < 0)
            {
                lastAttackTime = characterStats.CoolDown;
                characterStats.isCritical = Random.value < characterStats.CriticalChance;
                Attack();
            }
        }
    }

    private void Attack()
    {
        transform.LookAt(attackTarget.transform);
        if (TargetInAttackRange())
        {
            anim.SetTrigger("Attack");
        }

        if (TargetInSkillRange())
        {
            anim.SetTrigger("Skill");
        }
    }

    private void Die()
    {
        collider.enabled = false;
        agent.enabled = false;
        Destroy(gameObject, 2f);
    }

    private bool FoundPlayer()
    {
        var hitColliders = Physics.OverlapSphere(transform.position, sightRadius);
        foreach (var target in hitColliders)
        {
            if (target.CompareTag("Player"))
            {
                attackTarget = target.gameObject;
                return true;
            }
        }

        attackTarget = null;
        return false;
    }

    private bool TargetInAttackRange()
    {
        return attackTarget != null && Vector3.Distance(attackTarget.transform.position, transform.position) <=
            characterStats.AttackRange;
    }

    private bool TargetInSkillRange()
    {
        return attackTarget != null && Vector3.Distance(attackTarget.transform.position, transform.position) <=
            characterStats.SkillRange;
    }

    private void GetNewWayPoint()
    {
        remainLookAtTime = lookAtTime;

        float randomX = Random.Range(-patrolRange, patrolRange);
        float randomZ = Random.Range(-patrolRange, patrolRange);
        Vector3 randomPoint = new Vector3(guardPosition.x + randomX, transform.position.y, guardPosition.z + randomZ);

        wayPoint = NavMesh.SamplePosition(randomPoint, out var hit, patrolRange, 1) ? randomPoint : transform.position;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, sightRadius);
    }

    // Animation Event
    void Hit()
    {
        if (attackTarget == null) return;
        var targetStats = attackTarget.GetComponent<CharacterStats>();
        targetStats.TakeDamage(characterStats, targetStats);
    }
}