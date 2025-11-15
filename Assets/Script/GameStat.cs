using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static System.Net.Mime.MediaTypeNames;

public class GameStat : MonoBehaviour
{
    public PlayerData playerData; // PlayerData'ya referans

    [Header("UI Elements")]
    public TextMeshProUGUI healthText; // Can göstergesi için
    public TextMeshProUGUI foodText; // Yemek göstergesi için

    void Update()
    {
        // Can ve yemek bilgilerini UI'ye yazdýr
        healthText.text = $"Health: {playerData.health}/{playerData.maxHealth}";
        foodText.text = $"Food: {playerData.foodLevel}/100";
    }
}
