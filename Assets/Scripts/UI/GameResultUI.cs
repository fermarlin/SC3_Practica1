
using UnityEngine;
using UnityEngine.SceneManagement;

//Este script se encarga de mostrar las pantallas de victoria y derrota tambien reiniciar la partida o salir del juego
public class GameResultUI : MonoBehaviour
{
    //Paneles que muestro cuando gano o pierdo la partida
    [SerializeField] private GameObject victoryPanel;
    [SerializeField] private GameObject defeatPanel;

    //Referencia al GameManager para saber cuando termina la partida
    private GameManager gameManager;

    private void Awake()
    {
        //Oculto los paneles al iniciar el juego
        if (victoryPanel != null)
            victoryPanel.SetActive(false);

        if (defeatPanel != null)
            defeatPanel.SetActive(false);
    }

    private void OnEnable()
    {
        //Intento suscribirme al evento del GameManager
        SubscribeToGameManager();
    }

    private void Start()
    {
        //Vuelvo a intentar suscribirme por si el GameManager todavia no estaba disponible cuando se ejecuto OnEnable
        SubscribeToGameManager();
    }

    private void OnDisable()
    {
        //Me desuscribo del evento cuando se desactiva la interfaz
        if (gameManager != null)
        gameManager.OnGameEnded -= ShowResult;

        //Limpio la referencia al GameManager
        gameManager = null;
    }

    //Esta funcion se encarga de suscribirse al evento de final de partida
    private void SubscribeToGameManager()
    {
        //Si ya estoy suscrito, no hago nada
        if (gameManager != null) return;

        //Pillo la instancia del GameManager
        gameManager = GameManager.Instance;

        //Si todavia no existe, no puedo suscribirme
        if (gameManager == null)
            return;

        //Me suscribo al evento que avisa cuando termina la partida
        gameManager.OnGameEnded += ShowResult;

    }

    //Esta funcion muestra el panel correspondiente al resultado de la partida
    private void ShowResult(GameState result)
    {
        //Si he ganado, activo el panel de victoria
        if (victoryPanel != null)
            victoryPanel.SetActive(result == GameState.Victory);

        //Si he perdido, activo el panel de derrota
        if (defeatPanel != null)
            defeatPanel.SetActive(result == GameState.Defeat);

        //Desbloqueo y muestro el cursor para poder pulsar los botones
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    //Esta funcion se ejecuta cuando pulso el boton de reiniciar
    public void RestartGame()
    {
        //Restauro el tiempo del juego porque el GameManager lo habia pausado
        Time.timeScale = 1f;

        // Vuelvo a cargar la escena actual para comenzar una nueva partida
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    //Esta funcion se ejecuta cuando pulso el boton de salir
    public void QuitGame()
    {
        //Restauro el tiempo del juego antes de salir
        Time.timeScale = 1f;

        //Cierro la aplicacion
        Application.Quit();
    }
}
