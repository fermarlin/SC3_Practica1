
using UnityEngine;

[RequireComponent(typeof(Health))]
public class GeneratorRepair : MonoBehaviour, IInteractable
{
    [SerializeField, Min(0f)]
    private float repairPerSecond = 8f;

    private Health health;
    private float repairAccumulator;

    private void Awake()
    {
        health = GetComponent<Health>();
    }

    public void Interact(float deltaTime)
    {
        if (health.IsDead ||
            health.CurrentHealth >= health.MaxHealth)
        {
            repairAccumulator = 0f;
            return;
        }

        repairAccumulator += repairPerSecond * deltaTime;

        int amount = Mathf.FloorToInt(repairAccumulator);

        if (amount <= 0)
            return;

        health.Heal(amount);
        repairAccumulator -= amount;
    }
}
