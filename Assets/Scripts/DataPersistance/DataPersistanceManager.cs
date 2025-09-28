// DataPersistenceManager.cs
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Zentrale Steuerung: Hierrüber laufen alle Speicher- und Ladeaktionen.
/// Im Interface IDataPersistence implementieren alle Klassen, die Daten speichern/laden wollen.
/// </summary>
public class DataPersistenceManager : MonoBehaviour
{
    public static DataPersistenceManager Instance { get; private set; } // Singleton

    [Header("File Settings")]
    [SerializeField] private string fileName = "savegame.json"; // Dateiname
    [SerializeField] private bool useEncryption = false; // Verschlüsselung an/aus

    private FileDataHandler dataHandler; // Datei-Handler
    private GameData gameData; // Aktuelle Spieldaten
    private List<IDataPersistence> dataObjects; // Alle IDataPersistence-Objekte in der Szene als Liste

    private void Awake() // Singleton 
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(this.gameObject); // Persistenz über Szenenwechsel

        string dir = Application.persistentDataPath; // Plattformunabhängiger Pfad  
        dataHandler = new FileDataHandler(dir, fileName, useEncryption);   // Initialisiere FileDataHandler mit Verschlüsselung

        // Szenenwechsel-Callbacks
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.sceneUnloaded += OnSceneUnloaded;
    }

    private void OnDestroy()
    {
        if (Instance == this) // Nur wenn es die Instanz ist    
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
    #region Szenenwechsel
    // --- Scene events ---
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode) // Nach dem Laden einer Szene
    {
        dataObjects = FindAllDataPersistenceObjects(); // Finde alle Objekte, die IDataPersistence implementieren

        // Lade gespeicherte Daten in diese Objekte
        if (gameData != null)
        {
            foreach (var obj in dataObjects) // Jedes Objekt in der Szene
                obj.LoadData(gameData); // lädt seine Daten aus gameData
        }
    }

    private void OnSceneUnloaded(Scene scene)
    {
        // Beim Verlassen der Szene speichern
        SaveGame();
    }
    #endregion Szenenwechsel

    #region Speicher-Events
    public void NewGame(GameData initial = null) 
    {
        gameData = initial ?? new GameData(); // Neues GameData-Objekt, wenn keines übergeben wurde
        dataObjects = FindAllDataPersistenceObjects(); // Finde alle IDataPersistence-Objekte
        foreach (var obj in dataObjects) // Jedes Objekt in der Szene
            obj.LoadData(gameData); // lädt seine Daten aus gameData
        SaveGame();  // Gleich zu Beginn abspeichern
    }
    public void LoadGame()
    {
        if (!dataHandler.TryLoad(out var loaded)) // Versuche zu laden
        {
            // Kein Save vorhanden → neues Spiel
            Debug.Log("[DPM] Kein Save gefunden → NewGame wird gestartet.");
            NewGame(new GameData()); 
            return;
        }

        gameData = loaded; // Lade die Daten
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
            return; // Zweck: NullReference verhindern und Ursache sichtbar machen
        }

        // Sammle Daten aus der Szene ein
        dataObjects ??= FindAllDataPersistenceObjects();
        foreach (var obj in dataObjects)
            obj.SaveData(gameData);

        dataHandler.Save(gameData);
        Debug.Log("[DPM] Game doch gespeichert baby.");
    }
    #endregion Speicher-Events

    #region Export GameData
    public GameData GetGameData() => gameData; // Externer Zugriff auf GameData
    #endregion Export GameData

    #region IDataPersistence Zugriff
    // Hilfsmethode: findet alle aktiven IDataPersistence-Objekte in der Szene
    private List<IDataPersistence> FindAllDataPersistenceObjects()
    {
        // Findet alle aktiven MonoBehaviours in Szene, die IDataPersistence implementieren
        return FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None)
            .OfType<IDataPersistence>() // Interface filtern
            .ToList(); // als Liste zurückgeben
    }
    #endregion IDataPersistence Zugriff
}
