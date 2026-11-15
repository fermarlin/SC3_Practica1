
using UnityEngine;
using Unity.Cinemachine;
using UnityEngine.InputSystem;

//Este script se encarga de controlar la camara del jugador utilizo Cinemachine para cambiar entre la camara normal y la de apuntado
public class PlayerCameraController : MonoBehaviour
{
    //ScriptableObject donde guardo la configuracion de la camara
    [SerializeField] private CameraConfigSO cameraConfigSO;

    // camaras que utilizo para el movimiento normal y el apuntado
    [SerializeField] private CinemachineCamera CM_Normal;
    [SerializeField] private CinemachineCamera CM_Aim;

    //Objeto que utilizo para controlar la posicion y rotacion de las camaras
    [SerializeField] private Transform cameraPivot;

    //Referencia al jugador para que la camara pueda seguirlo
    [SerializeField] private Transform playerTransform;

    //Guardo los angulos de rotacion horizontal y vertical de la camara
    private float yaw;
    private float pitch;

    //Sistema de inputs del jugado
    private PlayerInputs playerInputs;

    //Guardo el movimiento del raton para rotar la camara
    private Vector2 rotationInput;

    private void Awake()
    {
        //Creo una instancia de los controles del jugador
        playerInputs = new PlayerInputs();

        // Oculto el cursor y lo bloqueo en el centro de la pantalla
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void OnEnable()
    {
        //Activo los controles del jugador
        playerInputs.PlayerActionMap.Enable();

        //Me suscribo a los eventos de apuntado para cambiar de camara
        playerInputs.PlayerActionMap.Aim.performed += ChangeCamera;
        playerInputs.PlayerActionMap.Aim.canceled += ChangeCamera;

        // Me suscribo a los eventos del movimiento del raton
        playerInputs.PlayerActionMap.Rotation.performed += RotateCamera;
        playerInputs.PlayerActionMap.Rotation.canceled += RotateCamera;
    }

    private void OnDisable()
    {
        //Me desuscribo de los eventos cuando se desactiva el script
        playerInputs.PlayerActionMap.Aim.performed -= ChangeCamera;
        playerInputs.PlayerActionMap.Aim.canceled -= ChangeCamera;

        playerInputs.PlayerActionMap.Rotation.performed -= RotateCamera;
        playerInputs.PlayerActionMap.Rotation.canceled -= RotateCamera;

        //Desactivo los controles
        playerInputs.PlayerActionMap.Disable();
    }

    private void LateUpdate()
    {
        //Actualizo la posicion del pivot para que siga al jugador, le sumo la altura configurada en el ScriptableObject
        cameraPivot.position = playerTransform.position + Vector3.up * cameraConfigSO.CameraHeight;
    }

    private void OnDestroy()
    {
        //Libero los recursos del sistema de inputs
        playerInputs.Dispose();
    }

    //Esta funcion cambia entre la camara normal y la de apuntado
    private void ChangeCamera(InputAction.CallbackContext context)
    {
        //Si el jugador mantiene pulsado el boton de apuntar, le doy mas prioridad a la camara de apuntado
        if (context.performed)
        {
            CM_Normal.Priority = 0;
            CM_Aim.Priority = 10;
        }
        //Cuando deja de apuntar, vuelvo a la camara normal
        else if (context.canceled)
        {
            CM_Normal.Priority = 10;
            CM_Aim.Priority = 0;
        }
    }

    //Esta funcion se encarga de rotar la camara utilizando el raton
    private void RotateCamera(InputAction.CallbackContext context)
    {
        //Pillo el movimiento del raton
        rotationInput = context.ReadValue<Vector2>();

        //Actualizo los angulos de rotacion utilizando la sensibilidad configurada
        yaw += rotationInput.x * cameraConfigSO.MouseSensitivity;
        pitch += rotationInput.y * cameraConfigSO.MouseSensitivity;

        //Limito la rotacion vertical para evitar que la camara gire demasiado
        pitch = Mathf.Clamp(pitch, -40f, 70f);

        //Aplico la rotacion al pivot de la camara invierto el pitch para que el movimiento vertical sea el correcto
        cameraPivot.localRotation = Quaternion.Euler(new Vector3(-pitch, yaw, 0));
    }
}
