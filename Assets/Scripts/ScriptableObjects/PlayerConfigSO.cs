
using UnityEngine;

//Este ScriptableObject se encarga de guardar los valores de configuracion del jugador, asi puedo modificar su movimiento y salto desde el Inspector sin tocar el PlayerController
[CreateAssetMenu(fileName = "PlayerConfig", menuName = "Game/Player Config")]
public class PlayerConfigSO : ScriptableObject
{
    //Velocidad a la que gira el jugador
    [SerializeField] private float rotationSpeed = 1000;

    //Velocidad a la que se mueve el jugador
    [SerializeField] private float moveSpeed = 2;

    //Fuerza que aplico al Rigidbody cuando el jugador salta
    [SerializeField] private float jumpForce = .7f;

    //Radio de la esfera que utilizo para comprobar si el jugador esta tocando el suelo
    [SerializeField] private float groundCheckRadius = .7f;

    // Capas que considero como suelo para permitir que el jugador salte
    [SerializeField] private LayerMask groundLayer;

    //Permito que otros scripts consulten los valores de movimiento y salto
    public float RotationSpeed => rotationSpeed;
    public float MoveSpeed => moveSpeed;
    public float JumpForce => jumpForce;
    public float GroundCheckRadius => groundCheckRadius;
    public LayerMask GroundLayer => groundLayer;
}
