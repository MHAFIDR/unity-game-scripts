using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using Lean.Pool;
using Ilumisoft.VisualStateMachine;

public class PartnerAI : MonoBehaviour
{
    public StateMachine state;
    public Animator animator;
    public NavMeshAgent agent;
    public GameObject audioPrefabs;
    public AudioClip impactClip;
    public AudioClip gunshotClip;
    public GameObject impactEffect;
    public GameObject muzzleFlashFX;
    public Transform muzzleFXSpawn;

    [Header("Referensi Objek")]
    public Transform playerTransform;

    [Header("FOV SETTINGS")]
    public float fovAngle;
    public float numberOfRays;
    public float rayDistance;
    public float rayHeight;

    [Header("AI SETTINGS")]
    public float followDistance = 3f;
    public float attackDelay = 2f;
    public float damageAmount = 10f;

    private Transform enemyTarget;
    private HealthManager enemyHealth;
    private bool isAttack;

    void Start()
    {
        if (playerTransform == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
                playerTransform = player.transform;
        }
    }

    public void FollowPlayer()
    {
        if (playerTransform == null) return;
        agent.isStopped = false;
        MoveAnimation();
        
        if (Vector3.Distance(transform.position, playerTransform.position) > followDistance)
        {
            agent.SetDestination(playerTransform.position);
        }
        else
        {
            agent.isStopped = true;
        }

        if (DetectEnemy())
        {
            state.TriggerByState("Attack");
        }
    }

    public void Attack()
    {
        agent.isStopped = true;
        
        if (enemyTarget != null)
        {
            Vector3 direction = (enemyTarget.position - transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);

            if (!isAttack)
            {
                animator.SetFloat("Blend", 0);
                PerformShot();
                isAttack = true;
            }
            else
            {
                state.TriggerByState("Attack Cooldown");
            }
        }
        else
        {
            state.TriggerByState("Follow");
        }
    }

    void PerformShot()
    {
        if (enemyTarget == null) return;

        if (audioPrefabs != null && gunshotClip != null && muzzleFXSpawn != null)
        {
            GameObject gunshotAudioClone = LeanPool.Spawn(audioPrefabs, muzzleFXSpawn.position, Quaternion.identity);
            AudioSource audioSource = gunshotAudioClone.GetComponent<AudioSource>();
            if (audioSource != null)
            {
                audioSource.clip = gunshotClip;
                audioSource.Play();
            }
        }

        Vector3 shootDirection = (enemyTarget.position - transform.position).normalized;
        Vector3 shootOrigin = transform.position + Vector3.up * rayHeight;
        if (Physics.Raycast(shootOrigin, shootDirection, out RaycastHit hit, rayDistance))
        {
            if (hit.collider.CompareTag("Enemy"))
            {
                if (impactEffect != null)
                    LeanPool.Spawn(impactEffect, hit.point, Quaternion.LookRotation(hit.normal));
                if (muzzleFlashFX != null && muzzleFXSpawn != null)
                    LeanPool.Spawn(muzzleFlashFX, muzzleFXSpawn.position, transform.rotation);
                if (audioPrefabs != null && impactClip != null)
                {
                    GameObject impactAudioClone = LeanPool.Spawn(audioPrefabs, hit.point, Quaternion.identity);
                    AudioSource audioSourceClone = impactAudioClone.GetComponent<AudioSource>();
                    if (audioSourceClone != null)
                    {
                        audioSourceClone.clip = impactClip;
                        audioSourceClone.Play();
                    }
                }
                HealthManager targetHealth = hit.collider.GetComponent<HealthManager>();
                if (targetHealth != null)
                {
                    targetHealth.TakeDamage(damageAmount);
                }
            }
        }
    }

    public void AttackCooldown()
    {
        StartCoroutine(IEDelayAttack());
    }

    private IEnumerator IEDelayAttack()
    {
        yield return new WaitForSeconds(attackDelay);
        isAttack = false;
        
        if (enemyHealth == null || enemyHealth.currentHealth <= 0 || Vector3.Distance(transform.position, enemyTarget.position) > rayDistance)
        {
            enemyTarget = null;
            enemyHealth = null;
            state.TriggerByState("Follow");
        }
        else
        {
            state.TriggerByState("Attack");
        }
    }

    bool DetectEnemy()
    {
        float halfFOV = fovAngle / 2f;
        float angleStep = fovAngle / (numberOfRays - 1);
        
        for (int i = 0; i < numberOfRays; i++)
        {
            float currentAngle = -halfFOV + (i * angleStep);
            Vector3 direction = Quaternion.Euler(0, currentAngle, 0) * transform.forward;
            Vector3 rayPosition = transform.position + Vector3.up * rayHeight;

            if (Physics.Raycast(rayPosition, direction, out RaycastHit hit, rayDistance))
            {
                if (hit.collider.CompareTag("Enemy"))
                {
                    enemyTarget = hit.transform;
                    enemyHealth = hit.collider.GetComponent<HealthManager>();
                    if (enemyHealth != null && enemyHealth.currentHealth > 0)
                    {
                        return true;
                    }
                }
            }
            Debug.DrawRay(rayPosition, direction * rayDistance, Color.red);
        }
        
        enemyTarget = null;
        enemyHealth = null;
        return false;
    }
    
    void MoveAnimation()
    {
        if (animator != null)
        {
            animator.SetFloat("Blend", agent.velocity.magnitude);
        }
    }
}