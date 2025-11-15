using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] private int health = 10; // Varsayılan sağlık değeri
    [SerializeField] private float decreaseRate = 1f; // Saniyede ne kadar sağlık azalacak

    public int GetHealth()
    {
        return health;
    }

    public void DecreaseHealthOverTime(float deltaTime)
    {
        // Sağlığı deltaTime ile orantılı olarak azalt
        health -= Mathf.RoundToInt(decreaseRate * deltaTime);
        if (health <= 0)
        {
            health = 0;
            Destroy(gameObject); // Sağlık sıfıra düşerse nesneyi yok et
        }
    }
    public void TakeDamage(int damage)
    {
        health = health - damage;
        if (health < 0) health = 0;

        if (health <= 0)
        {
            Destroy(gameObject); // Can sıfırsa nesneyi yok et
        }
    }

}
