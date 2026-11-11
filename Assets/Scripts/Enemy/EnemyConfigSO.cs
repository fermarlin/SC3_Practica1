
using UnityEngine;

[CreateAssetMenu(fileName = "NewEnemy", menuName = "Game/Enemy Config")]
public class EnemyConfigSO : ScriptableObject
{
    [Header("Stats")]
    [Min(1)] public int MaxHealth = 100;
    [Min(0)] public int AttackDamage = 10;

    [Header("Detection")]
    [Min(0)] public float DetectionRange = 10f;
    [Min(0)] public float AttackRange = 1.8f;

    [Header("Movement")]
    [Min(0)] public float MoveSpeed = 3.5f;

    [Header("Combat")]
    [Min(0)] public float AttackCooldown = 1.5f;
    [Min(0.1f)] public float HitRadius = 0.6f;

    [Header("Reactions")]
    [Min(0)] public float HitStunDuration = 0.6f;

    [Header("Rotation")]
    [Min(0f)] public float AttackTurnSpeed = 180f;
}
