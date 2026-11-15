
using UnityEngine;

//Este ScriptableObject guarda las estadisticas y valores de configuracion de los enemigos. Asi puedo crear distintos tipos de enemigos sin tener que modificar sus scripts
[CreateAssetMenu(fileName = "NewEnemy", menuName = "Game/Enemy Config")]
public class EnemyConfigSO : ScriptableObject
{
    [Header("Stats")]
    //La vida maxima que tiene el enemigo
    [Min(1)] public int MaxHealth = 100;

    //El dano que hace el enemigo cuando ataca
    [Min(0)] public int AttackDamage = 10;

    [Header("Detection")]
    //La distancia a la que el enemigo puede detectar a su objetivo
    [Min(0)] public float DetectionRange = 10f;

    //La distancia a la que el enemigo puede empezar a atacar
    [Min(0)] public float AttackRange = 1.8f;

    [Header("Movement")]
    //La velocidad a la que se mueve el enemigo
    [Min(0)] public float MoveSpeed = 3.5f;

    [Header("Combat")]
    //El tiempo que tiene que esperar el enemigo entre un ataque y otro
    [Min(0)] public float AttackCooldown = 1.5f;

    //El radio que tiene el golpe del enemigo para detectar si alcanza a alguien
    [Min(0.1f)] public float HitRadius = 0.6f;

    [Header("Reactions")]
    //El tiempo que se queda aturdido el enemigo cuando recibe dano
    [Min(0)] public float HitStunDuration = 0.6f;

    [Header("Rotation")]
    //La velocidad a la que gira el enemigo para mirar hacia su objetivo mientras ataca
    [Min(0f)] public float AttackTurnSpeed = 180f;
}
