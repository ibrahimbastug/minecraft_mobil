using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class TerrainBlockSpawner2 : MonoBehaviour
{
    public int terrainWidth = 50; // X ekseni
    public int terrainDepth = 50; // Z ekseni
    public int maxTerrainHeight = 10; // Maksimum yükseklik (katmanlar için)

    public string csvFilePath = "Assets/Resources/inventory.csv"; // CSV dosyasının yolu

    private List<BlockData> BlockDatas = new List<BlockData>();
    public List<Sprite> allSprites = new List<Sprite>();

    private HashSet<Vector3> occupiedPositions = new HashSet<Vector3>();

    void Start()
    {
        LoadSprites();
        LoadInventoryFromCSV();
        PlaceBlocksInCircularPattern();
    }

    void LoadSprites()
    {
        Sprite[] loadedSprites = Resources.LoadAll<Sprite>("Materials");
        allSprites.AddRange(loadedSprites);
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
                if (values.Length < 5 || string.IsNullOrEmpty(values[0]) || string.IsNullOrEmpty(values[1]))
                {
                    Debug.LogWarning("Hatalı satır atlandı: " + line);
                    continue;
                }

                string blockName = values[0];
                Sprite matchedSprite = allSprites.FirstOrDefault(sprite => sprite.name == blockName);

                if (matchedSprite == null)
                {
                    Debug.LogWarning($"Blok için sprite bulunamadı: {blockName}");
                    continue;
                }

                BlockDatas.Add(new BlockData
                {
                    blockName = blockName,
                    blockImage = matchedSprite,
                    blockHealth = int.Parse(values[2]),
                    blockCount = int.Parse(values[3]),
                    layer = int.Parse(values[4])
                });
            }
        }
    }

    void PlaceBlocksInCircularPattern()
    {
        BlockDatas = BlockDatas.OrderBy(b => b.layer).ToList(); // Katman sırasına göre sıralama

        foreach (var blockData in BlockDatas)
        {
            int placedCount = 0;
            int radius = 0;

            while (placedCount < blockData.blockCount)
            {
                for (int angle = 0; angle < 360; angle += 15) // 15 derecelik aralıklarla yerleştirme
                {
                    float radians = angle * Mathf.Deg2Rad;
                    int x = Mathf.RoundToInt(Mathf.Cos(radians) * radius);
                    int z = Mathf.RoundToInt(Mathf.Sin(radians) * radius);
                    int y = blockData.layer - 1; // Katmana göre yükseklik ayarı

                    Vector3 position = new Vector3(x, y, z);

                    if (placedCount >= blockData.blockCount)
                        break;

                    if (!occupiedPositions.Contains(position))
                    {
                        CreateBlockPrefab(blockData, position);
                        occupiedPositions.Add(position);
                        placedCount++;
                    }
                }
                radius++; // Daha geniş bir daire için yarıçapı artır
            }
        }
    }

    void CreateBlockPrefab(BlockData blockData, Vector3 position)
    {
        GameObject blockPrefab = GameObject.CreatePrimitive(PrimitiveType.Cube);
        blockPrefab.transform.position = position;
        blockPrefab.name = blockData.blockName;

        Material blockMaterial = new Material(Shader.Find("Unlit/Texture"));
        blockMaterial.mainTexture = blockData.blockImage.texture;
        blockPrefab.GetComponent<Renderer>().material = blockMaterial;
        var healthComponent = blockPrefab.AddComponent<Health>();
        blockPrefab.tag=blockData.blockName;

        Debug.Log($"Blok oluşturuldu: {blockData.blockName} - Pozisyon: {position}");
    }

    public class BlockData
    {
        public string blockName;
        public Sprite blockImage;
        public int blockHealth;
        public int blockCount;
        public int layer; // Katman bilgisi
    }
}
