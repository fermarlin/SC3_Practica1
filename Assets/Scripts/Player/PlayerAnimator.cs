using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private Rigidbody playerRb;
    [SerializeField] private PlayerController playerController;
    
    float speed;


    // Update is called once per frame
    void Update()
    {
        Vector3 horizontalVelocity = new Vector3(
            playerRb.linearVelocity.x,
            0f,
            playerRb.linearVelocity.z
        );

        speed = Mathf.InverseLerp(
            0f,
            playerController.MoveSpeed,
            horizontalVelocity.magnitude
        );
        animator.SetFloat("Speed", speed, 0.1f, Time.deltaTime);
    }
}
