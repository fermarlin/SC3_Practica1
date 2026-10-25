using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private float detectionRange = 10f;
    [SerializeField] private float attackRange = 2f;
    private EnemyStateMachine stateMachine;
    
    private void Update(){
        float distanceToPlayer = Vector3.Distance(transform.position,player.position);
        if(distanceToPlayer<attackRange){
            agent.isStopped=true;
            Debug.Log("El enemigo esta atacando");
        }
        else if(distanceToPlayer<detectionRange){
            agent.isStopped=false;
            agent.SetDestination(player.position);
        }else{
            agent.isStopped=true;
        }
    }
}
