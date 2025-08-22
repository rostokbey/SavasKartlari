using System.IO;
using UnityEngine;

public static class SaveSystem
{
    static string FilePath(string profileId)
    {
        if (string.IsNullOrEmpty(profileId)) profileId = "default";
        return Path.Combine(Application.persistentDataPath, $"inv_{profileId}.json");
    }

    public static void SaveInventory(string profileId, InventorySaveData data)
    {
        var json = JsonUtility.ToJson(data, true);
        File.WriteAllText(FilePath(profileId), json);
        Debug.Log($"[SaveSystem] Envanter kaydedildi: {FilePath(profileId)}");
    }

    public static InventorySaveData LoadInventory(string profileId)
    {
        var path = FilePath(profileId);
        if (!File.Exists(path)) return new InventorySaveData();

        var json = File.ReadAllText(path);
        return JsonUtility.FromJson<InventorySaveData>(json) ?? new InventorySaveData();
    }
}
