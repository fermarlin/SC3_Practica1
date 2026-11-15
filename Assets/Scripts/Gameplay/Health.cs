
using System;
using UnityEngine;

//Este script se encarga de gestionar la vida de los objetos del juego, implemento IDamageable para que puedan recibir dano desde otros sistemas
public class Health : MonoBehaviour, IDamageable
{
    [Header("Health")]
    //Vida maxima que puede tener el objeto
    [SerializeField, Min(1)] private int maxHealth = 100;

    //Guardo la vida actual del objeto
    private int currentHealth;

    //Permito que otros scripts consulten la vida actual y la maxima
    public int CurrentHealth => currentHealth;
    public int MaxHealth => maxHealth;

    //Compruebo si el objeto esta muerto cuando su vida llega a cero
    public bool IsDead => currentHealth <= 0;

    //Evento para avisar cuando el objeto recibe dano y sigue vivo, paso la cantidad de dano que ha recibido realmente
    public event Action<int> OnDamaged;

    //Evento para avisar cuando la vida del objeto llega a cero
    public event Action OnDied;

    //Evento para avisar cuando cambia la vida, paso la vida actual y la maxima para que otros scripts puedan utilizarlas
    public event Action<int, int> OnHealthChanged;

    private void Awake()
    {
        //Inicializo la vida actual con el valor maximo
        currentHealth = maxHealth;
    }

    //Esta funcion me permite inicializar la vida desde otros scripts, la utilizo por ejemplo para asignar la vida definida en el EnemyConfigSO
    public void Initialize(int health)
    {
        //Me aseguro de que la vida maxima sea como minimo 1
        maxHealth = Mathf.Max(1, health);

        //Establezco la vida actual al maximo
        currentHealth = maxHealth;

        //Aviso de que la vida ha cambiado
        NotifyHealthChanged();
    }

    // Esta funcion se encarga de aplicar dano al objeto
    public void TakeDamage(int damage)
    {
        //Si el dano es cero, negativo o el objeto ya esta muerto, no hago nada
        if (damage <= 0 || IsDead)
            return;

        //Guard la vida anterior para calcular cuanto dano ha recibido realmente
        int previousHealth = currentHealth;

        //Resto el dano sin permitir que la vida baje de cero
        currentHealth = Mathf.Max(0,currentHealth - damage);

        //Calculo el dano real por si el golpe supera la vida que quedaba
        int actualDamage = previousHealth - currentHealth;

        //Aviso a los demas scripts de que la vida ha cambiado
        NotifyHealthChanged();

        //Compruebo si el objeto ha muerto despues de recibir el dano
        if (IsDead)
        {
            //Lanzo el evento de muerte para que otros scripts reaccionen
            OnDied?.Invoke();
        }
        else
        {
            //Si sigue vivo, aviso de que ha recibido dano
            OnDamaged?.Invoke(actualDamage);
        }
    }

    //Esta funcion permite recuperar vida
    public void Heal(int amount)
    {
        //Si la cantidad no es valida o el objeto esta muerto, no lo curo
        if (amount <= 0 || IsDead)
            return;

        //Guardo la vida anterior para calcular cuanto se ha curado realmente
        int previousHealth = currentHealth;

        //Sumo la curacion sin superar la vida maxima
        currentHealth = Mathf.Min(maxHealth,currentHealth + amount);

        //Calculo la cantidad de vida recuperada realmente
        int actualHealing = currentHealth - previousHealth;

        //Si no ha recuperado vida, no necesito avisar a los demas scripts
        if (actualHealing <= 0)return;

        //Aviso de que la vida ha cambiado para actualizar la UI y otros sistemas
        NotifyHealthChanged();
    }

    //Esta funcion se encarga de avisar cuando cambia la vida del objeto
    private void NotifyHealthChanged()
    {
        //Lanzo el evento pasando la vida actual y la maxima
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }
}
