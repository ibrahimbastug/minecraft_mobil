using UnityEngine;

public class BlockHealth : MonoBehaviour
{
    public int health=10;

    public void Initialize(int initialHealth)
    {
        health = initialHealth;
    }

    public int GetHealth()
    {
        return health;
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        if (health < 0) health = 0;

        if (health <= 0)
        {
            Destroy(gameObject); // Can sýfýrsa nesneyi yok et
        }
    }
}
