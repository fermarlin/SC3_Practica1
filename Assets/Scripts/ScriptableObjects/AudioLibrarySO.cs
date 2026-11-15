
using System;
using System.Collections.Generic;
using UnityEngine;

//Esta clase se encarga de guardar los datos de cada sonido utilizo Serializable para poder configurar sus valores desde el Inspector
[Serializable]
public class Sound
{
    //Nombre que utilizo para identificar el sonido
    public string name;

    //Archivo de audio que quiero reproducir
    public AudioClip clip;

    //Volumen del sonido, limitado entre 0 y 1
    [Range(0f, 1f)]
    public float volume = 1f;
}

/*Este ScriptableObject se encarga de guardar todos los sonidos del juego
Asi puedo anadir o modificar sonidos sin tener que cambiar el AudioManager*/
[CreateAssetMenu(fileName = "AudioLibrary",menuName = "Game/Audio Library")]
public class AudioLibrarySO : ScriptableObject
{
    //Lista donde guardo los diferentes sonidos y sus configuraciones
    [SerializeField] private List<Sound> sounds = new();

    //Esta funcion busca un sonido por su nombre y devuelve sus datos
    public Sound GetSound(string soundName)
    {
        //Recorro todos los sonidos que tengo guardados en la lista
        foreach (Sound sound in sounds)
        {
            // Si encuentro un sonido con el nombre indicado, lo devuelvo
            if (sound.name == soundName)
                return sound;
        }

        //Si no encuentro el sonido, devuelvo null
        return null;
    }
}
