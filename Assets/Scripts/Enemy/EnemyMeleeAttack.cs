
using System.Collections.Generic;
using UnityEngine;

//Este script se encarga de aplicar el dano cuando el enemigo golpea cuerpo a cuerpo
public class EnemyMeleeAttack : MonoBehaviour
{
    // Punto desde el que compruebo si el enemigo ha golpeado a alguien
    [SerializeField] private Transform meleeHitPoint;
    //Capas de los objetos que pueden recibir el ataque
    [SerializeField] private LayerMask targetLayers;

    //Componentes que necesito para obtener el dano y comprobar el estado del enemigo
    private EnemyData enemyData;
    private EnemyDamageReactions reactions;
    private Health health;

    private void Awake()
    {
        // Pillo los componentes del padre porque tengo separado el modelo de la logica
        enemyData = GetComponentInParent<EnemyData>();
        reactions = GetComponentInParent<EnemyDamageReactions>();
        health = GetComponentInParent<Health>();
    }

    //Esta funcion se llama mediante un Animation Event en el momento del golpe
    public void OnAttackHit()
    {
        //Compruebo que tengo la configuracion del enemigo
        if (enemyData == null || enemyData.Config == null)
            return;

        //Compruebo que teng asignado el punto de golpe
        if (meleeHitPoint == null)
            return;

        //Si el enemigo esta muerto, no puede hacer dano
        if (health != null && health.IsDead)
            return;

        // Si esta aturdido, tampoco puede golpear
        if (reactions != null && reactions.IsStunned)
            return;

        //Creo una esfera en el punto de golpe para detectar los objetos cercanos, utilizo el radio configurado en el ScriptableObject del enemigo
        Collider[] hits = Physics.OverlapSphere(meleeHitPoint.position, enemyData.Config.HitRadius, targetLayers, QueryTriggerInteraction.Ignore);

        // Guardo los objetivos golpeados para evitar aplicarles dano varias veces
        HashSet<IDamageable> damagedTargets = new();

        foreach (Collider hit in hits)
        {
            //Ignoro los colliders del propio enemigo para que no se haga dano a si mismo
            if (hit.transform.IsChildOf(enemyData.transform)) continue;

            //Busco un componente que implemente IDamageable en el objeto o sus padres
            IDamageable damageable = hit.GetComponentInParent<IDamageable>();

            //Si el objeto no puede recibir dano, lo ignoro
            if (damageable == null)
                continue;

            //Si ya he golpeado a este objetivo, no vuelvo a aplicarle dano
            if (!damagedTargets.Add(damageable))
                continue;

            //Aplico el dano definido en el ScriptableObject del enemigo
            damageable.TakeDamage(enemyData.Config.AttackDamage);
        }
    }
}
