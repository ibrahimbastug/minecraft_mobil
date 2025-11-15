using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using System.IO;

public class TagManager : MonoBehaviour
{
    [MenuItem("Tools/Add Tags Automatically")]
    public static void AddTags()
    {
        // Eklenecek tagler
        Sprite[] loadedSprites = Resources.LoadAll<Sprite>("Materials");
        if (loadedSprites == null || loadedSprites.Length == 0)
        {
            Debug.LogWarning("No sprites found in 'Resources/Materials'. Make sure the path is correct.");
            return;
        }

        SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
        SerializedProperty tagsProp = tagManager.FindProperty("tags");

        foreach (var sprite in loadedSprites)
        {
            string tag = sprite.name;
            if (!TagExists(tagsProp, tag))
            {
                tagsProp.InsertArrayElementAtIndex(tagsProp.arraySize);
                tagsProp.GetArrayElementAtIndex(tagsProp.arraySize - 1).stringValue = tag;
                Debug.Log($"Tag eklendi: {tag}");
            }
        }

        // Deðiþiklikleri kaydet
        tagManager.ApplyModifiedProperties();
        Debug.Log("Tüm tagler baþarýyla eklendi.");
    }

    private static bool TagExists(SerializedProperty tagsProp, string tag)
    {
        for (int i = 0; i < tagsProp.arraySize; i++)
        {
            if (tagsProp.GetArrayElementAtIndex(i).stringValue == tag)
            {
                return true;
            }
        }
        return false;
    }
}
