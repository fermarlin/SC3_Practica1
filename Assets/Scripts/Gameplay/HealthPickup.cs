
using UnityEngine;

//Este script se encarga de gestionar los botiquines que puede recoger el jugador
public class HealthPickup : MonoBehaviour
{
    [Header("Healing")]
    //Cantidad de vida que recupera el jugador al recoger el botiquin
    [SerializeField] private int healAmount = 25;

    [Header("Effects")]
    //Particulas que aparecen cuando el jugador recoge el botiquin
    [SerializeField] private ParticleSystem pickupEffectPrefab;

    //Esta funcion se ejecuta cuando un objeto entra en el Trigger del botiquin
    private void OnTriggerEnter(Collider other)
    {
        //Compruebo si el objeto que ha entrado es el jugador, usco en los padres porque tengo separado el modelo de la logica
        PlayerController player = other.GetComponentInParent<PlayerController>();

        // Si no es el jugador, no hago nada
        if (!other.transform.CompareTag("Player"))
            return;

        //Pillo el componente de vida del jugador
        Health health = other.GetComponentInParent<Health>();

        //Compruebo que tenga vida, que no este muerto y que no tenga la vida al maximo, asi evito gastar un botiquin cuando no lo necesita
        if (health == null ||
            health.IsDead ||
            health.CurrentHealth >= health.MaxHealth)
            return;

        //Recupero la cantidad de vida configurada
        health.Heal(healAmount);

        //Reproduzco el sonido de recoger el botiquin
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayAudio2D("HealthPickup");
        }

        // Instancio las particulas en la posicion del jugador
        if (pickupEffectPrefab != null)
        {
            Instantiate(pickupEffectPrefab, player.transform.position, Quaternion.identity
            );
        }

        //Destruyo el botiquin despues de recogerlo
        Destroy(gameObject);
    }
}
