
using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using UnityEngine.AI;

//Esta accion me permite detener o reactivar el movimiento de un enemigo desde el Behavior Graph
[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Set Agent Stopped", story: "Set [Enemy] navigation stopped to [Stopped]", category: "Action/Navigation", id: "518dff0ef9068f5ce69753da7a4e1b89")]
public partial class SetAgentStoppedAction : Action
{
    //Variable del Blackboard que guarda el enemigo que quiero controlar
    [SerializeReference] public BlackboardVariable<GameObject> Enemy;

    //Booleano que determina si quiero detener o reactivar su movimiento
    [SerializeReference] public BlackboardVariable<bool> Stopped;

    //Esta funcion se ejecuta cuando el Behavior Graph entra en este nodo
    protected override Status OnStart()
    {
        // Compruebo que las variables del Blackboard existen y que tengo un enemigo asignado
        if (Enemy == null || Stopped == null || Enemy.Value == null)
            return Status.Failure;;

        //Pillo el NavMeshAgent del enemigo
        NavMeshAgent agent = Enemy.Value.GetComponent<NavMeshAgent>();

        //Si no tiene NavMeshAgent, esta desactivado o no esta sobre el NavMesh, devuelvo Failure
        if (agent == null || !agent.enabled || !agent.isOnNavMesh)
        {
            return Status.Failure;
        }

        //Detengo o reactivo el movimiento dependiendo del booleano del Blackboard
        agent.isStopped = Stopped.Value;

        // Devuelvo Success porque he podido modificar el estado del agente
        return Status.Success;
    }

    //Esta funcion se llamaria mientras el nodo estuviera en ejecucio como hago todo en OnStart, simplemente devuelvo Success
    protected override Status OnUpdate()
    {
        return Status.Success;
    }

    //Esta funcion se ejecutaria al finalizar el nodo, no necesito hacer nada porque el estado del NavMeshAgent debe mantenerse
    protected override void OnEnd()
    {
    }
}
