using UnityEngine;

public class Health : MonoBehaviour, IDamageable
{

    [SerializeField]
    private int maxHealth = 100;
    private int currentHealth;

    private void Awake(){
        currentHealth = maxHealth;
    }
    public void TakeDamage(int damage){
        damage = Mathf.Abs(damage);
        ChangeHealth(-damage);
    }

    private void ChangeHealth(int modifier)
    {
        currentHealth += modifier;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        if(currentHealth==0){
            Debug.Log("Muerto");
            Destroy(this.gameObject);
        }
        Debug.Log(currentHealth);
    }
}
