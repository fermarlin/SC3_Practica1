
using UnityEngine;
using UnityEngine.Animations.Rigging;

//Este script se encarga de controlar el Rig del torso del jugador para que apunte hacia donde mira la camara
public class PlayerAimRig : MonoBehaviour
{
    //Componente que controla el movimiento y el apuntado del jugador
    [SerializeField] private PlayerController playerController;

    //Rig que utilizo para modificar la animacion de la parte superior del cuerpo
    [SerializeField] private Rig upperBodyRig;

    //Objeto que utilizo como objetivo hacia el que debe apuntar el personaje
    [SerializeField] private Transform aimTarget;

    //Altura y distancia a la que coloco el objetivo de apuntado
    [SerializeField] private float aimHeight = 1.5f;
    [SerializeField] private float aimDistance = 10f;

    //Guardo la referencia a la camara
    private Transform cameraTransform;

    private void Awake()
    {
        //Pillo la camara principal para conocer hacia donde esta mirando
        cameraTransform = Camera.main.transform;
    }

    private void LateUpdate()
    {
        //Actualizo el peso del Rig y la posicion del objetivo, utilizo LateUpdate para hacerlo despues del movimiento del jugador
        UpdateRigWeight();
        UpdateAimTargetPosition();
    }

    //Esta funcion se encarga de activar o desactivar el Rig cuando el jugador apunta
    private void UpdateRigWeight()
    {
        // Si el jugador esta apuntando, activo el Rig, si deja de apuntar, lo desactivo
        upperBodyRig.weight = playerController.IsAiming ? 1f : 0f;
    }

    //Esta funcion se encarga de colocar el objetivo de apuntado delante de la camara
    private void UpdateAimTargetPosition()
    {
        //Calculo la posicion del objetivo uilizando la altura del jugador y la direccion hacia la que esta mirando la camara
        aimTarget.position = transform.position + Vector3.up * aimHeight + cameraTransform.forward * aimDistance;
    }
}
