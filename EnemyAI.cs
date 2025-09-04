using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Ilumisoft.VisualStateMachine;

public class EnemyAI : MonoBehaviour
{
    public StateMachine state;
    public Animator animator;
    public NavMeshAgent agent;

    [Header("FOV SETTINGS")]
    public float fovAngle;
    public float numberOfRays;
    public float rayDistance;
    public float rayHeight;

    [Header("AI SETTINGS")]
    public float wanderXradius;
    public float wanderZradius;
    public float attackDistance;
    public float attackDelay;

    Transform playerTransform;
    bool isWanderPointSet;
    bool isAttack;
    NavMeshHit hit;

    public void Wander()
    {
        agent.isStopped = false;
        MoveAnimation();
        if (isWanderPointSet)
        {
            agent.SetDestination(hit.position);
            if (Vector3.Distance(transform.position,hit.position) <= attackDistance)
            {
                isWanderPointSet = false;
            }
        }
        else
        {
            GetWanderPoint();
        }

        if (DetectPlayer())
        {
            state.TriggerByState("Chase");
        }
    }

    void GetWanderPoint()
    {
        float randomX       = transform.position.x + Random.Range(-wanderXradius, wanderXradius);
        float randomZ       = transform.position.z + Random.Range(-wanderZradius, wanderZradius);
        Vector3 randomPoint = new Vector3(randomX, 0, randomZ);
        NavMesh.SamplePosition(randomPoint, out hit, 5, NavMesh.AllAreas);
        isWanderPointSet = true;
    }

    public void Chase()
    {
        agent.isStopped = false;
        MoveAnimation();
        agent.SetDestination(playerTransform.position);
        if (Vector3.Distance(transform.position,playerTransform.position) <= attackDistance)
        {
            state.TriggerByState("Attack");
        }
    }

    public void Attack()
    {
        agent.isStopped = true;
        if (!isAttack)
        {
            animator.SetFloat("Blend", 0);
            animator.Play("Attack");
            isAttack = true;
        }
        else
        {
            state.TriggerByState("Attack Cooldown");
        }

    }

    public void AttackCooldown()
    {
        StartCoroutine(IEDelayAttack());
        IEnumerator IEDelayAttack()
        {
            yield return new WaitForSeconds(attackDelay);
            isAttack = false;
            if (playerTransform == null)
            {
                state.TriggerByState("Wander");
            }
            if (Vector3.Distance(transform.position,playerTransform.position) > attackDistance)
            {
                state.TriggerByState("Chase");
            }
            else
            {
                state.TriggerByState("Attack");
            }
        }
    }

    bool DetectPlayer()
    {
        float halfFOV   = fovAngle / 2f;
        float angleStep = fovAngle / (numberOfRays - 1);
        for (int i = 1; i < numberOfRays; i++)
        {
            float currentAngle  = -halfFOV + i * angleStep;
            Vector3 direction   = Quaternion.Euler(0, currentAngle, 0) * transform.forward;
            Vector3 rayPosition = new Vector3(transform.position.x, transform.position.y + rayHeight, transform.position.z);

            if (Physics.Raycast(rayPosition,direction,out RaycastHit hit,rayDistance))
            {
                if (hit.collider.CompareTag("Player"))
                {
                    playerTransform = hit.transform;
                    return true;
                }
            }
            Debug.DrawRay(rayPosition, direction * rayDistance, Color.red);
        }
        return false;
    }
    
    void MoveAnimation()
    {
        animator.SetFloat("Blend", agent.velocity.magnitude);
    }
}
