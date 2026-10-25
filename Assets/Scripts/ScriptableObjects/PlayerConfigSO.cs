using UnityEngine;

[CreateAssetMenu(fileName = "PlayerConfig",menuName = "Game/Player Config")]
public class PlayerConfigSO : ScriptableObject
{
    [SerializeField] private float rotationSpeed=1000;
    [SerializeField] private float moveSpeed=2;
    [SerializeField] private float jumpForce=.7f;
    [SerializeField] private float groundCheckRadius=7;
    [SerializeField] private LayerMask groundLayer;

    public float RotationSpeed => rotationSpeed;
    public float MoveSpeed => moveSpeed;
    public float JumpForce => jumpForce;
    public float GroundCheckRadius => groundCheckRadius;
    public LayerMask GroundLayer => groundLayer;

}

