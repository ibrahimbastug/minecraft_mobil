using UnityEditor;
using UnityEngine;
using System.IO;

public class TextureTypeChanger : MonoBehaviour
{
    // Belirli bir klasördeki tüm resimlerin texture type'ını değiştirir
    [MenuItem("Tools/Change Texture Type To 2D And UI")]
    public static void ChangeTextureTypeTo2DAndUI()
    {
        // Klasör yolu
        string folderPath = "Assets/Resources/Materials/block/"; // Kendi klasör yolunuzu buraya yazın
        // Klasördeki tüm dosyaları al
        string[] filePaths = Directory.GetFiles(folderPath, "*.png", SearchOption.AllDirectories);

        // Her bir dosya için texture type'ını değiştir
        foreach (string filePath in filePaths)
        {
            string assetPath = filePath.Substring(filePath.IndexOf("Assets"));
            TextureImporter textureImporter = AssetImporter.GetAtPath(assetPath) as TextureImporter;

            if (textureImporter != null)
            {
                // Texture Type'ı 2D and UI olarak değiştir
                textureImporter.textureType = TextureImporterType.Sprite;
                textureImporter.spriteImportMode = SpriteImportMode.Single;
                textureImporter.SaveAndReimport();

                Debug.Log("Texture type değiştirildi: " + assetPath);
            }
            else
            {
                Debug.LogWarning("Bu dosya bir resim değil veya uygun formatta değil: " + assetPath);
            }
        }
    }
}
