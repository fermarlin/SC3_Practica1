
using System;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Animator))]
public class EnemyAnimator : MonoBehaviour
{
    private static readonly int SpeedHash =
        Animator.StringToHash("SpeedMagnitude");

    private static readonly int AttackHash =
        Animator.StringToHash("Attack");

    private static readonly int HitHash =
        Animator.StringToHash("Hit");

    private static readonly int DieHash =
        Animator.StringToHash("Die");

    private Animator animator;
    private NavMeshAgent navAgent;

    private bool animationLocked;
    private bool isDead;

    public event Action DeathAnimationFinished;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        navAgent = GetComponentInParent<NavMeshAgent>();
    }

    private void Update()
    {
        UpdateMovement();
    }

    private void UpdateMovement()
    {
        float speed = 0f;

        if (!animationLocked &&
            navAgent != null &&
            navAgent.enabled &&
            navAgent.isOnNavMesh)
        {
            speed = navAgent.velocity.magnitude;
        }

        animator.SetFloat(SpeedHash, speed);
    }

    public void SetAttacking(bool attacking)
    {
        if (isDead || (animationLocked && attacking))
            return;

        animator.SetBool(AttackHash, attacking);
    }

    public void PlayHit()
    {
        if (isDead)
            return;

        animationLocked = true;

        animator.SetBool(AttackHash, false);
        animator.SetFloat(SpeedHash, 0f);

        animator.ResetTrigger(HitHash);
        animator.SetTrigger(HitHash);
    }

    public void RecoverFromHit()
    {
        if (!isDead)
            animationLocked = false;
    }

    public void PlayDeath()
    {
        if (isDead)
            return;

        isDead = true;
        animationLocked = true;

        animator.SetBool(AttackHash, false);
        animator.SetFloat(SpeedHash, 0f);

        animator.ResetTrigger(HitHash);
        animator.SetTrigger(DieHash);
    }

    public void OnDeathAnimationFinished()
    {
        DeathAnimationFinished?.Invoke();
    }
}
