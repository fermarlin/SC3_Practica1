
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using Unity.Behavior;

[RequireComponent(typeof(Health))]
[RequireComponent(typeof(BehaviorGraphAgent))]
[RequireComponent(typeof(NavMeshAgent))]
public class EnemyDamageReactions : MonoBehaviour
{
    private Health health;
    private EnemyData enemyData;
    private EnemyAnimator enemyAnimator;
    private BehaviorGraphAgent behaviorAgent;
    private NavMeshAgent navAgent;

    private Coroutine hitCoroutine;
    private bool isDead;

    public bool IsStunned { get; private set; }

    [SerializeField] private ParticleSystem explosionPrefab;
    [SerializeField] private Vector3 explosionOffset = new Vector3(0f, 1f, 0f);

    private void Awake()
    {
        health = GetComponent<Health>();
        enemyData = GetComponent<EnemyData>();
        behaviorAgent = GetComponent<BehaviorGraphAgent>();
        navAgent = GetComponent<NavMeshAgent>();

        enemyAnimator = GetComponentInChildren<EnemyAnimator>(true);
    }

    private void OnEnable()
    {
        health.OnDamaged += HandleDamaged;
        health.OnDied += HandleDeath;


        if (enemyAnimator != null)
        {
            enemyAnimator.DeathAnimationFinished +=
                HandleDeathAnimationFinished;
        }
    }

    private void OnDisable()
    {
        health.OnDamaged -= HandleDamaged;
        health.OnDied -= HandleDeath;

        if (enemyAnimator != null)
        {
            enemyAnimator.DeathAnimationFinished -=
                HandleDeathAnimationFinished;
        }
    }

    private void HandleDamaged(int damage)
    {

    if (isDead)
        return;

        if (hitCoroutine != null)
        {
            StopCoroutine(hitCoroutine);
            hitCoroutine = null;
        }

        IsStunned = true;

        StopAI();

        if (enemyAnimator != null)
            enemyAnimator.PlayHit();

        float duration = 0.6f;

        if (enemyData != null && enemyData.Config != null)
            duration = enemyData.Config.HitStunDuration;

        hitCoroutine = StartCoroutine(
            RecoverAfterHit(duration)
        );
    }

    private IEnumerator RecoverAfterHit(float duration)
    {
        yield return new WaitForSeconds(duration);

        hitCoroutine = null;

        if (isDead)
            yield break;

        IsStunned = false;

        if (enemyAnimator != null)
            enemyAnimator.RecoverFromHit();

        if (navAgent != null &&
            navAgent.enabled &&
            navAgent.isOnNavMesh)
        {
            navAgent.isStopped = false;
        }

        if (behaviorAgent != null)
            behaviorAgent.enabled = true;
    }

    private void StopAI()
    {
        if (behaviorAgent != null)
            behaviorAgent.enabled = false;

        if (navAgent != null &&
            navAgent.enabled &&
            navAgent.isOnNavMesh)
        {
            navAgent.isStopped = true;
            navAgent.velocity = Vector3.zero;
        }
    }

    private void HandleDeath()
    {
        if (isDead)
            return;

        isDead = true;
        IsStunned = false;

        if (hitCoroutine != null)
        {
            StopCoroutine(hitCoroutine);
            hitCoroutine = null;
        }

        StopAI();

        if (navAgent != null && navAgent.enabled)
        {
            if (navAgent.isOnNavMesh)
                navAgent.ResetPath();

            navAgent.enabled = false;
        }

        if (enemyAnimator != null)
        {
            enemyAnimator.PlayDeath();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void HandleDeathAnimationFinished()
    {
        if (!isDead)
            return;

        if (explosionPrefab != null)
        {
            Instantiate(
                explosionPrefab,
                transform.position + explosionOffset,
                Quaternion.identity
            );
        }

        Destroy(gameObject);
    }
}
