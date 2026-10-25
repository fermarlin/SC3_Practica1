using UnityEngine;

[CreateAssetMenu(fileName = "CameraConfig",menuName = "Game/Camera Config")]
public class CameraConfigSO : ScriptableObject
{
    [SerializeField]
    private float mouseSensitivity = 0.2f;
    [SerializeField]
    private float cameraHeight = .2f;

    public float MouseSensitivity => mouseSensitivity;
    public float CameraHeight => cameraHeight;
}
