
using System.Collections.Generic;
using UnityEngine;

// Este script se encarga de hacer explotar los barriles y aplicar dano a los objetos cercanos
public class ExplosiveBarrel : MonoBehaviour
{
    [Header("Explosion")]
    //Dano que provoca la explosion
    [SerializeField] private int explosionDamage = 60;

    //Radio que tiene la explosion para alcanzar a otros objetos
    [SerializeField] private float explosionRadius = 5f;

    //Capas de los objetos que pueden verse afectados por la explosion
    [SerializeField] private LayerMask damageLayers;

    [Header("Effects")]
    //Prefab de las particulas que aparecen cuando explota el barril
    [SerializeField] private ParticleSystem explosionPrefab;


    //Componente que controla la vida del barril
    private Health health;

    //Booleano para evitar que el barril explote varias veces
    private bool exploded;

    private void Awake()
    {
        //Pillo el componente Health del barril
        health = GetComponent<Health>();
    }

    private void OnEnable()
    {
        // Me suscribo al evento de muerte para que el barril explote cuando su vida llegue a cero
        health.OnDied += Explode;
    }

    private void OnDisable()
    {
        //Me desuscribo del evento cuando el objeto se desactiva
        health.OnDied -= Explode;
    }

    //Esta funcion se encarga de generar la explosion y aplicar el dano
    private void Explode()
    {
        //Si el barril ya ha explotado, no vuelvo a ejecutar la funcion
        if (exploded)
            return;

        // Marco el barril como explotado
        exploded = true;

        //Instancio las particulas de la explosion
        if (explosionPrefab != null)
        {
            ParticleSystem explosion = Instantiate(explosionPrefab, transform.position, Quaternion.identity);

            //Reproduzco el efecto de particulas
            explosion.Play();
        }

        //Creo una esfera para detectar todos los objetos dentro del radio de la explosion
        Collider[] hits = Physics.OverlapSphere(transform.position, explosionRadius, damageLayers, QueryTriggerInteraction.Collide);

        //Guardo los objetivos danados para evitar aplicarles dano varias veces
        HashSet<IDamageable> damagedTargets = new();

        foreach (Collider hit in hits)
        {
            //Ignoro los collidrs del propio barril para que no se haga dano a si mismo
            if (hit.transform.IsChildOf(transform))
                continue;

            //Busco un componente que implemente IDamageable en el objeto o sus padres
            IDamageable target = hit.GetComponentInParent<IDamageable>();

            //Si no puede recibir dano o ya ha sido golpeado, lo ignoro
            if (target == null || !damagedTargets.Add(target)) continue;

            //Aplico el dano de la explosion
            target.TakeDamage(explosionDamage);
        }

        //Destruyo el barril despues de aplicar todo el dano
        Destroy(gameObject);
    }
}
