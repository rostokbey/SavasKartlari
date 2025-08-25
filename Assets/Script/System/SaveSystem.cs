// Assets/Script/System/SaveSystem.cs
using System.IO;
using System.Text;
using UnityEngine;

public static class SaveSystem
{
    private const string FilePrefix = "inv_";

    static string PathFor(string profileId)
    {
        return System.IO.Path.Combine(
            Application.persistentDataPath,
            $"{FilePrefix}{profileId}.json"
        );
    }

    public static void SaveInventory(string profileId, InventorySaveData data)
    {
        var json = JsonUtility.ToJson(data);
        File.WriteAllText(PathFor(profileId), json, Encoding.UTF8);
        Debug.Log($"[SaveSystem] Envanter kaydedildi: {PathFor(profileId)}");
    }

    public static InventorySaveData LoadInventory(string profileId)
    {
        var path = PathFor(profileId);
        if (!File.Exists(path))
        {
            Debug.Log($"[SaveSystem] Kayýt yok, yeni oluþturuluyor: {path}");
            return new InventorySaveData();
        }

        var json = File.ReadAllText(path, Encoding.UTF8);
        var data = JsonUtility.FromJson<InventorySaveData>(json);
        return data ?? new InventorySaveData();
    }
}
