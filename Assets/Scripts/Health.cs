
using System;
using UnityEngine;

public class Health : MonoBehaviour, IDamageable
{
    [Header("Health")]
    [SerializeField, Min(1)] private int maxHealth = 100;
    [SerializeField] private bool destroyOnDeath = true;

    private int currentHealth;

    public int CurrentHealth => currentHealth;
    public int MaxHealth => maxHealth;
    public bool IsDead => currentHealth <= 0;

    public event Action<int> OnDamaged;
    public event Action OnDied;

    public event Action<int, int> OnHealthChanged;

    private void Awake()
    {
        currentHealth = maxHealth;
    }

    public void Initialize(int health)
    {
        maxHealth = Mathf.Max(1, health);
        currentHealth = maxHealth;

        NotifyHealthChanged();
    }

    public void TakeDamage(int damage)
    {
        if (damage <= 0 || IsDead)
            return;

        int previousHealth = currentHealth;

        currentHealth = Mathf.Max(
            0,
            currentHealth - damage
        );

        int actualDamage = previousHealth - currentHealth;

        NotifyHealthChanged();

        if (IsDead)
        {
            OnDied?.Invoke();

            if (destroyOnDeath)
                Destroy(gameObject);
        }
        else
        {
            OnDamaged?.Invoke(actualDamage);
        }
    }

    public void Heal(int amount)
    {
        if (amount <= 0 || IsDead)
            return;

        int previousHealth = currentHealth;

        currentHealth = Mathf.Min(
            maxHealth,
            currentHealth + amount
        );

        int actualHealing = currentHealth - previousHealth;

        if (actualHealing <= 0)
            return;

        NotifyHealthChanged();
    }

    private void NotifyHealthChanged()
    {
        OnHealthChanged?.Invoke(
            currentHealth,
            maxHealth
        );
    }
}
