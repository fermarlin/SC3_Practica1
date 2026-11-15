
using System;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;

using Action = Unity.Behavior.Action;

//Esta accion me permite activar o desactivar la animacion de ataque del enemigo desde el Behavior Graph
[Serializable, GeneratePropertyBag]
[NodeDescription(
    name: "Set Enemy Attacking",
    story: "Set [Enemy] attacking to [Attacking]",
    category: "Action/Animation",
    id: "57d6fcbeb32b42b6bfe5df8d2a79e124"
)]

public partial class SetEnemyAttackingAction : Action
{
    // Variable del Blackboard que guarda el enemigo que quiero controlar
    [SerializeReference]
    public BlackboardVariable<GameObject> Enemy;

    //Booleano que indica si quier activar o desactivar la animacion de ataque
    [SerializeReference]
    public BlackboardVariable<bool> Attacking;

    //Esta funcion se ejecuta cuando el Behavior Graph entra en este nodo
    protected override Status OnStart()
    {
        //Compruebo que las variables del Blackboard existen y que tengo un enemigo asignado
        if (Enemy == null || Attacking == null || Enemy.Value == null)
        {
            return Status.Failure;
        }

        //Busco el EnemyAnimator en los hijos porque tengo separado el modelo de la logica
        EnemyAnimator enemyAnimator = Enemy.Value.GetComponentInChildren<EnemyAnimator>(true);

        // Si no encuentro el componente no puedo modificar la animacion
        if (enemyAnimator == null)return Status.Failure;

        // Activo o desactivo el ataque dependiendo del booleano del Blackboard
        enemyAnimator.SetAttacking(Attacking.Value);

        //Devuelvo Success porque he ejecutado la accion
        return Status.Success;
    }
}
