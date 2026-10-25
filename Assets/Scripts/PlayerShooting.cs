using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using System.Collections.Generic;

public class PlayerShooting : MonoBehaviour
{
    [SerializeField] private LayerMask shootMask;
    [SerializeField] private List<WeaponConfigSO> weapons;

    private WeaponConfigSO currentWeapon;
    private PlayerInputs playerInputs;
    private float nextFireTime = 0;
    private Dictionary<WeaponConfigSO, int> weaponAmmo = new Dictionary<WeaponConfigSO, int>();
    private bool isReloading;
    private Transform cameraTransform;
    void Awake()
    {
        foreach (WeaponConfigSO weapon in weapons){
        if (weapon != null && !weaponAmmo.ContainsKey(weapon))
            {
                weaponAmmo.Add(weapon, weapon.Bullets);
            }
        }

        if (weapons.Count > 0 && weapons[0] != null)
        {
            currentWeapon = weapons[0];
        }

        if(Camera.main!=null){
            cameraTransform = Camera.main.transform;
        }


        playerInputs = new PlayerInputs();
        
    }


    void OnEnable()
    {
        playerInputs.PlayerActionMap.Enable();
        playerInputs.PlayerActionMap.Shoot.performed += Shoot;
        playerInputs.PlayerActionMap.Recharge.performed += Recharge;
        playerInputs.PlayerActionMap.EquipPistol.performed += EquipPistol;
        playerInputs.PlayerActionMap.EquipShotgun.performed += EquipShotgun;



    }

    void OnDisable(){
        playerInputs.PlayerActionMap.Shoot.performed -= Shoot;
        playerInputs.PlayerActionMap.Recharge.performed -= Recharge;
        playerInputs.PlayerActionMap.EquipPistol.performed -= EquipPistol;
        playerInputs.PlayerActionMap.EquipShotgun.performed -= EquipShotgun;


        playerInputs.PlayerActionMap.Disable();
    }

    void OnDestroy()
    {
        playerInputs.Dispose();
    }

    void Recharge(InputAction.CallbackContext context){
        if (currentWeapon == null) return;
        if (isReloading ||  weaponAmmo[currentWeapon] >= currentWeapon.Bullets)
        {
            return;
        } 
        StartCoroutine(ReloadRoutine());

    }

    void Shoot(InputAction.CallbackContext context){
        if(nextFireTime>Time.time|| weaponAmmo[currentWeapon]<=0||isReloading||currentWeapon == null||cameraTransform == null) return;
        
        nextFireTime = currentWeapon.FireRate+Time.time;
        float halfSpread = currentWeapon.SpreadAngle / 2f;

        for (int i = 0; i < currentWeapon.PelletCount; i++)
        {

            float horizontal = Random.Range(-halfSpread, halfSpread);
            float vertical = Random.Range(-halfSpread, halfSpread);

            Vector3 direction = cameraTransform.rotation * Quaternion.Euler(vertical, horizontal, 0) * Vector3.forward;

            if(Physics.Raycast(cameraTransform.position, direction, out RaycastHit hit, currentWeapon.Range, shootMask)){
                IDamageable hitDamage = hit.collider.gameObject.GetComponent<IDamageable>();
                if (hitDamage != null)
                {
                    hitDamage.TakeDamage(currentWeapon.Damage);
                }
            }
        }
        weaponAmmo[currentWeapon]--;
        
    }

    void EquipPistol(InputAction.CallbackContext context)
    {
        EquipWeaponByIndex(0);
    }

    void EquipShotgun(InputAction.CallbackContext context)
    {
        EquipWeaponByIndex(1);
    }


    void EquipWeaponByIndex(int index)
    {
        if (index < 0 || index >= weapons.Count)
            return;

        EquipWeapon(weapons[index]);
    }

    void EquipWeapon(WeaponConfigSO newWeapon)
    {
        if (isReloading || newWeapon == null || currentWeapon == newWeapon)
            return;

        if (!weaponAmmo.ContainsKey(newWeapon))
        {
            return;
        }

        currentWeapon = newWeapon; 

    }
        

    IEnumerator ReloadRoutine()
    {
        isReloading = true;

        WeaponConfigSO reloadingWeapon = currentWeapon;

        yield return new WaitForSeconds(reloadingWeapon.ReloadTime);

        weaponAmmo[reloadingWeapon] = reloadingWeapon.Bullets;

        isReloading = false;
    }
}
