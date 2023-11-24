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
[RequireComponent(typeof(CharacterStats))]
public class EnemyController : MonoBehaviour, IEndGameObserver
{
    protected CharacterStats characterStats;
    private EnemyStates enemyStates;
    private NavMeshAgent agent;
    private Animator anim;
    private Collider collider;

    [Header("Basic Settings")] public float sightRadius;
    public bool isGuard;
    public float lookAtTime;
    protected GameObject attackTarget;
    private float speed;
    private float remainLookAtTime;
    private float lastAttackTime;

    [Header("Patrol State")] public float patrolRange;
    private Vector3 wayPoint;
    private Vector3 guardPosition;
    private Quaternion guardRotation;

    // Animator Parameters
    private bool isWalk;
    private bool isChase;
    private bool isFollow;
    private bool isDead;
    private bool playerDead;

    #region Event Functions

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

        GameManager.Instance.AddObserver(this);
    }

    // private void OnEnable()
    // {
    //     GameManager.Instance.AddObserver(this);
    // }

    private void OnDisable()
    {
        if (!GameManager.isInitialized)
        {
            return;
        }

        GameManager.Instance.RemoveObserver(this);
        if (GetComponent<LootSpawner>() && isDead)
        {
            GetComponent<LootSpawner>().SpawnLoot();
        }
    }

    private void Update()
    {
        isDead = characterStats.CurrentHealth == 0;
        if (!playerDead)
        {
            SwitchState();
            SwitchAnimation();
            lastAttackTime -= Time.deltaTime;
        }
    }

    #endregion

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
            agent.stoppingDistance = characterStats.AttackRange;
            enemyStates = EnemyStates.CHASE;
        }
        else
        {
            agent.stoppingDistance = agent.radius;
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

    #region State Functions

    private void Guard()
    {
        isChase = false;
        agent.speed = speed * 0.5f;

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
        agent.radius = 0;
        Destroy(gameObject, 2f);
    }

    #endregion

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

        wayPoint = NavMesh.SamplePosition(randomPoint, out var hit, patrolRange, 1) ? hit.position : transform.position;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, sightRadius);
    }

    #region Animation Event

    void Hit()
    {
        if (attackTarget != null && transform.IsFacingTarget(attackTarget.transform))
        {
            var targetStats = attackTarget.GetComponent<CharacterStats>();
            targetStats.TakeDamage(characterStats, targetStats);
        }
    }

    #endregion

    public void EndNotify()
    {
        playerDead = true;
        anim.SetBool("Win", true);

        isWalk = false;
        isChase = false;
        isFollow = false;
        attackTarget = null;
    }
}