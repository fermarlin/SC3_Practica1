using UnityEngine;

[CreateAssetMenu(fileName = "WeaponConfig",menuName = "Game/Weapon Config")]

public class WeaponConfigSO : ScriptableObject
{
    [SerializeField] private int damage=10;
    [SerializeField] private int range=200;
    [SerializeField] private float fireRate=.5f;
    [SerializeField] private int bullets=20;
    [SerializeField] private float reloadTime=1.5f;
    [SerializeField] private int pelletCount=1;
    [SerializeField] private float spreadAngle=0;
    [SerializeField] private GameObject weaponPrefab;

    public int Damage => damage;
    public int Range => range;
    public float FireRate => fireRate;
    public int Bullets => bullets;
    public float ReloadTime => reloadTime;    
    public int PelletCount => pelletCount;
    public float SpreadAngle => spreadAngle;
    public GameObject WeaponPrefab => weaponPrefab;
    
}
