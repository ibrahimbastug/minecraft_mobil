using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HighlightOnHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private Image borderImage; // Çerçeve için kullanılan Image bileşeni
    private Color originalColor; // Orijinal renk
    public Color highlightColor = Color.yellow; // Çerçeve rengi

    void Start()
    {
        // Çerçeve için bir GameObject oluştur ve Image bileşeni ekle
        GameObject border = new GameObject("Border");
        border.transform.SetParent(transform, false); // Border'ı mevcut öğeye child olarak ekle
        border.transform.SetAsFirstSibling(); // Border'ı ilk sıraya al (arka planda kalması için)

        // RectTransform ayarlarını yap
        RectTransform borderRect = border.AddComponent<RectTransform>();
        RectTransform currentRect = GetComponent<RectTransform>();
        borderRect.sizeDelta = new Vector2(currentRect.rect.width + 4, currentRect.rect.height + 4); // Çerçeve boyutunu artır
        borderRect.anchorMin = Vector2.zero;
        borderRect.anchorMax = Vector2.one;
        borderRect.offsetMin = new Vector2(-2, -2); // Çerçeve kalınlığı
        borderRect.offsetMax = new Vector2(2, 2);

        // Border için Image bileşeni oluştur
        borderImage = border.AddComponent<Image>();
        borderImage.color = Color.clear; // Varsayılan olarak görünmez yap
        borderImage.raycastTarget = false; // Tıklamaları engellemez
        originalColor = borderImage.color;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // Mouse üzerine geldiğinde çerçeveyi göster
        borderImage.color = highlightColor;
        Debug.Log("fare geldi");
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // Mouse çıktığında çerçeveyi gizle
        borderImage.color = originalColor;
        Debug.Log("fare gitti");
    }
}
