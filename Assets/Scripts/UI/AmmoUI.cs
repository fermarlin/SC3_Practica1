
using TMPro;
using UnityEngine;

//Este script se encarga de mostrar la municion del arma equipada en la interfaz utilizo el evento de PlayerShooting para actualizar el texto cuando cambia la municion
public class AmmoUI : MonoBehaviour
{
    //Referencia al script que controla las armas y su municion
    [SerializeField] private PlayerShooting playerShooting;

    // Texto de TextMeshPro donde muestro las balas actuales y el tamano del cargador
    [SerializeField] private TMP_Text ammoText;

    private void OnEnable()
    {
        //Si no tengo asignado el PlayerShooting, no hago nada
        if (playerShooting == null)
            return;

        //Me suscribo al evento que avisa cuando cambia la municion
        playerShooting.OnAmmoChanged += UpdateAmmo;

        //Actualizo el contador al activar la interfaz asi muestro la municion aunque todavia no se haya disparado
        UpdateAmmo(playerShooting.CurrentAmmo,playerShooting.MagazineSize);
    }

    private void OnDisable()
    {
        //Me desuscribo del evento cuando se desactiva la interfaz
        if (playerShooting != null)
        playerShooting.OnAmmoChanged -= UpdateAmmo;
    }

    //Esta funcion actualiza el texto con las balas que quedan en el cargador
    private void UpdateAmmo(int current, int maximum)
    {
        //Compruebo que el texto exista antes de modificarl
        if (ammoText != null)
            ammoText.text = $"{current} / {maximum}";
    }
}
