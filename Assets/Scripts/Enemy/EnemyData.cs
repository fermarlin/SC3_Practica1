
using UnityEngine;
using Unity.Behavior;

//Este script se encarga de pasar los datos de configuracion del enemigo a su IA, asi puedo utilizar el mismo Behavior Graph con distintos tipos de enemigos
public class EnemyData : MonoBehaviour
{
    //ScriptableObject donde guardo las estadisticas del enemigo
    [SerializeField] private EnemyConfigSO config;

    // Componentes que necesito para configurar la IA y la vida del enemigo
    private BehaviorGraphAgent behaviorAgent;
    private Health health;

    //Permito que otros scripts puedan acceder a la configuracion del enemigo
    public EnemyConfigSO Config => config;

    private void Awake()
    {
        //Pillo los componentes del enemigo
        behaviorAgent = GetComponent<BehaviorGraphAgent>();
        health = GetComponent<Health>();
    }

    private void Start()
    {
        //Inicializo al enemigo con los valores de su ScriptableObject
        InitializeEnemy();
    }

    // Esta funcion se encarga de configurar la vida y las variables del Behavior Graph
    private void InitializeEnemy()
    {
        //Establezco la vida maxima del enemigo
        health.Initialize(config.MaxHealth);

        //Paso los valores de configuracion al Blackboard de la IA
        SetFloat("DetectionRange", config.DetectionRange);
        SetFloat("AttackRange", config.AttackRange);
        SetFloat("MoveSpeed", config.MoveSpeed);
        SetFloat("AttackCooldown", config.AttackCooldown);
    }

    //Esta funcion me permite cambiar el valor de una variable float del Behavior Graph
    private void SetFloat(string variableName, float value)
    {
        //Busco la variable por su nombre y le asigno el valor correspondiente
        behaviorAgent.SetVariableValue(variableName, value);
    }

    //Esta funcion permite que otros scripts conozcan el objetivo actual del enemigo
    public GameObject GetTarget()
    {
        //Busco la variable Target en el Blackboard, compruebo que existe y que guarda un GameObject
        if (behaviorAgent.GetVariable("Target", out BlackboardVariable variable) && variable is BlackboardVariable<GameObject> target)
        {
            //Devuelvo el objetivo que tiene seleccionado la IA
            return target.Value;
        }

        // Si no encuentra un objetivo valido, devuelvo null
        return null;
    }
}
