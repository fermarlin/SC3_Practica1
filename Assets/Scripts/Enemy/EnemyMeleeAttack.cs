
using System.Collections.Generic;
using UnityEngine;

public class EnemyMeleeAttack : MonoBehaviour
{
    [SerializeField] private Transform meleeHitPoint;
    [SerializeField] private LayerMask targetLayers = ~0;

    private EnemyData enemyData;
    private EnemyDamageReactions reactions;
    private Health health;

    private void Awake()
    {
        enemyData = GetComponentInParent<EnemyData>();
        reactions = GetComponentInParent<EnemyDamageReactions>();
        health = GetComponentInParent<Health>();
    }

    public void OnAttackHit()
    {
        if (enemyData == null || enemyData.Config == null)
            return;

        if (meleeHitPoint == null)
            return;

        if (health != null && health.IsDead)
            return;

        if (reactions != null && reactions.IsStunned)
            return;

        Collider[] hits = Physics.OverlapSphere(
            meleeHitPoint.position,
            enemyData.Config.HitRadius,
            targetLayers,
            QueryTriggerInteraction.Ignore
        );

        HashSet<IDamageable> damagedTargets = new();

        foreach (Collider hit in hits)
        {
            if (hit.transform.IsChildOf(enemyData.transform))
                continue;

            IDamageable damageable =
                hit.GetComponentInParent<IDamageable>();

            if (damageable == null)
                continue;

            if (!damagedTargets.Add(damageable))
                continue;

            damageable.TakeDamage(
                enemyData.Config.AttackDamage
            );
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (meleeHitPoint == null)
            return;

        EnemyData data = GetComponentInParent<EnemyData>();

        if (data == null || data.Config == null)
            return;

        Gizmos.color = Color.red;

        Gizmos.DrawWireSphere(
            meleeHitPoint.position,
            data.Config.HitRadius
        );
    }
}
