// FileDataHandler.cs
using System.IO;
using UnityEngine;

/// <summary>
/// Verantwortlich für: Pfad, Datei lesen/schreiben, (optional) simple Verschlüsselung.
/// </summary>
public class FileDataHandler
{
    private readonly string dirPath; 
    private readonly string fileName;
    private readonly bool useEncryption;

    private const string encryptionKeyword = "xor-key"; // simple XOR (optional)

    public FileDataHandler(string dirPath, string fileName, bool useEncryption = false)
    {
        this.dirPath = dirPath;
        this.fileName = fileName;
        this.useEncryption = useEncryption;
    }

    public string GetFullPath() => Path.Combine(dirPath, fileName);

    public bool TryLoad(out GameData data)
    {
        string fullPath = GetFullPath();
        if (!File.Exists(fullPath))
        {
            data = null;
            return false;
        }

        try
        {
            string json = File.ReadAllText(fullPath);
            if (useEncryption) json = XOR(json, encryptionKeyword);
            data = JsonUtility.FromJson<GameData>(json);
            return data != null;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[FileDataHandler] Load failed: {e}");
            data = null;
            return false;
        }
    }
    
    public void Save(GameData data)
    {
        try
        {
            if (!Directory.Exists(dirPath))
                Directory.CreateDirectory(dirPath);

            string json = JsonUtility.ToJson(data, true);
            if (useEncryption) json = XOR(json, encryptionKeyword);

            string fullPath = GetFullPath();
            File.WriteAllText(fullPath, json);
#if UNITY_EDITOR
            Debug.Log($"[FileDataHandler] Saved: {fullPath}");
#endif
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[FileDataHandler] Save failed: {e}");
        }
    }

    // simple XOR "obfuscation" (optional)
    private static string XOR(string text, string key)
    {
        if (string.IsNullOrEmpty(text) || string.IsNullOrEmpty(key)) return text;
        var result = new char[text.Length];
        for (int i = 0; i < text.Length; i++)
            result[i] = (char)(text[i] ^ key[i % key.Length]);
        return new string(result);
    }
}
