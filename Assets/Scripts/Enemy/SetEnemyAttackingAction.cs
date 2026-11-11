
using System;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;

using Action = Unity.Behavior.Action;

[Serializable, GeneratePropertyBag]
[NodeDescription(
    name: "Set Enemy Attacking",
    story: "Set [Enemy] attacking to [Attacking]",
    category: "Action/Animation",
    id: "57d6fcbeb32b42b6bfe5df8d2a79e124"
)]
public partial class SetEnemyAttackingAction : Action
{
    [SerializeReference]
    public BlackboardVariable<GameObject> Enemy;

    [SerializeReference]
    public BlackboardVariable<bool> Attacking;

    protected override Status OnStart()
    {
        if (Enemy == null ||
            Attacking == null ||
            Enemy.Value == null)
        {
            return Status.Failure;
        }

        EnemyAnimator enemyAnimator =
            Enemy.Value.GetComponentInChildren<EnemyAnimator>(true);

        if (enemyAnimator == null)
            return Status.Failure;

        enemyAnimator.SetAttacking(Attacking.Value);

        return Status.Success;
    }
}
