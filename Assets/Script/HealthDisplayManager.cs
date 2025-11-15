using UnityEngine;
using TMPro;
using static System.Net.Mime.MediaTypeNames;
using System.Linq;

public class HealthDisplayManager : MonoBehaviour
{
    public PlayerData item_Data; // ScriptableObject
    public Health currentHealth; // �u anda hedef al�nan nesnenin Health script'i
    public texture_checker envanterUi;
    public TextMeshProUGUI blok_can; // UI �zerindeki sa�l�k metni
    public TextMeshProUGUI blok_ad; // UI �zerindeki sa�l�k metni	
    public TextMeshProUGUI blok_tag; // UI �zerindeki sa�l�k metni
    public TextMeshProUGUI player_health; // Can g�stergesi i�in
    public TextMeshProUGUI player_food; // Yemek g�stergesi i�in
    private Camera mainCamera; // Ana kamera referans�
    public float breakRange = 2f; // Kırma mesafesi
    public LayerMask blockLayer; // Sadece blok katmanını hedef al	
    float basilan_sure = 0f;

    void Start()
    {
        blok_can.text = "";
        blok_ad.text = "";
        blok_tag.text = "";
        player_health.text = $"Health: {item_Data.health}/{item_Data.maxHealth}";
        player_food.text = $"Food: {item_Data.foodLevel}/100";
        mainCamera = Camera.main;

    }

    void Update()
    {
        //player_health.text = $"Health: {item_Data.health}/{item_Data.maxHealth}";
        //player_food.text = $"Food: {item_Data.foodLevel}/100";

        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        //Physics.Raycast(ray, out RaycastHit hit, breakRange, blockLayer))
        if (Physics.Raycast(ray, out hit, breakRange))
        {
            string blok_name = hit.collider.name;
            Health health = hit.collider.GetComponent<Health>();

            if (health != null)
            {
                blok_can.text = $"Blok Can: {health.GetHealth()}";
                blok_ad.text = $"Blok Adı: {blok_name}";
                blok_tag.text = $"Blok Tag: {hit.collider.tag}";
                currentHealth = health;

                // Fare sol tuş basılı
                if (Input.GetMouseButton(0))
                {
                    basilan_sure = basilan_sure + Time.deltaTime;
                    //Debug.Log("aa"+basilan_sure);

                    int sure = Mathf.FloorToInt(basilan_sure);
                    health.TakeDamage(sure);
                    blok_can.text = $"Blok Can: {health.GetHealth()}";
                    blok_ad.text = $"Blok Adı: {blok_name}";
                    blok_tag.text = $"Blok Tag: {hit.collider.tag}";

                    if (health.GetHealth() == 0)
                    {
                        var targetObject = item_Data.collectedObjects.FirstOrDefault(obj => obj.blockName == hit.collider.gameObject.tag);
                        if (targetObject != null)
                        {
                            targetObject.blockCount += 1;
                        }
                        else
                        {
                            item_Data.collectedObjects.Add(
                                new collectedObject
                                {
                                    blockName = hit.collider.gameObject.tag,
                                    blockCount = 1
                                }
                            );
                        }
                        envanterUi.GetComponent<texture_checker>().resim_yukle();
                        //Debug.Log(hit.collider.tag);
                    }

                }
                if (!Input.GetMouseButton(0))
                {
                    basilan_sure = 0f;
                }

            }
            else
            {
                blok_can.text = "";
                blok_ad.text = "";
                blok_tag.text = "";
                currentHealth = null;
            }
        }
        else
        {
            blok_can.text = "";
            blok_ad.text = "";
            blok_tag.text = "";
            currentHealth = null;
        }
    }
}
