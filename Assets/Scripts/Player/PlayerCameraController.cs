using UnityEngine;
using Unity.Cinemachine;
using UnityEngine.InputSystem;

public class PlayerCameraController : MonoBehaviour
{
    [SerializeField]
    CameraConfigSO cameraConfigSO;
    [SerializeField]
    private CinemachineCamera CM_Normal;
    [SerializeField]
    private CinemachineCamera CM_Aim;
    [SerializeField]
    private Transform cameraPivot;
    [SerializeField]
    private Transform playerTransform;


    private float yaw;
    private float pitch;
    private PlayerInputs playerInputs;
    private Vector2 rotationInput;

    void Awake(){
        playerInputs = new PlayerInputs();
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }


    void OnEnable()
    {
        playerInputs.PlayerActionMap.Enable();
        playerInputs.PlayerActionMap.Aim.performed += ChangeCamera;
        playerInputs.PlayerActionMap.Aim.canceled += ChangeCamera;
        playerInputs.PlayerActionMap.Rotation.performed += RotateCamera;
        playerInputs.PlayerActionMap.Rotation.canceled += RotateCamera;
    }

    void OnDisable(){
        playerInputs.PlayerActionMap.Aim.performed -= ChangeCamera;
        playerInputs.PlayerActionMap.Aim.canceled -= ChangeCamera;
        playerInputs.PlayerActionMap.Rotation.performed -= RotateCamera;
        playerInputs.PlayerActionMap.Rotation.canceled -= RotateCamera;
        playerInputs.PlayerActionMap.Disable();
    }

    void LateUpdate(){
        cameraPivot.position = playerTransform.position+Vector3.up*cameraConfigSO.CameraHeight;
    }


    void OnDestroy(){
        playerInputs.Dispose();
    }


     void ChangeCamera(InputAction.CallbackContext context){
        if(context.performed){
            CM_Normal.Priority = 0; 
            CM_Aim.Priority = 10; 
        }else if (context.canceled){
            CM_Normal.Priority = 10; 
            CM_Aim.Priority = 0; 
        }
        
    }

    void RotateCamera(InputAction.CallbackContext context){
        rotationInput = context.ReadValue<Vector2>(); 
        yaw += rotationInput.x * cameraConfigSO.MouseSensitivity;
        pitch += rotationInput.y * cameraConfigSO.MouseSensitivity;
        pitch = Mathf.Clamp(pitch, -40f, 70f);
        
        cameraPivot.localRotation = Quaternion.Euler(new Vector3(-pitch,yaw,0));
    }
}
