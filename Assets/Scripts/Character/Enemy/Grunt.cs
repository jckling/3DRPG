using UnityEngine;
using UnityEngine.AI;

public class Grunt : EnemyController
{
    [Header("Skill")] public float kickForce = 20;

    public void Kickoff()
    {
        if (attackTarget == null) return;

        transform.LookAt(attackTarget.transform);

        var direction = attackTarget.transform.position - transform.position;
        direction.Normalize();

        var agent = attackTarget.GetComponent<NavMeshAgent>();
        agent.isStopped = true;
        agent.velocity = direction * kickForce;

        var anim = attackTarget.GetComponent<Animator>();
        anim.SetTrigger("Dizzy");
    }
}