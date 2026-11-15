
using UnityEngine;

// Este script se encarga de controlar las animaciones del jugador, utilizo eventos para que las animaciones reaccionen a las acciones de otros scripts
public class PlayerAnimator : MonoBehaviour
{
    //Componentes que necesito para controlar las animaciones y el movimiento
    [SerializeField] private Animator animator;
    [SerializeField] private Rigidbody playerRb;
    [SerializeField] private PlayerController playerController;

    //Scripts que utilizo para sabe cuando el jugador dispara, recarga o repara
    private PlayerShooting playerShooting;
    private PlayerInteractor playerInteractor;

    //Componente que controla la vida del jugador
    private Health health;

    //Particulas que aparecen cuando el jugador recibe dano
    [SerializeField] private ParticleSystem hitEffectPrefab;

    //Guardo la velocidad que utilizo para las animaciones de movimiento
    private float speed;

    //Compruebo si el jugador esta reparando para bloquear algunas animaciones
    private bool isRepairing;

    // Guardo los identificadores de los parametros del Animator para evitar utilizar strings repetidamente
    private static readonly int ReloadHash = Animator.StringToHash("Reload");
    private static readonly int ShootHash = Animator.StringToHash("Shoot");
    private static readonly int JumpHash = Animator.StringToHash("Jump");
    private static readonly int RepairHash = Animator.StringToHash("IsRepairing");
    private static readonly int HitHash = Animator.StringToHash("Hit");

    private void Awake()
    {
        //Pillo los componentes del jugador
        playerRb = GetComponent<Rigidbody>();
        playerShooting = GetComponent<PlayerShooting>();
        playerInteractor = GetComponent<PlayerInteractor>();
        health = GetComponent<Health>();
    }

    private void OnEnable()
    {
        // Me suscribo a los eventos de disparo y recarga
        if (playerShooting != null)
        {
            playerShooting.OnReloadStarted += PlayReload;
            playerShooting.OnShotFired += PlayShoot;
        }

        //Me suscribo al evento de salto
        if (playerController != null) playerController.OnJumped += PlayJump;

        //Me suscribo al evento que indica si el jugador esta reparando
        if (playerInteractor != null) playerInteractor.OnRepairingChanged += SetRepairing;

        //Me suscribo al evento de dano para reproducir la animacion de golpe
        if (health != null) health.OnDamaged += PlayHit;
    }

    private void OnDisable()
    {
        //Me desuscribo de los eventos cuando se desactiva el script
        if (playerShooting != null)
        {
            playerShooting.OnReloadStarted -= PlayReload;
            playerShooting.OnShotFired -= PlayShoot;
        }

        if (playerController != null) playerController.OnJumped -= PlayJump;

        if (playerInteractor != null) playerInteractor.OnRepairingChanged -= SetRepairing;

        if (health != null) health.OnDamaged -= PlayHit;

        //Quito el estado de reparacion
        SetRepairing(false);
    }

    private void Update()
    {
        //Calculo la velocidad horizontal del jugador, ignoro el eje Y para que los saltos no afecten a la animacion de movimiento
        Vector3 horizontalVelocity = new Vector3(playerRb.linearVelocity.x, 0f, playerRb.linearVelocity.z);

        //Normalizo la velocidad entre 0 y 1 utilizando la velocidad maxima del jugador
        speed = Mathf.InverseLerp(0f, playerController.MoveSpeed, horizontalVelocity.magnitude);

        //Actualizo la velocidad del Animator suavizando el cambio entre animaciones
        animator.SetFloat("Speed", speed, 0.1f, Time.deltaTime);

        //Indico al Animator si el jugador esta apuntando
        animator.SetBool("IsAiming", playerController.IsAiming);
    }

    //Esta funcion reproduce la animacion de recarga
    private void PlayReload()
    {
        //Si esta reparando, no permito reproducir la recarga
        if (!isRepairing) animator.SetTrigger(ReloadHash);
    }

    //Esta funcion reproduce la animacion de disparo
    private void PlayShoot()
    {
        //Si esta reparando, no permito reproducir el disparo
        if (!isRepairing) animator.SetTrigger(ShootHash);
    }

    //Esta funcion reproduce la animacion de salto
    private void PlayJump()
    {
        //Si esta reparando, no reproduzco la animacion de salto
        if (!isRepairing) animator.SetTrigger(JumpHash);
    }

    //Esta funcion se ejecuta cuando el jugador recibe dano
    private void PlayHit(int damage)
    {
        //Activo la animacion de recibir dano
        animator.SetTrigger(HitHash);

        //Reproduzco el sonido de dano del jugador
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayAudio2D("PlayerHit");
        }

        //Instancio las particulas de impacto en la posicion del jugador
        if (hitEffectPrefab != null)
        {
            Instantiate(hitEffectPrefab, transform.position, Quaternion.identity);
        }
    }

    //Esta funcion se encarga de activar o desactivar la animacion de reparacion
    private void SetRepairing(bool value)
    {
        //Guardo si el jugador esta reparando
        isRepairing = value;

        //Si empieza a reparar, limpio los triggers de disparo y recarga
        if (isRepairing)
        {
            animator.ResetTrigger(ReloadHash);
            animator.ResetTrigger(ShootHash);
        }

        //Actualizo el booleano del Animator para entrar o salir de la animacion
        animator.SetBool(RepairHash, isRepairing);
    }
}
