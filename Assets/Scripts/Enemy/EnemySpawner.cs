using System.Collections;
using System.Collections.Generic;
using Unity.Behavior;
using UnityEngine;

//Este script se encarga de generar los enemigos y asignarles sus objetivos y puntos de patrulla
public class EnemySpawner : MonoBehaviour
{
    [Header("Enemy")]
    //Prefab del enemigo que voy a instanciar
    [SerializeField] private GameObject enemyPrefab;

    [Header("Spawn")]
    //Posiciones donde pueden aparecer los enemigos
    [SerializeField] private Transform[] spawnPoints;

    //Tiempo que espero entre cada intento de generar un enemigo
    [SerializeField, Min(0.1f)] private float spawnInterval = 10f;

    //Numero maximo de enemigos que pueden estar vivos al mismo tiempo
    [SerializeField, Min(1)] private int maxAliveEnemies = 8;

    [Header("Patrol")]
    //Objeto padre que contiene los diferentes puntos de patrulla
    [SerializeField] private Transform patrolPointsParent;

    //Guardo los enemigos que he generado para controlar cuantos siguen vivos
    private readonly List<GameObject> aliveEnemies = new();

    //Guardo los puntos de patrulla para pasarselos al Behavior Graph
    private readonly List<GameObject> waypoints = new();

    private void Awake()
    {
        // Si no tengo puntos de patrulla asignados no hago nada
        if (patrolPointsParent == null)
            return;

        //Recorro los hijos del objeto padre y guardo sus GameObjects
        foreach (Transform point in patrolPointsParent) waypoints.Add(point.gameObject);
    }

    //Utilizo una corrutina para generar enemigos cada cierto tiempo
    private IEnumerator Start()
    {
        while (true)
        {
            //Intento generar un enemigo
            SpawnEnemy();

            //Espero el tiempo configurado antes del siguiente intento
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    //Esta funcion se encarga de generar y configurar un enemigo
    private void SpawnEnemy()
    {
        //Compruebo que tengo un prefab y puntos donde generar enemigos
        if (enemyPrefab == null || spawnPoints == null || spawnPoints.Length == 0)
            return;

        //Elimino de la lista las referencias a enemigos que ya han sido destruidos
        aliveEnemies.RemoveAll(enemy => enemy == null);

        //Si ya he llegado al numero maximo de enemigos no genero mas
        if (aliveEnemies.Count >= maxAliveEnemies)
            return;

        //Selecciono aleatoriamente uno de los puntos de spawn
        Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];

        if (spawnPoint == null) return;

        //Instancio el enemigo en la posicion y rotacion del punto seleccionado
        GameObject enemy = Instantiate( enemyPrefab, spawnPoint.position, spawnPoint.rotation);

        //Busco el componente que controla el Behavior Graph del enemigo
        BehaviorGraphAgent agent = enemy.GetComponent<BehaviorGraphAgent>();

        //Si el enemigo no tiene una IA configurada lo destruyo
        if (agent == null)
        {
            Destroy(enemy);
            return;
        }

        /*Asigno aleatoriamente una de las tres misiones del enemigo, puede atacar al jugador, al generador o al ordenador
        Si mas adelante hace faltn meter mas objetivos habria que modificar este rango
        */
        EnemyObjective mission = (EnemyObjective)Random.Range(0, 3);

        //Paso la mision seleccionada al Blackboard del enemigo
        agent.SetVariableValue("Mission", mission);

        //Paso una copia de los puntos de patrulla a su Blackboard
        agent.SetVariableValue("Waypoints", new List<GameObject>(waypoints));

        //Activo el Behavior Graph para que el enemigo empiece a ejecutar su IA
        agent.enabled = true;

        //Guardo el enemigo generado para controlar cuantos hay vivos
        aliveEnemies.Add(enemy);
    }
}
