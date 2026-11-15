
using UnityEngine;

//Este script se encarga de mostrar la mirilla cuando el jugador esta apuntando, utilizo el evento de PlayerController para actualizarla cuando cambia el apuntado
public class PlayerAimUI : MonoBehaviour
{
    //Referencia al script que controla el movimiento y apuntado del jugador
    [SerializeField] private PlayerController playerController;

    //Objeto de la interfaz que utilizo como mirilla
    [SerializeField] private GameObject crosshair;

    private void OnEnable()
    {
        //Si no tengo asignado el PlayerController, no hago nada
        if (playerController == null)
            return;

        //Me suscribo al evento que avisa cuando empiezo o dejo de apuntar
        playerController.OnAimChanged += UpdateCrosshair;

        //Actualizo la mirilla con el estado actual del jugador
        UpdateCrosshair(playerController.IsAiming);
    }

    private void OnDisable()
    {
        //Me desuscribo del evento cuando se desactiva la interfaz
        if (playerController != null)
        playerController.OnAimChanged -= UpdateCrosshair;
    }

    //Esta funcion muestra u oculta la mirilla dependiendo de si estoy apuntando
    private void UpdateCrosshair(bool isAiming)
    {
        if (crosshair != null)
        crosshair.SetActive(isAiming);
    }
}
