// DataPersistenceManager.cs
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Zentrale Steuerung: findet alle IDataPersistence-Objekte in der Szene,
/// lädt zur Laufzeit GameData und verteilt sie; speichert auf Befehl/Exit.
/// </summary>
public class DataPersistenceManager : MonoBehaviour
{
    public static DataPersistenceManager Instance { get; private set; }

    [Header("File Settings")]
    [SerializeField] private string fileName = "savegame.json";
    [SerializeField] private bool useEncryption = false;

    private FileDataHandler dataHandler;
    private GameData gameData;
    private List<IDataPersistence> dataObjects;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(this.gameObject);

        string dir = Application.persistentDataPath;
        dataHandler = new FileDataHandler(dir, fileName, useEncryption);

        // Szenenwechsel-Callbacks
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.sceneUnloaded += OnSceneUnloaded;
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            SceneManager.sceneUnloaded -= OnSceneUnloaded;
        }
    }

    private void Start()
    {
        // Beim ersten Start laden (oder NewGame fallback)
        LoadGame();
    }

    // --- Scene events ---
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Finde alle Objekte, die IDataPersistence implementieren
        dataObjects = FindAllDataPersistenceObjects();

        // Re-dispatch Daten an sie (z. B. wenn man später Szenen wechselt)
        if (gameData != null)
        {
            foreach (var obj in dataObjects)
                obj.LoadData(gameData);
        }
    }

    private void OnSceneUnloaded(Scene scene)
    {
        // Optional: beim Szenenwechsel automatisch speichern
        SaveGame();
    }

    // --- Public API ---
    public void NewGame(GameData initial = null)
    {
        gameData = initial ?? new GameData();
        // Direkt allen mitteilen
        dataObjects = FindAllDataPersistenceObjects();
        foreach (var obj in dataObjects)
            obj.LoadData(gameData);
        SaveGame();  // Gleich zu Beginn abspeichern
    }

    public void LoadGame()
    {
        if (!dataHandler.TryLoad(out var loaded))
        {
            // Kein Save vorhanden → neues Spiel
            Debug.Log("[DPM] Kein Save gefunden → NewGame wird gestartet.");
            NewGame(new GameData());
            return;
        }

        gameData = loaded;
        Debug.Log($"[DPM] Save geladen. LastScene={gameData.selectedPlanetScene}");

        dataObjects = FindAllDataPersistenceObjects();
        foreach (var obj in dataObjects)
            obj.LoadData(gameData);
    }

    public void SaveGame()
    {
        if (gameData == null)
        {
            Debug.LogWarning("[DPM] SaveGame aufgerufen, aber GameData ist null!");
            return;
        }

        // Sammle Daten aus der Szene ein
        dataObjects ??= FindAllDataPersistenceObjects();
        foreach (var obj in dataObjects)
            obj.SaveData(gameData);

        dataHandler.Save(gameData);
        Debug.Log("[DPM] Game doch gespeichert baby.");
    }

    public GameData GetGameData() => gameData;

    // --- Helpers ---
    private List<IDataPersistence> FindAllDataPersistenceObjects()
    {
        // Findet alle aktiven MonoBehaviours in Szene, die IDataPersistence implementieren
        return FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None)
            .OfType<IDataPersistence>()
            .ToList();
    }
}
