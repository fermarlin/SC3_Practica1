
using System;
using UnityEngine;
using UnityEngine.InputSystem;

//Este script se encarga de controlar el movimiento, salto y rotacion del jugador

public class PlayerController : MonoBehaviour
{
    // Objeto que utilizo para comprobar si el jugador esta tocando el suelo
    [SerializeField] private Transform groundCheck;

    //ScriptableObject donde guardo la configuracion del jugador
    [SerializeField] private PlayerConfigSO config;

    //Componentes que necesito para controlar al jugador
    private PlayerInputs playerInputs;
    private Rigidbody playerRb;
    private Transform cameraTransform;

    //Guardo el movimiento introducido por el jugador
    private Vector2 moveInput;

    //Boolanos para controlar el apuntado y el salto
    private bool isAiming;
    private bool jumpRequested;
    private bool isGrounded;

    //Permito que otros scripts consulten la velocidad y el estado de apuntado
    public float MoveSpeed => config.MoveSpeed;
    public bool IsAiming => isAiming;

    // Eventos para avisar cuando el jugador apunta o salta
    public event Action<bool> OnAimChanged;
    public event Action OnJumped;

    private void Awake()
    {
        //Creo una instancia de los controles y pillo el Rigidbody
        playerInputs = new PlayerInputs();
        playerRb = GetComponent<Rigidbody>();

        //Guardo la camara principal para no tener que buscarla en cada FixedUpdate
        Camera mainCamera = Camera.main;

        if (mainCamera == null)
        {
            enabled = false;
            return;
        }

        cameraTransform = mainCamera.transform;
    }

    private void OnEnable()
    {
        //Activo los controles del jugador
        playerInputs.PlayerActionMap.Enable();

        //Me suscribo a los eventos del movimiento
        playerInputs.PlayerActionMap.Movement.performed += Movement;
        playerInputs.PlayerActionMap.Movement.canceled += Movement;

        //Me suscribo a los eventos de apuntado
        playerInputs.PlayerActionMap.Aim.performed += AimMovement;
        playerInputs.PlayerActionMap.Aim.canceled += AimMovement;

        //Me suscribo al evento del salto
        playerInputs.PlayerActionMap.Jump.started += Jump;
    }

    private void OnDisable()
    {
        //Me desuscribo de los eventos
        playerInputs.PlayerActionMap.Movement.performed -= Movement;
        playerInputs.PlayerActionMap.Movement.canceled -= Movement;

        playerInputs.PlayerActionMap.Aim.performed -= AimMovement;
        playerInputs.PlayerActionMap.Aim.canceled -= AimMovement;

        playerInputs.PlayerActionMap.Jump.started -= Jump;

        //Desactivo los controles
        playerInputs.PlayerActionMap.Disable();

        //Limpio los inputs por si se vuelve a activar el jugador
        moveInput = Vector2.zero;
        jumpRequested = false;
        isAiming = false;
    }

    private void OnDestroy()
    {
        //Libero los recursos del sistema de inputs
        playerInputs.Dispose();
    }

    private void FixedUpdate()
    {
        //Pillo las direcciones de la camara que he guardado anteriormente
        Vector3 cameraForward = cameraTransform.forward;
        Vector3 cameraRight = cameraTransform.right;

        //Ignoro la inclinacion vertical para moverme siempre horizontalmente
        cameraForward.y = 0f;
        cameraRight.y = 0f;

        cameraForward.Normalize();
        cameraRight.Normalize();

        //Calculo la direccion de movimiento respecto a la camara
        Vector3 moveDirection =
            cameraRight * moveInput.x +
            cameraForward * moveInput.y;

        //Evito que el jugador se mueva mas rapido en diagonal
        moveDirection = Vector3.ClampMagnitude(moveDirection, 1f);

        //Calculo la velocidad y mantengo el movimiento vertical del Rigidbody
        Vector3 velocity = moveDirection * config.MoveSpeed;
        velocity.y = playerRb.linearVelocity.y;

        playerRb.linearVelocity = velocity;

        //Si estoy apuntando giro hacia la camara
        //Si no, giro hacia donde me estoy moviendo
        Vector3 targetDirection = isAiming ? cameraForward : moveDirection;

        //Solo giro si tengo una direccion valida
        if (targetDirection.sqrMagnitude > 0.001f)
        {
            Quaternion targetRot = Quaternion.LookRotation(targetDirection);

            //Giro suavemente utilizando la velocidad configurada
            Quaternion finalRot = Quaternion.RotateTowards(playerRb.rotation, targetRot, config.RotationSpeed * Time.fixedDeltaTime);

            playerRb.MoveRotation(finalRot);
        }

        //Compruebo si estoy tocando el suelo mediante una esfera
        isGrounded = Physics.CheckSphere(groundCheck.position, config.GroundCheckRadius, config.GroundLayer);

        //Si he pulsado saltar y estoy en el suelo, aplico una fuerza hacia arriba
        if (jumpRequested && isGrounded)
        {
            playerRb.AddForce(Vector3.up * config.JumpForce, ForceMode.Impulse);

            //Aviso a otros scripts de que he saltado
            OnJumped?.Invoke();

            //Reproduzco el sonido del salto
            if (AudioManager.Instance != null) AudioManager.Instance.PlayAudio2D("Jump");
        }

        //Limpio la peticion de salto
        jumpRequested = false;
    }

    //Guardo la direccion de movimiento introducida por el jugador
    private void Movement(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    //Actualizo el estado de apuntado y aviso a otros scripts
    private void AimMovement(InputAction.CallbackContext context)
    {
        isAiming = context.performed;
        OnAimChanged?.Invoke(isAiming);
    }

    //Guardo la peticion de salto para ejecutarla en FixedUpdate
    private void Jump(InputAction.CallbackContext context)
    {
        jumpRequested = true;
    }
}
