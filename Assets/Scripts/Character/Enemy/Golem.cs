using UnityEngine;
using UnityEngine.AI;

public class Golem : EnemyController
{
    [Header("Skill")] public float kickForce = 30;
    public GameObject rockPrefab;
    public Transform handPos;

    public void Kickoff()
    {
        if (attackTarget == null || !transform.IsFacingTarget(attackTarget.transform)) return;
        var targetStats = attackTarget.GetComponent<CharacterStats>();

        transform.LookAt(attackTarget.transform);

        var direction = (attackTarget.transform.position - transform.position).normalized;
        // direction.Normalize();

        var agent = attackTarget.GetComponent<NavMeshAgent>();
        agent.isStopped = true;
        agent.velocity = direction * kickForce;

        var anim = attackTarget.GetComponent<Animator>();
        anim.SetTrigger("Dizzy");

        targetStats.TakeDamage(characterStats, targetStats);
    }

    public void ThrowRock()
    {
        if (attackTarget == null) return;
        var rock = Instantiate(rockPrefab, handPos.position, Quaternion.identity);
        rock.GetComponent<Rock>().target = attackTarget;
    }
}