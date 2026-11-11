using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using UnityEngine.AI;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Set Agent Stopped", story: "Set [Enemy] navigation stopped to [Stopped]", category: "Action/Navigation", id: "518dff0ef9068f5ce69753da7a4e1b89")]
public partial class SetAgentStoppedAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Enemy;
    [SerializeReference] public BlackboardVariable<bool> Stopped;


    protected override Status OnStart()
    {
        if (Enemy == null || Stopped == null || Enemy.Value == null)
        return Status.Failure;
        
        if (Enemy.Value == null)
            return Status.Failure;

        NavMeshAgent agent = Enemy.Value.GetComponent<NavMeshAgent>();

        if (agent == null || !agent.enabled || !agent.isOnNavMesh)
        {
            return Status.Failure;
        }

        agent.isStopped = Stopped.Value;

        return Status.Success;
    }


    protected override Status OnUpdate()
    {
        return Status.Success;
    }

    protected override void OnEnd()
    {
    }
}

