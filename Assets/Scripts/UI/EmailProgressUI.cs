
using UnityEngine;
using UnityEngine.UI;
using TMPro;

//Este script se encarga de mostrar el progreso del envio del mensaje en la interfaz utilizo los eventos de EmailTransmission para actualizar la barra y los textos
public class EmailProgressUI : MonoBehaviour
{
    [Header("References")]
    //Referencia al script que controla el progreso del envio del mensaje
    [SerializeField] private EmailTransmission transmission;

    [Header("UI")]
    //Imagen que utilizo como barra de progreso
    [SerializeField] private Image progressFill;

    //Texto donde muestro el porcentaje del mensaje enviado
    [SerializeField] private TMP_Text progressText;

    //Texto que indica si el mensaje se esta enviando o ya se ha completado
    [SerializeField] private TMP_Text statusText;

    private void OnEnable()
    {
        //Si no tengo asignado el EmailTransmission, no hago nada
        if (transmission == null)
            return;

        //Me suscribo al evento que avisa cuando cambia el progreso
        transmission.OnProgressChanged += UpdateProgress;

        // Me suscribo al evento que avisa cuando termina el envio
        transmission.OnCompleted += HandleCompleted;

        //Actualizo la barra y el porcentaje con el progreso actual, asi la interfaz muestra el valor correcto desde que se activa
        UpdateProgress(transmission.Progress);

        //Si el mensaje ya se ha enviado, muestro el estado completado
        if (transmission.IsCompleted)
            HandleCompleted();

        //Si todavia no ha terminado, muestro que el mensaje se esta enviando
        else if (statusText != null)
            statusText.text = "ENVIANDO MENSAJE...";
    }

    private void OnDisable()
    {
        //Si no tengo asignado el EmailTransmission, no hago nada
        if (transmission == null)
            return;

        //Me desuscribo de los eventos cuando se desactiva la interfaz
        transmission.OnProgressChanged -= UpdateProgress;
        transmission.OnCompleted -= HandleCompleted;
    }

    //Esta funcion actualiza la barra y el porcentaje del mensaje enviado
    private void UpdateProgress(float progress)
    {
        //Actualizo el relleno de la barra utilizando un valor entre 0 y 1
        if (progressFill != null)
            progressFill.fillAmount = Mathf.Clamp01(progress);

        //Convierto el progreso a porcentaje y lo muestro en pantalla
        if (progressText != null)
            progressText.text =
                $"{Mathf.FloorToInt(progress * 100f)}%";
    }

    // Esta funcion se ejecuta cuando el mensaje termina de enviarse
    private void HandleCompleted()
    {
        //Establezco el progreso al 100%
        UpdateProgress(1f);

        // Cambio el texto para indicar que el mensaje se ha enviado
        if (statusText != null)
            statusText.text = "MENSAJE ENVIADO";
    }
}
