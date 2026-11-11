using UnityEngine;
using UnityEngine.Animations.Rigging;

public class PlayerAimRig : MonoBehaviour
{
    [SerializeField] private PlayerController playerController;
    [SerializeField] private Rig upperBodyRig;
    [SerializeField] private Transform aimTarget;

    [SerializeField] private float aimHeight = 1.5f;
    [SerializeField] private float aimDistance = 10f;

    private Transform cameraTransform;

    private void Awake(){
        cameraTransform = Camera.main.transform;
    }

    private void LateUpdate(){
        UpdateRigWeight();
        UpdateAimTargetPosition();
    }

    private void UpdateRigWeight(){
        upperBodyRig.weight = playerController.IsAiming ? 1f : 0f;
    }

    private void UpdateAimTargetPosition(){
        aimTarget.position = transform.position + Vector3.up * aimHeight + cameraTransform.forward * aimDistance;
    }
}