
using UnityEngine;

//Este ScriptableObject se encarga de guardar los valores de configuracion de la camara, asi puedo modificar su comportamiento desde el Inspector sin tocar los scripts
[CreateAssetMenu(fileName = "CameraConfig", menuName = "Game/Camera Config")]
public class CameraConfigSO : ScriptableObject
{
    // Sensibilidad del raton para controlar la rotacion de la camara
    [SerializeField]
    private float mouseSensitivity = 0.2f;

    //Altura a la que coloco el pivot de la camara respecto al jugador
    [SerializeField]
    private float cameraHeight = .2f;

    //Permito que otros scripts consulten la sensibilidad del raton
    public float MouseSensitivity => mouseSensitivity;

    //Permito que otros scripts consulten la altura de la camara
    public float CameraHeight => cameraHeight;
}
