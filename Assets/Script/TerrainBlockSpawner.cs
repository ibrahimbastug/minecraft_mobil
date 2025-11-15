using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class BlockData
{
    public GameObject blockPrefab; // Prefab referansı
    public int count;              // Prefab sayısı
    public int initialHealth = 100; // Blokların başlangıç sağlığı
}

public class TerrainBlockSpawner : MonoBehaviour
{
    public int terrainWidth = 50; // X ekseni
    public int terrainDepth = 50; // Z ekseni
    public int maxTerrainHeight = 10; // Maksimum Y ekseni (yükseklik)

    [SerializeField] private List<BlockData> blockDataList = new List<BlockData>();

    private int[,] terrainData;

    void Start()
    {
        GenerateTerrain();
        SpawnBlocksLayered();
    }

    void GenerateTerrain()
    {
        terrainData = new int[terrainWidth, terrainDepth];
        for (int x = 0; x < terrainWidth; x++)
        {
            for (int z = 0; z < terrainDepth; z++)
            {
                // UnityEngine.Random kullanıyoruz
                terrainData[x, z] = UnityEngine.Random.Range(1, maxTerrainHeight + 1);
            }
        }
    }

    void SpawnBlocksLayered()
    {
        foreach (var blockData in blockDataList)
        {
            int blocksPlaced = 0;

            for (int y = 0; y <= maxTerrainHeight && blocksPlaced < blockData.count; y++)
            {
                for (int x = 0; x < terrainWidth && blocksPlaced < blockData.count; x++)
                {
                    for (int z = 0; z < terrainDepth && blocksPlaced < blockData.count; z++)
                    {
                        Vector3 spawnPosition = new Vector3(x, y, z);

                        GameObject newBlock = Instantiate(blockData.blockPrefab, spawnPosition, Quaternion.identity);

                        // Can bileşenini ekle
                        var healthComponent = newBlock.AddComponent<Health>();

                        blocksPlaced++;
                    }
                }
            }
        }
    }
}
