using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using System.IO;

public class texture_checker : MonoBehaviour
{
    public PlayerData item_Data; // ScriptableObject
    public GameObject panel; // Panel nesnesini buraya sürükleyip bırakın
    public string csvFilePath = "Assets/Resources/inventory.csv"; // CSV dosyasının yolu
    public string Block_Path = "Assets/Resources/block/"; // CSV dosyasının yolu
    public GameObject inventoryItemPrefab; // Envanter öğesi için UI prefab'ı
    private List<InventoryItem> inventoryItems = new List<InventoryItem>();
    private List<BlockData> BlockDatas = new List<BlockData>();
    private GameObject selectedItemUI; // Şu anda seçili olan envanter öğesi UI
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public int say = 0;
    void Start()
    {
        resim_yukle();
    }
    // Update is called once per frame
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
    public void DeselectItem(GameObject itemUI)
    {
        // Görsel vurgulamayı kaldır
        Image itemImage = itemUI.GetComponentInChildren<Image>();
        if (itemImage != null)
        {
            itemImage.color = Color.white; // Normal renk
        }
    }
    public void resim_yukle()
    {
        ClearInventoryPanel();
        Sprite[] loadedSprites = Resources.LoadAll<Sprite>("Materials");
        foreach (var sprite in loadedSprites)
        {
            say = 0;
            var targetObject = item_Data.collectedObjects.FirstOrDefault(obj => obj.blockName == sprite.name);
            if (targetObject != null)
            {
                say = targetObject.blockCount;
            }
            if (say > 0)
            {
                //Debug.Log("Yüklenen resim: " + sprite.name + " " + say);
                // Yeni bir envanter öğesi oluştur
                GameObject itemUI = Instantiate(inventoryItemPrefab, panel.transform);

                // UI öğesindeki Image ve TextMeshPro bileşenlerini al
                Image itemImage = itemUI.GetComponentInChildren<Image>(); // Image bileşenini alt öğelerde bul
                TextMeshProUGUI itemCount = itemUI.GetComponentInChildren<TextMeshProUGUI>(); // TextMeshPro bileşenini alt öğelerde bul

                // Resmi ve metni ayarla
                itemImage.sprite = sprite; // Resmi değiştirdik
                itemCount.text = "" + say; // Sayıyı (miktarı) değiştirdik

                itemUI.name = sprite.name;

                // Prefab'ın RectTransform ayarlarını yapın
                RectTransform itemRect = itemUI.GetComponent<RectTransform>();
                itemRect.localScale = Vector3.one; // Ölçek ayarlarını yap
                itemRect.anchoredPosition = Vector2.zero; // Pozisyonu sıfırlayın
                //Debug.Log(" " + sprite.name + " " + say + " ");

                // Tıklanabilirlik ekle
                Button itemButton = itemUI.GetComponent<Button>();
                if (itemButton != null)
                {
                    itemButton.onClick.AddListener(() => OnInventoryItemClick(itemUI));
                }

                inventoryItems.Add(new InventoryItem
                {
                    itemName = sprite.name,
                    itemImage = sprite,
                    itemCount = say,
                });
            }
        }
    }


    public class InventoryItem
    {
        public string itemName; // Öğenin adı
        public Sprite itemImage; // Öğenin görseli
        public int itemCount; // Öğenin miktarı
    }
    public class BlockData
    {
        public string blackName; // Öğenin adı
        public int blockCount; // Öğenin miktarı
        public int blockHealth; // Öğenin miktarı
    }
    public void ClearInventoryPanel()
    {
        // Panel altındaki tüm öğeleri sil
        foreach (Transform child in panel.transform)
        {
            Destroy(child.gameObject);
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

        Debug.Log($"Nesne seçildi: {selectedItemUI.name}");

    }
    public void CreateCubePrefabWithShader(GameObject selectedItemUI)
    {
        Debug.Log($"Küp Yarat: {selectedItemUI.name}");

        // Ana karakterin adını "Player" olarak varsayalım
        GameObject mainCharacter = GameObject.Find("Player");
        if (mainCharacter == null)
        {
            Debug.LogError("Ana karakter (Player) bulunamadı!");
            return;
        }

        Vector3 characterPosition = mainCharacter.transform.position;
        Vector3 characterForward = mainCharacter.transform.forward; // Karakterin yönü

        InventoryItem selectedItem = inventoryItems.FirstOrDefault(item => item.itemName == selectedItemUI.name);
        if (selectedItem == null)
        {
            Debug.LogError("Seçili öğe envanterde bulunamadı!");
            return;
        }

        Debug.Log("Seçilen öğe adı: " + selectedItem.itemName);

        // 1. Küp GameObject oluştur
        GameObject cubePrefab = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cubePrefab.name = selectedItem.itemName;  // Küpe ismini atıyoruz.
        cubePrefab.tag = selectedItem.itemName;

        // 2. Küp'ü karakterin hemen önüne yerleştirme
        float offsetDistance = 2.0f; // Karakterin önünde ne kadar uzaklıkta yerleşecek
        Vector3 spawnPosition = characterPosition + characterForward * offsetDistance; // Pozisyon ayarı
        spawnPosition.x = (int)spawnPosition.x;
        spawnPosition.y = (int)spawnPosition.y;
        spawnPosition.z = (int)spawnPosition.z;
        cubePrefab.transform.position = spawnPosition;

        // 3. Sprite'dan Texture2D'ye dönüştürme
        Texture2D texture = selectedItem.itemImage.texture;

        // 4. Material ve Shader oluşturma
        Material cubeMaterial = new Material(Shader.Find("Unlit/Texture"));

        // 5. Shader'a uygun materyal yükleyelim
        if (texture != null)
        {
            cubeMaterial.mainTexture = texture;
        }

        // 6. Küp'e Material atama
        MeshRenderer renderer = cubePrefab.GetComponent<MeshRenderer>();
        renderer.material = cubeMaterial;

        // Sağlık bileşeni ekleme (opsiyonel)
        var healthComponent = cubePrefab.AddComponent<Health>();

        var targetObject = item_Data.collectedObjects.FirstOrDefault(obj => obj.blockName == selectedItem.itemName);
        if (targetObject != null)
        {
            targetObject.blockCount -= 1;
        }
        resim_yukle();
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

                BlockDatas.Add(new BlockData
                {
                    blackName = values[0],
                    blockCount = int.Parse(values[2]),
                    blockHealth = int.Parse(values[3])
                });
            }
        }
    }
    private void HighlightSelectedItem(GameObject itemUI)
    {
        Image itemImage = itemUI.GetComponentInChildren<Image>();
        if (itemImage != null)
        {
            itemImage.color = Color.yellow; // Seçili nesne için sarı renk
        }
    }
}
