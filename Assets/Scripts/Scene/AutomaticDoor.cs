
using System.Collections.Generic;
using UnityEngine;

//Este script se encarga de abrir y cerrar automaticamente las puertas compruebo si esta el jugador o enemigos dentro de su zona de deteccion
public class AutomaticDoor : MonoBehaviour
{
    //Capas de los objetos que pueden activar la puerta
    [SerializeField] private LayerMask detectionLayers;

    //Animator que utilizo para abrir y cerrar la puerta
    private Animator animator;

    //Guardo los colliders que estan dentro de la zona de deteccion, utilizo un HashSet para evitar guardar el mismo collider varias veces
    private readonly HashSet<Collider> detected = new();

    private void Awake()
    {
        //Pillo el Animator de la puerta
        animator = GetComponent<Animator>();
    }

    //Esta funcion se ejecuta cuando un objeto entra en el Trigger
    private void OnTriggerEnter(Collider other)
    {
        //Compruebo si el objeto pertenece a una de las capas permitidas, si no pertenece, lo ignoro
        if ((detectionLayers.value & (1 << other.gameObject.layer)) == 0)
            return;

        //Guardo el collider que ha entrado
        detected.Add(other);

        //Activo el booleano del Animator para abrir la puerta
        animator.SetBool("Open", true);
    }

    //Esta funcion se ejecuta cuando un objeto sale del Trigger
    private void OnTriggerExit(Collider other)
    {
        //Elimino el collider de la lista de objetos detectados
        detected.Remove(other);

        //Si no queda ningun collider dentro, cierro la puerta
        if (detected.Count == 0)
            animator.SetBool("Open", false);
    }

    private void Update()
    {
        //Si no tengo ningun objeto detectado, no necesito comprobar nada
        if (detected.Count == 0)
            return;

        //Elimino los colliders que han sido destruidos o desactivados esto evita que la puerta se quede abierta cuando un enemigo muere dentro
        detected.RemoveWhere(collider => collider == null || !collider.enabled ||!collider.gameObject.activeInHierarchy);

        //Si despues de limpiar la lista ya no queda nadie, cierro la puerta
        if (detected.Count == 0)
        animator.SetBool("Open", false);
    }

    private void OnDisable()
    {
        //Limpio los colliders guardados cuando se desactiva la puerta
        detected.Clear();

        //Restablezco el booleano del Animator para dejarla cerrada
        if (animator != null) animator.SetBool("Open", false);
    }
}
