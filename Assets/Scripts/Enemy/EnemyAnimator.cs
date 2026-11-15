
using System;
using UnityEngine;
using UnityEngine.AI;

//Este script se encarga de las animaciones del enemigo
[RequireComponent(typeof(Animator))]
public class EnemyAnimator : MonoBehaviour
{
    // Guardamos los identificadores de los parametros del Animator para evitar utilizar strings repetidamente.
    private static readonly int SpeedHash = Animator.StringToHash("SpeedMagnitude");
    private static readonly int AttackHash = Animator.StringToHash("Attack");
    private static readonly int HitHash = Animator.StringToHash("Hit");
    private static readonly int DieHash = Animator.StringToHash("Die");

    //El componente que anima al enemigo
    private Animator animator;
    //El componente que se encarga de mover al enemigo por el nivel
    private NavMeshAgent navAgent;

    //Bloquea las animaciones de movimiento y el inicio de ataques mientras el enemigo recibe dano o esta muriendo.
    private bool animationLocked;
    //Esta variable determina si el enemigo esta muerto
    private bool isDead;
    //Evento que notifica que ha terminado la animacion de muerte.
    public event Action DeathAnimationFinished;

    private void Awake()
    {
        //Pillo el animator
        animator = GetComponent<Animator>();
        //Pillo el navAgent del padre (Prefiero separar las meshes de la logica, el player esta igual si te fijas)
        navAgent = GetComponentInParent<NavMeshAgent>();
    }

    private void Update()
    {
        //Funcion del movimiento
        UpdateMovement();
    }

    //Esta funcion se encarga de aplicar al animator la velocidad actual
    private void UpdateMovement()
    {
        float speed = 0f;

        if (!animationLocked && navAgent != null && navAgent.enabled && navAgent.isOnNavMesh)
        {
            speed = navAgent.velocity.magnitude;
        }

        animator.SetFloat(SpeedHash, speed);
    }

    //Esta funcion se encarga de decirle al animator que esta atacando
    public void SetAttacking(bool attacking)
    {
    //Impide atacar si esta muerto o si las animaciones estan bloqueadas.
        if (isDead || (animationLocked && attacking)) 
            return;

        animator.SetBool(AttackHash, attacking);
    }

    
    //Esta funcion se encarga de decirle al animator que esta recibiendo dano
    public void PlayHit()
    {
        if (isDead)
            return;

        animationLocked = true;

        animator.SetBool(AttackHash, false);
        animator.SetFloat(SpeedHash, 0f);

        animator.ResetTrigger(HitHash);
        animator.SetTrigger(HitHash);
    }

    //Esta funcion se encarga de resetear el bloqueo del enemigo, para poder moverse
    public void RecoverFromHit()
    {
        if (!isDead)
            animationLocked = false;
    }

    //Esta funcion se encarga de decirle al animator que el enemigo esta muerto
    public void PlayDeath()
    {
        if (isDead)
            return;

        //Limpio el animator por si estaba con otra animacion y aviso al animator que esta muerto
        isDead = true;
        animationLocked = true;

        animator.SetBool(AttackHash, false);
        animator.SetFloat(SpeedHash, 0f);

        animator.ResetTrigger(HitHash);
        animator.SetTrigger(DieHash);
    }

    //Metodo que llamo mediante un Animation Event al finalizar la animacion de muerte.
    public void OnDeathAnimationFinished()
    {
        DeathAnimationFinished?.Invoke();
    }
}
