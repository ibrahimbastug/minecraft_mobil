using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using System.IO;

public class InventoryUI : MonoBehaviour
{
    public TextMeshProUGUI inventoryText; // UI'deki Text bileşeni
    public PlayerData item_Data; // ScriptableObject
    private Dictionary<string, int> tagCounts = new Dictionary<string, int>();
    public GameObject panel; // Panel nesnesini buraya sürükleyip bırakın
    public string csvFilePath = "Assets/Resources/inventory.csv"; // CSV dosyasının yolu
    public GameObject inventoryItemPrefab; // Envanter öğesi için UI prefab'ı
    private List<InventoryItem> inventoryItems = new List<InventoryItem>();
    private GameObject selectedItemUI; // Şu anda seçili olan envanter öğesi UI

    int say = 0;
    void Start()
    {
        LoadInventoryFromCSV();
        Ekran_Guncelle();

    }
    void Update()
    {
        // M tuşuna basıldığında seçili nesneyi ortala
        if (Input.GetKeyDown(KeyCode.M) && selectedItemUI != null)
        {
            if (selectedItemUI != null)
            {
                Debug.Log("Seçilen öğe adı:1 " + selectedItemUI.name);
                CreateCubePrefabWithShader(selectedItemUI);
            }
            else
            {
                Debug.LogWarning("Henüz bir blok seçilmedi!");
            }
        }
    }
    public void Ekran_Guncelle()
    {
        ClearInventoryPanel();
        //Debug.Log("Toplam Nesne Sayısı"+item_Data.envanter.Count);
        if (inventoryItems != null && item_Data.envanter.Count > 0)
        {
            foreach (InventoryItem item in inventoryItems)
            {
                say = 0;
                var targetObject = item_Data.collectedObjects.FirstOrDefault(obj => obj.blockName == item.itemName);
                if (targetObject != null)
                {
                    say = targetObject.blockCount;
                }
                // Yeni bir envanter öğesi oluştur
                GameObject itemUI = Instantiate(inventoryItemPrefab, panel.transform);

                // UI öğesindeki Image ve TextMeshPro bileşenlerini al
                Image itemImage = itemUI.GetComponentInChildren<Image>(); // Image bileşenini alt öğelerde bul
                TextMeshProUGUI itemCount = itemUI.GetComponentInChildren<TextMeshProUGUI>(); // TextMeshPro bileşenini alt öğelerde bul

                // Resmi ve metni ayarla
                itemImage.sprite = item.itemImage; // Resmi değiştirdik
                itemCount.text = say.ToString(); // Sayıyı (miktarı) değiştirdik

                itemUI.name = item.itemName;

                // Prefab'ın RectTransform ayarlarını yapın
                RectTransform itemRect = itemUI.GetComponent<RectTransform>();
                itemRect.localScale = Vector3.one; // Ölçek ayarlarını yap
                itemRect.anchoredPosition = Vector2.zero; // Pozisyonu sıfırlayın
                Debug.Log(" " + item.itemName + " " + say + " ");

                // Tıklanabilirlik ekle
                Button itemButton = itemUI.GetComponent<Button>();
                if (itemButton != null)
                {
                    itemButton.onClick.AddListener(() => OnInventoryItemClick(itemUI));
                }

            }
        }
        else
        {
            Debug.LogWarning("Envanter öğesi bulunamadı.");
        }
    }
    public void LoadInventoryFromCSV()
    {
        string csvFilePath = Path.Combine(Application.dataPath, "Resources/inventory.csv");
        if (!File.Exists(csvFilePath))
        {
            Debug.LogError("CSV dosyası bulunamadı: " + csvFilePath);
            return;
        }

        using (StreamReader sr = new StreamReader(csvFilePath))
        {
            bool firstLine = true;

            while (!sr.EndOfStream)
            {
                string line = sr.ReadLine();
                if (firstLine)
                {
                    firstLine = false;
                    continue;
                }

                string[] values = line.Split(',');
                if (values.Length < 2 || string.IsNullOrEmpty(values[0]) || string.IsNullOrEmpty(values[1]))
                {
                    Debug.LogWarning("Hatalı satır atlandı: " + line);
                    continue;
                }

                string item_name = values[0];
                string itemImagePath = "Materials/block/" + values[1];  // Uzantısız yol
                Sprite item_image = Resources.Load<Sprite>(itemImagePath);

                if (item_image == null)
                {
                    Debug.LogWarning("Resim yüklenemedi: " + itemImagePath);
                    continue;
                }
                say = 0;
                var targetObject = item_Data.collectedObjects.FirstOrDefault(obj => obj.blockName == item_name);
                if (targetObject != null)
                {
                    say = targetObject.blockCount;
                }


                inventoryItems.Add(new InventoryItem
                {
                    itemName = item_name,
                    itemImage = item_image,
                    itemCount = say,
                    itemHealth = int.Parse(values[3])
                });
            }
        }
    }
    public class InventoryItem
    {
        public string itemName; // Öğenin adı
        public Sprite itemImage; // Öğenin görseli
        public int itemCount; // Öğenin miktarı
        public int itemHealth; // Öğenin miktarı
    }
    public void ClearInventoryPanel()
    {
        // Panel altındaki tüm öğeleri sil
        foreach (Transform child in panel.transform)
        {
            Destroy(child.gameObject);
        }
    }
    public void SelectItem(GameObject itemUI)
    {
        // Önceki seçimi temizle
        if (selectedItemUI != null)
        {
            DeselectItem(selectedItemUI);
        }

        // Yeni öğeyi seç
        selectedItemUI = itemUI;

        // Görsel vurgulama ekle
        Image itemImage = selectedItemUI.GetComponentInChildren<Image>();
        if (itemImage != null)
        {
            itemImage.color = Color.yellow; // Seçili nesne için sarı renk
        }

        Debug.Log($"Nesne seçildi: {selectedItemUI.name}");
    }
    public void DeselectItem(GameObject itemUI)
    {
        // Görsel vurgulamayı kaldır
        Image itemImage = itemUI.GetComponentInChildren<Image>();
        if (itemImage != null)
        {
            itemImage.color = Color.white; // Normal renk
        }
    }
    void OnInventoryItemClick(GameObject itemUI)
    {
        // Önceki seçimi temizle
        if (selectedItemUI != null)
        {
            DeselectItem(selectedItemUI);
        }

        // Yeni öğeyi seç
        selectedItemUI = itemUI;

        // Görsel vurgulama ekle
        Image itemImage = selectedItemUI.GetComponentInChildren<Image>();
        if (itemImage != null)
        {
            itemImage.color = Color.yellow; // Seçili nesne için sarı renk
        }

    }
    public void CreateCubePrefabWithShader(GameObject selectedItemUI)
    {
        // Ana karakterin adını "Player" olarak varsayalım
        GameObject mainCharacter = GameObject.Find("Player");
        Vector3 characterPosition = mainCharacter.transform.position;

        InventoryItem selectedItem = inventoryItems.FirstOrDefault(item => item.itemName == selectedItemUI.name);

        // 1. Küp GameObject oluştur
        GameObject cubePrefab = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cubePrefab.name = selectedItem.itemName;  // Küpe ismini atıyoruz.
        cubePrefab.tag = selectedItem.itemName;

        // 2. Küp'ü sahnede doğru şekilde yerleştirelim
        cubePrefab.transform.position = characterPosition;  // Küp'ün pozisyonunu sıfırla ayarlıyoruz.
        //cubePrefab.transform.localScale = new Vector3(1, 1, 1);  // Boyutunu 1x1x1 olarak ayarlıyoruz.

        // 3. Sprite'dan Texture2D'ye dönüştürme
        Texture2D texture = selectedItem.itemImage.texture;  // Sprite'ın texture'ını alıyoruz

        // 4. Material ve Shader oluşturma
        Material cubeMaterial = new Material(Shader.Find("Unlit/Texture"));  // Unlit shader'ı kullanıyoruz

        // 5. Shader'a uygun materyal yükleyelim
        if (texture != null)
        {
            cubeMaterial.mainTexture = texture;  // Shader'a uygun materyali atıyoruz.
        }

        // 6. Küp'e Material atama
        MeshRenderer renderer = cubePrefab.GetComponent<MeshRenderer>();
        renderer.material = cubeMaterial;  // Küpe materyali atıyoruz.
        var healthComponent = cubePrefab.AddComponent<Health>();

        var targetObject = item_Data.collectedObjects.FirstOrDefault(obj => obj.blockName == selectedItem.itemName);
        if (targetObject != null)
        {
            targetObject.blockCount -= 1;
            Debug.Log(selectedItem.itemName);
            Debug.Log(targetObject.blockCount);
        }
        Ekran_Guncelle();

    }



}
