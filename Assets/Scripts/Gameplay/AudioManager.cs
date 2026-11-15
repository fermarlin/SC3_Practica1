
using UnityEngine;

//Este script se encarga de reproducir los sonidos del juego, tanto en 2D como en 3D
public class AudioManager : MonoBehaviour
{
    // Utilizo un Singleton para poder acceder al AudioManager desde cualquier script
    public static AudioManager Instance { get; private set; }

    //ScriptableObject donde tengo guardados los sonidos del juego
    [SerializeField] private AudioLibrarySO audioLibrary;

    private void Awake()
    {
        //Compruebo si ya existe otro AudioManager para evitar  duplicados
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        // Guardo esta instancia para poder acceder a ella desde otros scripts
        Instance = this;
    }

    //Esta funcion reproduce un sonido en 3D desde una posicion del escenario
    public void PlaySound(string soundName, Vector3 position)
    {
        PlayAudio(soundName, position, 1f);
    }

    //Esta funcion reproduce un sonido en 2D, sin depender de la posicion del jugador
    public void PlayAudio2D(string soundName)
    {
        PlayAudio(soundName, Vector3.zero, 0f);
    }

    //Esta funcion se encarga de buscar, configurar y reproducir el sonido
    private void PlayAudio(
        string soundName,
        Vector3 position,
        float spatialBlend)
    {
        //Si no tengo asignada la libreria de sonidos, no hago nada
        if (audioLibrary == null)
            return;

        //Busco el sonido por su nombre dentro del ScriptableObject
        Sound sound = audioLibrary.GetSound(soundName);

        //Si no existe el sonido o no tiene un AudioClip asignado, no lo reproduzco
        if (sound == null || sound.clip == null)
            return;

        //Creo un GameObject temporal para reproducir el sonido
        GameObject obj = new GameObject($"Audio_{soundName}");
        obj.transform.position = position;

        //Anado un AudioSource al objeto que acabo de crear
        AudioSource source = obj.AddComponent<AudioSource>();

        //Configuro el AudioSource con los datos de la libreria
        source.playOnAwake = false;
        source.clip = sound.clip;
        source.volume = sound.volume;

        //Determino si el sonido es 2D o 3D, con 0 es 2D y con 1 es completamente 3D
        source.spatialBlend = spatialBlend;

        //Configuro las distancias a las que se escucha el sonido en 3D
        source.minDistance = 6f;
        source.maxDistance = 40f;

        //Cambio ligeramente el pitch para que los sonidos no sean siempre identicos
        source.pitch = Random.Range(0.92f, 1.08f);

        //Reproduzco el sonido
        source.Play();

        //Destruyo el GameObject cuando termina el audio, divido la duracion entre el pitch porque este modifica la velocidad de reproduccion
        Destroy(obj,sound.clip.length / source.pitch + 0.1f);
    }

    private void OnDestroy()
    {
        //Si se destruye este AudioManager, limpio la referencia del Singleton
        if (Instance == this)
            Instance = null;
    }
}
