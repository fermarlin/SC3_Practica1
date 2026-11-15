
using System;
using UnityEngine;

//Este script se encarga de detectar los objetos con los que puede interactuar el jugador utilizo la interfaz IInteractable para no depender de un tipo de objeto concreto
public class PlayerInteractor : MonoBehaviour
{
    [Header("Interaction")]
    //Distancia maxima a la que puedo interactuar con los objetos
    [SerializeField, Min(0.1f)]
    private float interactionRadius = 2.5f;

    // Capas de los objetos que quiero detectar
    [SerializeField]
    private LayerMask interactionLayers;

    //Sistema de inputs del jugador
    private PlayerInputs inputs;

    //Guardo si el jugador esta reparando actualmente
    private bool isRepairing;

    //Permito que otros scripts consulten si estoy reparando
    public bool IsRepairing => isRepairing;

    //Evento para avisar cuando empiezo o dejo de reparar
    public event Action<bool> OnRepairingChanged;

    private void Awake()
    {
        //Creo una instancia de los controles del jugador
        inputs = new PlayerInputs();
    }

    private void OnEnable()
    {
        //Activo el input de reparacion
        inputs.PlayerActionMap.Repair.Enable();
    }

    private void OnDisable()
    {
        // Dejo de reparar si se desactiva el script
        SetRepairing(false);

        //Desactivo el input de reparacion
        inputs.PlayerActionMap.Repair.Disable();
    }

    private void OnDestroy()
    {
        //Libero los recursos del sistema de inputs
        inputs.Dispose();
    }

    private void Update()
    {
        //Si no estoy manteniendo pulsada la tecla de reparar desactivo la reparacion y no sigo comprobando objetos
        if (!inputs.PlayerActionMap.Repair.IsPressed())
        {
            SetRepairing(false);
            return;
        }

        //Busco el objeto interactuable mas cercano
        IInteractable target = FindInteractable();

        //Compruebo si el objeto es un generador y si necesita reparacion
        bool canRepair =
            target is GeneratorRepair generator &&
            generator.CanRepair;

        //Actualizo el estado de reparacion del jugador
        SetRepairing(canRepair);

        //Si he encontrado un objeto interactuable, ejecuto su interaccion, paso deltaTime para que pueda funcionar de manera continua
        if (target != null) target.Interact(Time.deltaTime);
    }

    //Esta funcion se encarga de actualizar el estado de reparacion
    private void SetRepairing(bool value)
    {
        //Si el estado no ha cambiado, no necesito hacer nada
        if (isRepairing == value)
            return;

        //Guardo el nuevo estado
        isRepairing = value;

        //Aviso al resto de scripts de que he empezado o dejado de reparar por ejemplo, para cambiar el arma por la llave inglesa y reproducir la animacion
        OnRepairingChanged?.Invoke(isRepairing);
    }

    //Esta funcion busca el objeto interactuable mas cercano al jugador
    private IInteractable FindInteractable()
    {
        //Creo una esfera alrededor del jugador para detectar objetos cercanos
        Collider[] hits = Physics.OverlapSphere(transform.position, interactionRadius, interactionLayers, QueryTriggerInteraction.Collide);

        //Guardo el objeto mas cercano y su distancia
        IInteractable closest = null;
        float closestDistance = float.MaxValue;

        //Recorro todos los colliders que he encontrado
        foreach (Collider hit in hits)
        {
            //Ignoro los colliders del propio jugador
            if (hit.transform.IsChildOf(transform))
                continue;

            // Busco un componente que implemente IInteractable yambien compruebo los padres por si el collider esta en un hijo
            IInteractable interactable = hit.GetComponentInParent<IInteractable>();

            //Si el objeto no es interactuable, lo ignoro
            if (interactable == null)
                continue;

            //Si el componente esta desactivado, no permito interactuar
            if (interactable is Behaviour behaviour &&
                !behaviour.isActiveAndEnabled)
                continue;

            //Ignoro los generadores que no necesitan reparacion
            if (interactable is GeneratorRepair generator && !generator.CanRepair)
                continue;

            //Busco el punto del collider mas cercano al jugador
            Vector3 point = hit.ClosestPoint(transform.position);

            //Calculo la distancia al objeto, utilizo sqrMagnitude para evitar calcular la raiz cuadrada
            float distance = (point - transform.position).sqrMagnitude;

            //Si este objeto esta mas cerca que el anterior, lo guardo
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closest = interactable;
            }
        }

        //Devuelvo el objeto interactuable mas cercano
        return closest;
    }
}
