
using System;
using UnityEngine;

//Enum que define los estados de la partida
public enum GameState
{
    Playing,
    Victory,
    Defeat
}

//Este script se encarga de controlar el estado general de la partida, comprueba si hemos ganado o perdido y detiene el juego cuando termina
public class GameManager : MonoBehaviour
{
    //Utilizo un Singleton para poder acceder al GameManager desde otros scripts
    public static GameManager Instance { get; private set; }

    [Header("Objectives")]
    //Componentes de vida de los objetos que tenemos que proteger
    [SerializeField] private Health playerHealth;
    [SerializeField] private Health generatorHealth;
    [SerializeField] private Health computerHealth;

    //Componente que controla el progreso delcorreo
    [SerializeField] private EmailTransmission emailTransmission;

    [Header("Gameplay Controls")]
    // Scripts que quiero desactivar cuando termina la partida
    [SerializeField] private Behaviour[] controlsToDisable;

    //Guardo el estado actual de la partida, que inicialmente es Playing
    public GameState State { get; private set; } = GameState.Playing;

    //Evento para avisar a otros scripts cuando termina la partida
    public event Action<GameState> OnGameEnded;

    private void Awake()
    {
        //Compruebo que no exista otro GameManager para evitar duplicados
        if (Instance != null && Instance != this)
        {
            enabled = false;
            Destroy(gameObject);
            return;
        }

        //Guardo esta instancia para poder acceder a ella desde otros scripts
        Instance = this;

    }

    private void OnEnable()
    {
        // Me suscribo a los eventos de muerte del jugador y los objetivos, si cualquiera de ellos muere, se pierde la partida
        if (playerHealth != null)
            playerHealth.OnDied += HandleDefeat;

        if (generatorHealth != null)
            generatorHealth.OnDied += HandleDefeat;

        if (computerHealth != null)
            computerHealth.OnDied += HandleDefeat;

        //Me suscribo al evento que avis cuando termina de enviarse el correo
        if (emailTransmission != null)
            emailTransmission.OnCompleted += HandleVictory;
    }


    private void OnDisable()
    {
        //Me desuscribo de los eventos para evitar llamadas cuando el objeto este desactivado
        if (playerHealth != null)
            playerHealth.OnDied -= HandleDefeat;

        if (generatorHealth != null)
            generatorHealth.OnDied -= HandleDefeat;

        if (computerHealth != null)
            computerHealth.OnDied -= HandleDefeat;

        if (emailTransmission != null)
            emailTransmission.OnCompleted -= HandleVictory;
    }

    private void OnDestroy()
    {
        //Limpio la referencia del Singleton si se destruye este GameManager
        if (Instance == this)
            Instance = null;
    }

    //Esta funcion se ejecuta cuando el correo termina de enviarse
    private void HandleVictory()
    {
        EndGame(GameState.Victory);
    }

    //Esta funcion se ejecuta cuando muere el jugador o alguno de los objetivos
    private void HandleDefeat()
    {
        EndGame(GameState.Defeat);
    }

    //Esta funcion se encarga de finalizar la partida
    private void EndGame(GameState result)
    {
        //Si la partida ya ha terminado, no vuelvo a ejecutar esta funcion
        if (State != GameState.Playing) return;

        //Guardo si hemos ganado o perdido
        State = result;

        //Desactivo los scripts de control que he asignado en el Inspector
        if (controlsToDisable != null)
        {
            foreach (Behaviour control in controlsToDisable)
            {
                //Compruebo que el script exista y que no sea el propio GameManager
                if (control != null && control != this)
                    control.enabled = false;
            }
        }

        //Pauso el tiempo del juego
        Time.timeScale = 0f;

        //Aviso al resto de sistemas de que ha terminado la partida y les paso el resultado
        OnGameEnded?.Invoke(result);
    }
}
