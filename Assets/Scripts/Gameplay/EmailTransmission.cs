
using System;
using UnityEngine;

//Este script se encarga de controlar el tiempo que tarda el ordenador en enviar el correo
public class EmailTransmission : MonoBehaviour
{
    //Tiempo total que tiene que pasar para completar la transmision
    [SerializeField, Min(1f)]
    private float duration = 120f;

    // Guardo el tiempo que lleva transcurrido desde que empieza la transmision
    private float elapsedTime;

    /*Calculo el progreso de la transmision entre 0 y 1
    Utilizo Clamp01 para evitar que el valor sea mayor que 1*/
    public float Progress =>
        Mathf.Clamp01(elapsedTime / duration);

    // Esta propiedad indica si el ordenador ha terminado de enviar el correo
    public bool IsCompleted { get; private set; }

    //Evento para avisar a otros scripts cuando cambia el progreso de la transmision
    public event Action<float> OnProgressChanged;

    //Evento para avisar cuando se ha completado la transmision
    public event Action OnCompleted;

    private void Update()
    {
        //Si la transmision ya ha terminado, no sigo contando tiempo
        if (IsCompleted) return;

        //Voy sumando el tiempo transcurrido en cada frame, utilizo Min para que el tiempo nunca supere la duracion total
        elapsedTime = Mathf.Min(elapsedTime + Time.deltaTime, duration);

        //Aviso del progreso actual para que otros scripts puedan actualizar la UI
        OnProgressChanged?.Invoke(Progress);

        //Compruebo si ya ha pasado todo el tiempo necesario
        if (elapsedTime >= duration)
        {
            //Marco la transmision como completada
            IsCompleted = true;

            //Aviso al resto del juego de que el correo se ha enviado
            OnCompleted?.Invoke();
        }
    }
}
