using UnityEngine;

public class PlayerAimUI : MonoBehaviour
{
    [SerializeField] private PlayerController playerController;
    [SerializeField] private GameObject crosshair;

    void OnEnable()
    {
        playerController.OnAimChanged += UpdateCrosshair;



    }

    void OnDisable(){
        playerController.OnAimChanged -= UpdateCrosshair;
    }

    private void UpdateCrosshair(bool isAiming)
    {
        crosshair.SetActive(isAiming);
    }
}
