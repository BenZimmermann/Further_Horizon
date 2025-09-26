using System;
using System.Collections.Generic;

[Serializable]
public class GameData
{
    // --- Planetenauswahl ---
    public string selectedPlanetScene;                 // aktuell ausgewählter Planet
    public List<string> unlockedPlanets = new();       // freigeschaltete Planeten

    // --- Inventar (flach: itemId + Anzahl) ---
    public List<ItemSaveData> items = new();           // wird vom InventoryManager befüllt/ausgelesen

    // --- Checkpoints: JsonUtility-safe (kein Dictionary!) ---
    public List<SceneSpawnRecord> spawnRecords = new(); // (sceneName, spawnId) Paare

    // >>> NEU: Inventar als speicherbare Liste
    public List<ItemSaveData> inventory = new List<ItemSaveData>();
    
    //// NEU: letzter Spawnpunkt je Szene (sceneName -> spawnId)
    //public Dictionary<string, string> lastSpawnByScene = new Dictionary<string, string>();

    // Vorbereitung zum Initialisieren der Module Items im Schiff später
    // Die listen müssen ausgelesen werden, damit ma die Items im Schiff später platzieren kann
    public List<string> pendingInstallModuleIds = new List<string>(); // ItemIds werden hier hinterlegt vom jeweiligen Level -> kann man später auslese zum Modul abgeben
    public List<string> installedModuleIds = new List<string>(); // schon platzierte Module -> zum level freischalten später
    public string lastReturnFromScene; // von wo bin ich gekommen -> falls im debug nötig 
    // Ende vorbereitung

    // Für Module Funktion, damits mit dem QuestManager passt
    public List<string> completedQuestIds = new List<string>();   // abgeschlossene Quests (IDs oder Namen)


    public GameData()
    {
        // Default Planet ist unser Startplanet Cryovista
        unlockedPlanets = new List<string> { "Cryovista" };
        selectedPlanetScene = "Cryovista";
    }

    // ---------- Convenience-Helpers ----------

    // Schreib Zugriff: setze/überschreibe SpawnId für eine Szene

    public void SetSpawnForScene(string sceneName, string spawnId)
    {
        int idx = spawnRecords.FindIndex(r => r.sceneName == sceneName);
        if (idx >= 0) spawnRecords[idx] = new SceneSpawnRecord(sceneName, spawnId);
        else spawnRecords.Add(new SceneSpawnRecord(sceneName, spawnId));
    }

    // liest den gespeicherten Spawn (oder null, falls keiner existiert)
    public string GetSpawnForScene(string sceneName)
    {
        int idx = spawnRecords.FindIndex(r => r.sceneName == sceneName);
        return idx >= 0 ? spawnRecords[idx].spawnId : null;
    }
}

[Serializable]
public struct SceneSpawnRecord
{
    public string sceneName;
    public string spawnId;

    public SceneSpawnRecord(string sceneName, string spawnId)
    {
        this.sceneName = sceneName;
        this.spawnId = spawnId;
    }
}
