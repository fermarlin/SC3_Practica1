
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using Unity.Behavior;

//Este script se encarga de gestionar las reacciones del enemigo cuando recibe dano o muere
public class EnemyDamageReactions : MonoBehaviour
{
    [Header("Explosion")]
    //Particulas que aparecen cuando el enemigo explota
    [SerializeField] private ParticleSystem explosionPrefab;

    [Header("Health Drop")]
    //Prefab del botiquin que puede soltar el enemigo al morir
    [SerializeField] private GameObject healthPickupPrefab;

    //Probabilidad de que el enemigo suelte el botiquin
    [SerializeField, Range(0f, 1f)]
    private float dropChance = 1f;

    //Componentes que necesito para controlar la vida, las animaciones y la IA del enemigo
    private Health health;
    private EnemyData enemyData;
    private EnemyAnimator enemyAnimator;
    private BehaviorGraphAgent behaviorAgent;
    private NavMeshAgent navAgent;

    //Guardo la corrutina por si necesito detenerla cuando el enemigo recibe otro golpe
    private Coroutine hitCoroutine;

    //Compruebo si el enemigo esta muerto para evitar ejecutar acciones despues
    private bool isDead;

    //Esta propiedad permite que otros scripts sepan si el enemigo esta aturdido
    public bool IsStunned { get; private set; }

    private void Awake()
    {
        //Pillo los componentes del enemigo
        health = GetComponent<Health>();
        enemyData = GetComponent<EnemyData>();
        behaviorAgent = GetComponent<BehaviorGraphAgent>();
        navAgent = GetComponent<NavMeshAgent>();

        //Busco el animator en los hijos porque tengo separado el modelo de la logica
        enemyAnimator = GetComponentInChildren<EnemyAnimator>(true);
    }

    private void OnEnable()
    {
        //Me suscribo a los eventos de Health para saber cuando recibe dano o muere
        health.OnDamaged += HandleDamaged;
        health.OnDied += HandleDeath;

        // Me suscribo al evento que avisa cuando termina la animacion de muerte
        if (enemyAnimator != null)
            enemyAnimator.DeathAnimationFinished += HandleDeathAnimationFinished;
    }

    private void OnDisable()
    {
        //Me desuscribo de los eventos para evitar que se sigan llamando al desactivar el objeto
        health.OnDamaged -= HandleDamaged;
        health.OnDied -= HandleDeath;

        if (enemyAnimator != null)
            enemyAnimator.DeathAnimationFinished -= HandleDeathAnimationFinished;
    }

    //Esta funcion se ejecuta cuando el enemigo recibe dano
    private void HandleDamaged(int damage)
    {
        //Si ya esta muerto no hago nada
        if (isDead) return;

        //Si ya estaba recuperandose de otro golpe, paro esa corrutina, asi evito que se recupere antes de tiempo
        if (hitCoroutine != null)
        {
            StopCoroutine(hitCoroutine);
            hitCoroutine = null;
        }

        //Marco al enemigo como aturdido y detengo su IA
        IsStunned = true;
        StopAI();

        //Reproduzco la animacion de recibir dano
        if (enemyAnimator != null)
            enemyAnimator.PlayHit();

        //Establezco una duracion por defecto para el aturdimiento
        float duration = 0.6f;

        //Si tiene una configuracion, utilizo el tiempo definido en su ScriptableObject
        if (enemyData != null && enemyData.Config != null)
        duration = enemyData.Config.HitStunDuration;

        //Inicio una corrutina para que el enemigo se recupere pasado ese tiempo
        hitCoroutine = StartCoroutine(RecoverAfterHit(duration));
    }

    //Esta corrutina espera a que termine el aturdimiento para reactivar al enemigo
    private IEnumerator RecoverAfterHit(float duration)
    {
        //Espero el tiempo configurado
        yield return new WaitForSeconds(duration);

        hitCoroutine = null;

        //Si ha muerto durante la espera, no vuelvo a activar su IA
        if (isDead)
            yield break;

        //Quito el estado de aturdimiento
        IsStunned = false;

        // Aviso al animator para que pueda volver a reproducir las animaciones normales
        if (enemyAnimator != null)
            enemyAnimator.RecoverFromHit();

        //Permito que el NavMeshAgent vuelva a moverse
        if (navAgent != null && navAgent.enabled && navAgent.isOnNavMesh)
        {
            navAgent.isStopped = false;
        }

        //Reactivo el Behavior Graph para que el enemigo vuelva a tomar decisiones
        if (behaviorAgent != null)
            behaviorAgent.enabled = true;
    }

    //Esta funcion detiene temporalmente la IA y el movimiento del enemigo
    private void StopAI()
    {
        //Desactivo el Behavior Graph para que no siga ejecutando sus comportamientos
        if (behaviorAgent != null)
            behaviorAgent.enabled = false;

        //Detengo el movimiento del NavMeshAgent
        if (navAgent != null && navAgent.enabled && navAgent.isOnNavMesh)
        {
            navAgent.isStopped = true;
            navAgent.velocity = Vector3.zero;
        }
    }

    //Esta funcion se ejecuta cuando la vida del enemigo llega a cero
    private void HandleDeath()
    {
        //Evito ejecutar la muerte varias veces
        if (isDead) return;

        //Marco al enemigo como muerto y quito el aturdimiento
        isDead = true;
        IsStunned = false;

        //Si estaba recuperandose de un golpe, detengo la corrutina
        if (hitCoroutine != null)
        {
            StopCoroutine(hitCoroutine);
            hitCoroutine = null;
        }

        //Detengo definitivamente su IA
        StopAI();

        //Limpio la ruta y desactivo el NavMeshAgent
        if (navAgent != null && navAgent.enabled)
        {
            if (navAgent.isOnNavMesh) navAgent.ResetPath();
            navAgent.enabled = false;
        }

        //Reproduzco la animacion de muerte, si no tiene animator, ejecuto directamente los efectos finales
        if (enemyAnimator != null)
            enemyAnimator.PlayDeath();
        else
            HandleDeathAnimationFinished();
    }

    //Esta funcion se ejecuta cuando termina la animacion de muerte. El aviso llega mediante el evento del EnemyAnimator
    private void HandleDeathAnimationFinished()
    {
        if (!isDead)
            return;


        //Instancio las particulas de la explosion
        if (explosionPrefab != null)
        {
            Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        }

        //Reproduzco el sonido de la explosion en la posicion del enemigo
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySound("EnemyExplosion", transform.position);
        }

        //Genero un numero aleatorio para decidir si suelta el botiquin
        if (healthPickupPrefab != null && Random.value <= dropChance)
        {
            Instantiate(healthPickupPrefab, transform.position, Quaternion.identity);
        }

        //Destruyo al enemigo
        Destroy(gameObject);
    }
}
