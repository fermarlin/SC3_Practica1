
using UnityEngine;
using UnityEngine.UI;
using TMPro;

//Este script se encarga de mostrar la vida del generador, ordenador o el jugador en la interfaz tilizo los eventos de Health para actualizar la barra y el texto cuando cambia la vida
public class HealthUI : MonoBehaviour
{
    [Header("References")]
    //Referencia al componente Health del objeto cuya vida quiero mostrar
    [SerializeField] private Health health;

    [Header("UI")]
    //Imagen que utilizo como barra de vida
    [SerializeField] private Image healthFill;

    //Texto donde muestro la vida actual y la vida maxima
    [SerializeField] private TMP_Text healthText;

    private void OnEnable()
    {
        //Si no tengo asignado el componente Health, no hago nada
        if (health == null)
            return;

        //Me suscribo al evento que avisa cuando cambia la vida
        health.OnHealthChanged += UpdateHealthUI;

        //Actualizo la interfaz con los valores actuales
        RefreshUI();
    }

    private void Start()
    {
        //Vuelvo a actualizar la interfaz por si la vida se ha inicializado despues de OnEnable
        RefreshUI();
    }

    private void OnDisable()
    {
        //Me desuscribo del evento cuando se desactiva la interfaz
        if (health != null)
        {
            health.OnHealthChanged -= UpdateHealthUI;
        }
    }

    //Esta funcion actualiza la interfaz utilizando los valores actuales de Health
    private void RefreshUI()
    {
        //Si no tengo asignado el componente Health, no hago nada
        if (health == null)
            return;

        //Paso la vida actual y maxima a la funcion que actualiza la interfaz
        UpdateHealthUI(
            health.CurrentHealth,
            health.MaxHealth
        );
    }

    //Esta funcion actualiza la barra y el texto de vida
    private void UpdateHealthUI(int current, int max)
    {
        //Calculo el porcentaje de vida que le queda al objeto,compruebo que la vida maxima sea mayor que cero para evitar dividir entre cero
        float healthPercentage =
            max > 0 ? (float)current / max : 0f;

        //Actualizo el relleno de la barra de vida
        if (healthFill != null)
        {
            //Limito el valor entre 0 y 1 porque es lo que utiliza fillAmount
            healthFill.fillAmount = Mathf.Clamp01(healthPercentage);
        }

        //Actualizo el texto con la vida actual y la vida maxima
        if (healthText != null)
        {
            healthText.text = $"{current} / {max}";
        }
    }
}
