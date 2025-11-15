using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerData", menuName = "Game/Player Data")]
public class PlayerData : ScriptableObject
{
    [Header("Basic Info")]
    public string playerName = "Player";
    public int level = 1;
    public float health = 100f;
    public float maxHealth = 100f;
    public float foodLevel = 100f;

    [Header("Position")]
    public Vector3 position;

    // Kullanılabilir blokların prefab'leri
    public List<GameObject> blockPrefabs;

    // Toplanan nesneler
    public List<collectedObject> collectedObjects = new List<collectedObject>();

    public List<string> envanter; // Envanterdeki blok isimleri

    [Header("Statistics")]
    public int blocksPlaced;
    public int blocksBroken;

    // Can azaltan bir fonksiyon
    public void TakeDamage(float damage)
    {
        health -= damage;
        if (health < 0) health = 0;
    }

    // Can artıran bir fonksiyon
    public void Heal(float amount)
    {
        health += amount;
        if (health > maxHealth) health = maxHealth;
    }
}

// Toplanan nesne sınıfı
[System.Serializable]
public class collectedObject
{
    public string blockName;
    public int blockCount;
}
