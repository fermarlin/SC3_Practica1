
using UnityEngine;
using Unity.Behavior;

[RequireComponent(typeof(BehaviorGraphAgent))]
[RequireComponent(typeof(Health))]
public class EnemyData : MonoBehaviour
{
    [SerializeField] private EnemyConfigSO config;

    private BehaviorGraphAgent behaviorAgent;
    private Health health;

    public EnemyConfigSO Config => config;

    private void Awake()
    {
        behaviorAgent = GetComponent<BehaviorGraphAgent>();
        health = GetComponent<Health>();
    }

    private void Start()
    {
        InitializeEnemy();
    }

    private void InitializeEnemy()
    {

        health.Initialize(config.MaxHealth);

        SetFloat("DetectionRange", config.DetectionRange);
        SetFloat("AttackRange", config.AttackRange);
        SetFloat("MoveSpeed", config.MoveSpeed);
        SetFloat("AttackCooldown", config.AttackCooldown);
    }

    private void SetFloat(string variableName, float value)
    {
        bool success = behaviorAgent.SetVariableValue(
            variableName,
            value
        );


    }

    public GameObject GetTarget()
    {
        if (behaviorAgent.GetVariable(
                "Target",
                out BlackboardVariable variable)
            && variable is BlackboardVariable<GameObject> target)
        {
            return target.Value;
        }

        return null;
    }
}
