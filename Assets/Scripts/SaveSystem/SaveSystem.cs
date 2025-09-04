using System.IO;
using UnityEngine;

public static class SaveSystem
{
    private const string FileName = "savegame.json";
    private static string FilePath => Path.Combine(Application.persistentDataPath, FileName);

    public static void Save<T>(T data)  //generische Methode, weil verschiedene Daten
    {
        string json = JsonUtility.ToJson(data, true); // Objekt -> JSON
        File.WriteAllText(FilePath, json); // JSON -> Datei
        Debug.Log($"[SaveSystem] Saved to {FilePath}\n{json}");
    }

    public static bool Load<T>(out T data) //generische Methode, weil verschiedene Daten laden
    {
        if (!File.Exists(FilePath))
        {
            data = default; // Wenn nein, leeres Objekt zurück
            Debug.LogWarning($"[SaveSystem] No file found at {FilePath}");
            return false;
        }

        string json = File.ReadAllText(FilePath);   // Datei -> JSON
        data = JsonUtility.FromJson<T>(json);       // JSON -> Objekt
        Debug.Log($"[SaveSystem] Loaded from {FilePath}\n{json}");
        return true;
    }

    public static void DeleteSave()
    {
        if (File.Exists(FilePath))
        {
            File.Delete(FilePath);
            Debug.Log("[SaveSystem] Save deleted.");
        }
    }
}
