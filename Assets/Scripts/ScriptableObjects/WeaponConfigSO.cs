
using UnityEngine;

//Este ScriptableObject se encarga de guardar los valores de configuracion de cada arma, asi puedo crear distintas armas sin tener que modificar el PlayerShooting
[CreateAssetMenu(fileName = "WeaponConfig", menuName = "Game/Weapon Config")]
public class WeaponConfigSO : ScriptableObject
{
    //Cantidad de dano que aplica cada proyectil al golpear un objetivo
    [SerializeField] private int damage = 10;

    //Distancia maxima que pueden alcanzar los disparos
    [SerializeField] private int range = 200;

    //Tiempo minimo que tiene que pasar entre un disparo y el siguiente
    [SerializeField] private float fireRate = .5f;

    //Cantidad de balas que caben en el cargador
    [SerializeField] private int bullets = 20;

    //Tiempo que tarda el arma en recargarse
    [SerializeField] private float reloadTime = 1.5f;

    //Numero de proyectiles que disparo cada vez que pulso el boton
    [SerializeField] private int pelletCount = 1;

    //Angulo de dispersion de los proyectiles
    [SerializeField] private float spreadAngle = 0;

    //Prefab del modelo del arma que voy a equipar en el jugador
    [SerializeField] private GameObject weaponPrefab;

    //Permito que otros scripts consulten los valores del arma, sin que puedan modificarlos directamente
    public int Damage => damage;
    public int Range => range;
    public float FireRate => fireRate;
    public int Bullets => bullets;
    public float ReloadTime => reloadTime;
    public int PelletCount => pelletCount;
    public float SpreadAngle => spreadAngle;
    public GameObject WeaponPrefab => weaponPrefab;
}
