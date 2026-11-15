
using UnityEngine;

//Este script se encarga de reparar el generador cuando el jugador interactua con el, implemento IInteractable para utilizar el sistema de interacciones del jugador
public class GeneratorRepair : MonoBehaviour, IInteractable
{
    [Header("Repair")]
    //Cantidad de vida que recupera el generador por segundo
    [SerializeField, Min(0f)]
    private float repairPerSecond = 8f;

    [Header("Audio")]
    //Nombre del sonido de reparacion que tengo guardado en el AudioLibrary
    [SerializeField] private string repairSound = "Repair";

    //Tiempo que espero entre cada sonido de reparacion
    [SerializeField, Min(0.1f)]
    private float soundInterval = 0.5f;

    //Componente que controla la vida del generador
    private Health health;

    //Acumulo la cantidad de vida reparada para poder trabajar con valores decimales
    private float repairAccumulator;

    //Guardo el momento en el que puedo volver a reproducir el sonido
    private float nextSoundTime;

    //Compruebo si el generador puede repararse, tiene que existir, seguir funcionando y no tener la vida al maximo
    public bool CanRepair => health != null && !health.IsDead && health.CurrentHealth < health.MaxHealth;

    private void Awake()
    {
        //Pillo el componente Health del generador
        health = GetComponent<Health>();
    }

    //Esta funcion se ejecuta mientras el jugador mantiene pulsada la tecla de interaccion
    public void Interact(float deltaTime)
    {
        //Si el generador no necesita reparacion, no hago nada
        if (!CanRepair)
        {
            //Limpio la cantidad de reparacion acumulada
            repairAccumulator = 0f;
            return;
        }

        //Compruebo si ya puedo reproducir otro sonido de reparacion
        if (Time.time >= nextSoundTime)
        {
            //Reproduzco el sonido en 3D desde la posicion del generador
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlaySound(repairSound,transform.position);
            }

            //Calculo cuando podre volver a reproducir el sonido
            nextSoundTime = Time.time + soundInterval;
        }

        //Calculo la vida que voy reparando utilizando el tiempo transcurrido asi la velocidad de reparacion no depende de los FPS
        repairAccumulator += repairPerSecond * deltaTime;

        //Convierto la cantidad acumulada a un numero entero porque mi sistema Health trabaja con puntos de vida enteros
        int amount = Mathf.FloorToInt(repairAccumulator);

        //Si todavia no he acumulado un punto de vida, sigo esperando
        if (amount <= 0)
            return;

        //Recupero la vida correspondiente del generador
        health.Heal(amount);

        //Resto la cantidad aplicada y guardo el resto decimal para utilizarlo en los siguientes frames
        repairAccumulator -= amount;
    }
}
