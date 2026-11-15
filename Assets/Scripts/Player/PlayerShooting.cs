
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

//Este script se encarga de gestionar las armas, los disparos y la municion del jugador, tambien controla el cambio de arma por la llave inglesa cuando estoy reparando
public class PlayerShooting : MonoBehaviour
{
    //Capas con las que pueden chocar los disparos
    [SerializeField] private LayerMask shootMask;

    //Lista de armas que puede utilizar el jugador, cada arma tiene sus valores guardados en un ScriptableObject
    [SerializeField] private List<WeaponConfigSO> weapons;

    //Objeto donde instancio el modelo del arma equipada
    [SerializeField] private Transform weaponHolder;

    [Header("Impact Effects")]
    //Particulas que aparecen cuando una bala golpea una superficie
    [SerializeField] private ParticleSystem impactEffectPrefab;

    [Header("Repair Tool")]
    //Modelo de la llave inglesa que utilizo al reparar el generador
    [SerializeField] private GameObject wrenchModel;

    //Guardo la configuracion del arma que tengo equipada
    private WeaponConfigSO currentWeapon;

    //Sistema de inputs y componente que controla las interacciones del jugador
    private PlayerInputs playerInputs;
    private PlayerInteractor playerInteractor;

    //Guardo cuando puedo volver a disparar para respetar la cadencia del arma
    private float nextFireTime;

    //Guardo las balas de cada arma por separado, utilizo un diccionario para relacionar cada arma con su municion actual
    private readonly Dictionary<WeaponConfigSO, int> weaponAmmo = new();

    //Booleanos para comprobar si estoy recargando o reparando
    private bool isReloading;
    private bool isRepairing;

    //Referencia a la camara y al modelo del arma equipada
    private Transform cameraTransform;
    private GameObject currentWeaponModel;

    //Evento para avisar cuando empieza una recarga
    public event Action OnReloadStarted;

    // Evento para avisar cuando se realiza un disparo
    public event Action OnShotFired;

    //Evento para avisar a la UI cuando cambia la municion, paso las balas actuales y la capacidad del cargador
    public event Action<int, int> OnAmmoChanged;

    //Permito que otros scripts consulten las balas del arma equipada
    public int CurrentAmmo => currentWeapon != null && weaponAmmo.TryGetValue(currentWeapon, out int ammo) ? ammo : 0;

    // Permito consulta la capacidad maxima del cargador
    public int MagazineSize => currentWeapon != null ? currentWeapon.Bullets : 0;

    private void Awake()
    {
        //Creo los controles del jugador y pillo el componente de interaccion
        playerInputs = new PlayerInputs();
        playerInteractor = GetComponent<PlayerInteractor>();

        //Recorro las armas disponibles para guardar su municion inicial
        foreach (WeaponConfigSO weapon in weapons)
        {
            //Compruebo que el arma exista y que no este ya en el diccionario
            if (weapon != null && !weaponAmmo.ContainsKey(weapon))
            weaponAmmo.Add(weapon, weapon.Bullets);
        }

        //Equipo la primera arma de la lista al comenzar
        if (weapons.Count > 0 && weapons[0] != null)
        EquipWeaponByIndex(0);

        //Guardo la camara principal para calcular la direccion de los disparos
        if (Camera.main != null)
            cameraTransform = Camera.main.transform;

        //Oculto la llave inglesa porque inicialmente tengo un arma equipada
        if (wrenchModel != null)
            wrenchModel.SetActive(false);
    }

    private void OnEnable()
    {
        //Activo los controles del jugador
        playerInputs.PlayerActionMap.Enable();

        //Me suscribo a los eventos de disparo, recarga y cambio de arma
        playerInputs.PlayerActionMap.Shoot.performed += Shoot;
        playerInputs.PlayerActionMap.Recharge.performed += Recharge;
        playerInputs.PlayerActionMap.EquipPistol.performed += EquipPistol;
        playerInputs.PlayerActionMap.EquipShotgun.performed += EquipShotgun;

        //Me suscribo al evento que indica cuando empiezo o dejo de reparar
        if (playerInteractor != null)
            playerInteractor.OnRepairingChanged += SetRepairing;
    }

    private void OnDisable()
    {
        //Me desuscribo de los eventos de los controles
        playerInputs.PlayerActionMap.Shoot.performed -= Shoot;
        playerInputs.PlayerActionMap.Recharge.performed -= Recharge;
        playerInputs.PlayerActionMap.EquipPistol.performed -= EquipPistol;
        playerInputs.PlayerActionMap.EquipShotgun.performed -= EquipShotgun;

        // Me desuscribo del evento de reparacion
        if (playerInteractor != null)
            playerInteractor.OnRepairingChanged -= SetRepairing;

        //Desactivo los controles
        playerInputs.PlayerActionMap.Disable();

        //Restauro el arma si estaba reparando
        SetRepairing(false);
    }

    private void OnDestroy()
    {
        //Libero los recursos del sistema de inputs
        playerInputs.Dispose();
    }

    // Esta funcion se encarga de cambiar entre el arma y la llave inglesa
    private void SetRepairing(bool value)
    {
        //Si el estado de reparacion no ha cambiado, no hago nada
        if (isRepairing == value)
            return;

        //Guardo si estoy reparando
        isRepairing = value;

        // Si estoy reparando oculto el arma, si no la vuelvo a mostrar
        if (weaponHolder != null)
            weaponHolder.gameObject.SetActive(!isRepairing);

        //Activo la llave inglesa mientras estoy reparando
        if (wrenchModel != null)
            wrenchModel.SetActive(isRepairing);
    }

    //Esta funcion comprueba si puedo empezar a recargar el arma
    private void Recharge(InputAction.CallbackContext context)
    {
        // Si estoy reparando o no tengo arma equipada, no recargo
        if (isRepairing || currentWeapon == null)
            return;

        //Si ya estoy recargando o tengo el cargador lleno, no hago nada
        if (isReloading || weaponAmmo[currentWeapon] >= currentWeapon.Bullets)
            return;

        //Inicio la corrutina de recarga
        StartCoroutine(ReloadRoutine());
    }

    //Utilizo una corrutina para esperar el tiempo de recarga del arma
    private IEnumerator ReloadRoutine()
    {
        //Marco que estoy recargando para impedir otros disparos y recargas
        isReloading = true;

        //Guardo el arma que estoy recargando
        WeaponConfigSO reloadingWeapon = currentWeapon;

        //Aviso de que ha empezado la recarga para reproducir su animacion
        OnReloadStarted?.Invoke();

        //Reproduzco el sonido de recarga
        if (AudioManager.Instance != null)
        AudioManager.Instance.PlayAudio2D("Reload");

        //Espero el tiempo de recarga configurado en el ScriptableObject
        yield return new WaitForSeconds(reloadingWeapon.ReloadTime);

        //Rellen el cargador del arma
        weaponAmmo[reloadingWeapon] = reloadingWeapon.Bullets;

        //Indico que ya he terminado de recargar
        isReloading = false;

        //Aviso a la UI para que actualice el contador de balas
        NotifyAmmoChanged();
    }

    //Esta funcion se ejecuta cuando el jugador intenta disparar
    private void Shoot(InputAction.CallbackContext context)
    {
        //Compruebo que no este reparando y que tenga un arma y una camara
        if (isRepairing || currentWeapon == null || cameraTransform == null)
            return;

        //Compruebo que haya pasado el tiempo entre disparos, tambien que tenga balas y que no este recargando
        if (nextFireTime > Time.time || weaponAmmo[currentWeapon] <= 0 || isReloading)
            return;

        //Reproduzco los efectos visuales de disparo del arma equipada
        if (currentWeaponModel != null && currentWeaponModel.TryGetComponent(out WeaponView view))
        {
            view.PlayShootEffect();
        }

        //Calculo cuando podre realizar el siguiente disparo
        nextFireTime = Time.time + currentWeapon.FireRate;

        //Calculo la mitad de la dispersion para repartir las balas hacia ambos lados de la direccion de disparo
        float halfSpread = currentWeapon.SpreadAngle / 2f;

        //Repito el Raycast segun el numero de proyectiles del arma, esto me permite utilizar el mismo sistema para la pistola y la escopeta
        for (int i = 0; i < currentWeapon.PelletCount; i++)
        {
            //Genero una desviacion horizontal aleatoria
            float horizontal = UnityEngine.Random.Range(-halfSpread,halfSpread);

            //Genero una desviacion vertical aleatoria
            float vertical = UnityEngine.Random.Range(-halfSpread,halfSpread);

            //Calculo la direccion del disparo utilizando la rotacion de la camara y la dispersion que he generado
            Vector3 direction =cameraTransform.rotation * Quaternion.Euler(vertical, horizontal, 0f) * Vector3.forward;

            //Lanzo un Raycast desde la camara para comprobar si la bala golpea algo
            if (Physics.Raycast( cameraTransform.position, direction, out RaycastHit hit, currentWeapon.Range, shootMask))
            {
                //Instancio las particulas en el punto donde ha impactado la bala
                if (impactEffectPrefab != null)
                {
                    Instantiate(impactEffectPrefab, hit.point + hit.normal * 0.02f, Quaternion.LookRotation(hit.normal));
                }

                //Busco si el objeto golpeado implementa la interfaz IDamageable, compruebo tambien sus padres por si el collider esta en un hijo
                IDamageable hitDamage =
                    hit.collider.GetComponentInParent<IDamageable>();

                // Si puede recibir dano, aplico el valor configurado en el arma
                if (hitDamage != null)
                    hitDamage.TakeDamage(currentWeapon.Damage);
            }
        }

        //Resto una bala del cargador despues de disparar aaunque sea una escopeta con varios perdigones, solo consume un cartucho
        weaponAmmo[currentWeapon]--;

        //Reproduzco el sonido correspondiente al arma equipada
        if (AudioManager.Instance != null)
        {
            //Compruebo si tengo la pistola o la escopeta
            string soundName = currentWeapon == weapons[0]? "PistolShot": "ShotgunShot";

            AudioManager.Instance.PlayAudio2D(soundName);
        }

        //Aviso a otros scripts para que reproduzcan la animacion de disparo
        OnShotFired?.Invoke();

        //Actualizo el contador de municion de la UI
        NotifyAmmoChanged();
    }

    //Esta funcion equipa la pistola
    private void EquipPistol(InputAction.CallbackContext context)
    {
        EquipWeaponByIndex(0);
    }

    //Esta funcion equipa la escopeta
    private void EquipShotgun(InputAction.CallbackContext context)
    {
        EquipWeaponByIndex(1);
    }

    //Esta funcion comprueba que puedo cambiar al arma seleccionada
    private void EquipWeaponByIndex(int index)
    {
        //Si estoy reparando o el indice no es valido, no cambio de arma
        if (isRepairing || index < 0 || index >= weapons.Count)
            return;

        //Intento equipar el arma correspondiente
        EquipWeapon(weapons[index]);
    }

    //Esta funcion se encarga de cambiar el arma que tengo equipada
    private void EquipWeapon(WeaponConfigSO newWeapon)
    {
        //Compruebo que no este reparando ni recargando, tambien que el arma exista y que no sea la que ya tengo equipada
        if (isRepairing || isReloading ||newWeapon == null ||currentWeapon == newWeapon)
            return;

        //Compruebo que el arma este registrada en el diccionaro de municion
        if (!weaponAmmo.ContainsKey(newWeapon))
            return;

        //Guardo la nueva arma equipada
        currentWeapon = newWeapon;

        //Cambio su modelo visual
        UpdateWeaponModel();

        //Aviso a la UI para que muestre la municion del arma seleccionada
        NotifyAmmoChanged();
    }

    //Esta funcion se encarga de actualizar el modelo del arma
    private void UpdateWeaponModel()
    {
        //Si ya tengo un modelo equipado, lo destruyo
        if (currentWeaponModel != null)
        {
            Destroy(currentWeaponModel);
            currentWeaponModel = null;
        }

        //Compruebo que tenga donde colocar el arma y un prefab asignado
        if (weaponHolder == null ||currentWeapon == null ||currentWeapon.WeaponPrefab == null)
            return;

        // Instancio el modelo del arma como hijo del WeaponHolder
        currentWeaponModel = Instantiate( currentWeapon.WeaponPrefab,weaponHolder);
    }

    //Esta funcion avisa a la UI cuando cambia la municion
    private void NotifyAmmoChanged()
    {
        //Paso las balas actuales y la capacidad maxima del cargador
        OnAmmoChanged?.Invoke(CurrentAmmo, MagazineSize);
    }
}
