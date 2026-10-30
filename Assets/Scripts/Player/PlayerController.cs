using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Transform groundCheck;
    [SerializeField] private PlayerConfigSO config;
    
    private PlayerInputs playerInputs;
    private Rigidbody playerRb;
    private Vector2 moveInput;
    private bool isAiming;
    private bool jumpRequested;
    private bool isGrounded;

    public float MoveSpeed => config.MoveSpeed;

    void Awake()
    {
        playerInputs = new PlayerInputs();
        playerRb = GetComponent<Rigidbody>();
    }

    void OnEnable()
    {
        playerInputs.PlayerActionMap.Enable();
        playerInputs.PlayerActionMap.Movement.performed += Movement;
        playerInputs.PlayerActionMap.Movement.canceled += Movement;
        playerInputs.PlayerActionMap.Aim.performed += AimMovement;
        playerInputs.PlayerActionMap.Aim.canceled += AimMovement;
        playerInputs.PlayerActionMap.Jump.started += Jump;

    }

    void OnDisable(){
        playerInputs.PlayerActionMap.Movement.performed -= Movement;
        playerInputs.PlayerActionMap.Movement.canceled -= Movement;
        playerInputs.PlayerActionMap.Aim.performed -= AimMovement;
        playerInputs.PlayerActionMap.Aim.canceled -= AimMovement;
        playerInputs.PlayerActionMap.Jump.started -= Jump;

        playerInputs.PlayerActionMap.Disable();
    }

    void OnDestroy(){
        playerInputs.Dispose();
    }

    void FixedUpdate(){
        Vector3 cameraForward = Camera.main.transform.forward;
        Vector3 cameraRight = Camera.main.transform.right;
        cameraForward.y = 0;
        cameraRight.y = 0;
        cameraForward.Normalize();
        cameraRight.Normalize();

        Vector3 moveDirection = (cameraRight*moveInput.x) + (cameraForward*moveInput.y);
        
        moveDirection = Vector3.ClampMagnitude(moveDirection, 1f);
        Vector3 velocity = moveDirection * config.MoveSpeed;
        velocity.y = playerRb.linearVelocity.y;
        playerRb.linearVelocity = velocity;


        if(isAiming){
            Quaternion targetRot = Quaternion.LookRotation(cameraForward);
            Quaternion finalRot = Quaternion.RotateTowards(playerRb.rotation, targetRot, config.RotationSpeed*Time.fixedDeltaTime);
            playerRb.MoveRotation(finalRot); 
        }
        else if(moveDirection.sqrMagnitude > 0.001f){
            Quaternion targetRot = Quaternion.LookRotation(moveDirection);
            Quaternion finalRot = Quaternion.RotateTowards(playerRb.rotation, targetRot, config.RotationSpeed*Time.fixedDeltaTime);
            playerRb.MoveRotation(finalRot);
        }
        
        isGrounded = Physics.CheckSphere(groundCheck.position, config.GroundCheckRadius, config.GroundLayer);

        if(jumpRequested && isGrounded){
            playerRb.AddForce(Vector3.up*config.JumpForce, ForceMode.Impulse);
        }
        jumpRequested = false;



    }

    void Movement(InputAction.CallbackContext context){
        moveInput = context.ReadValue<Vector2>();
            
    }

    void AimMovement(InputAction.CallbackContext context){
        
        if(context.performed){
            isAiming = true;
        }else isAiming = false;
                    
    }

    void Jump(InputAction.CallbackContext context){
        jumpRequested = true;            
    }

}
