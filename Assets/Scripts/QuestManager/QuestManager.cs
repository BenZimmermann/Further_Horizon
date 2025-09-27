////die aktuelle quest wird angezeigt
////wenn ein questobjekt enteagiert wird, wird überprüft ob sie requirement hat
////wenn dieser erfüllt wurde wird das objekt aus der liste entfernt
////und alle objekte die davor lagen welche kein requirement hatten auch entfernt
////dann wied die nächste quest angezeigt

using UnityEngine;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEditor.PackageManager.Requests;
using System.Linq;


public class QuestManager : MonoBehaviour, IDataPersistence
{
    public static QuestManager Instance { get; private set; }

    [Header("UI Manager")]
    [SerializeField] private QuestUIManager questUIManager;  // Referenz zum UI Manager

    [Header("Quest Liste")]
    [SerializeField] private List<Quest> allQuests = new List<Quest>(); // Alle verfügbaren Quests im Spiel -> im Inspector setzen

    // Aktive Quests, die noch nicht abgeschlossen sind
    private List<Quest> activeQuests = new List<Quest>(); // Wird in Awake/Start initialisiert

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);        
    }

    private void Start()
    {
        // Initialisiere den Quest UI Manager, falls nicht gesetzt
        if (questUIManager == null)
            questUIManager = GetComponent<QuestUIManager>();
        // Starte mit allen Quests oder nur bestimmten – hier alle
        activeQuests = allQuests.Where(q => !q.isCompleted).ToList();
        UpdateUI();
    }

    /// Wird aufgerufen, wenn der Spieler ein Item einsammelt.
    public void ReportItemCollected(string itemId, int amount)
    {
        foreach (var quest in activeQuests.ToList()) // Kopie, weil Liste sich ändern kann
        {
            if (quest.isCompleted) continue;

            quest.AddProgress(itemId, amount);

            if (quest.isCompleted)
            {
                CompleteQuest(quest);
            }
        }
    }

    #region Persistence     // Speichert den Quest-Status ins GameData-Objekt.

    public void LoadData(GameData data)
    {
        if (data == null || data.completedQuestIds == null) return;

        foreach (var quest in allQuests)
        {
            quest.isCompleted = data.completedQuestIds.Contains(quest.questId);
            if (!quest.isCompleted && !activeQuests.Contains(quest))
                activeQuests.Add(quest);
        }

        UpdateUI();
    }

    public void SaveData(GameData data)
    {
        data.completedQuestIds.Clear();
        foreach (var quest in allQuests)
        {
            if (quest.isCompleted)
                data.completedQuestIds.Add(quest.questId);
        }
    }
  

    #endregion

    #region Quest Prozesse
    // Schließt eine reine Interaktions-Quest ab (z. B. Konsole drücken, Hebel betätigen).
    public bool ProgressQuest(string questId)
    {
        var quest = allQuests.FirstOrDefault(q => q.questId == questId);
        if (quest == null)
        {
            Debug.LogWarning($"[QuestManager] ProgressQuest: Unbekannte questId '{questId}'.");
            return false;
        }

        if (quest.isCompleted)
            return true; // schon erledigt

        // Annahme: Interaktions-Quests haben keine Item-Anforderungen und werden beim
        // Interagieren direkt abgeschlossen. Sammel-Quests laufen über ReportItemCollected.
        CompleteQuest(quest);
        return true;
    }

    /// Schließt eine Quest ab und entfernt sie aus der aktiven Liste.
    private void CompleteQuest(Quest quest)
    {
        if (quest == null) return; // Sicherheitscheck

        quest.isCompleted = true; // Markiere die Quest als abgeschlossen
        activeQuests.Remove(quest); // Entferne die Quest aus den aktiven Quests

        Debug.Log($"[QuestManager] Quest abgeschlossen: {quest.questId}");
        UpdateUI();
    }

    /// Gibt dir Zugriff auf den Status einer bestimmten Quest.
    public bool IsQuestCompleted(string questId)
    {
        var quest = allQuests.FirstOrDefault(q => q.questId == questId); // Suche die Quest
        return quest != null && quest.isCompleted; // Rückgabe des Status 
    }
    #endregion

    #region UI Aktualisierung
    /// Falls du UI-Updates hast → hier einhängen.
    private void UpdateUI()
    {
        Debug.Log("[QuestManager] UpdateUI aufgerufen");

        if (questUIManager == null)
        {
            Debug.LogWarning("[QuestManager] Kein QuestUIManager verknüpft!");
            return;
        }

        var currentQuest = activeQuests.FirstOrDefault();
        if (currentQuest != null)
        {
            Debug.Log($"[QuestManager] Zeige Quest an: {currentQuest.questName}");
            questUIManager.ShowQuest(currentQuest);
        }
        else
        {
            Debug.Log("[QuestManager] Keine aktive Quest gefunden");
            questUIManager.ClearQuest();
        }
    }
    #endregion

    #region Quest Abfrage
    /// Gibt eine Liste aller aktiven Quests zurück (z. B. für UI).
    public List<Quest> GetActiveQuests()
    {
        return activeQuests;
    }

    /// Gibt eine Liste aller Quests zurück (z. B. für Save/Load).
    public List<Quest> GetAllQuests()
    {
        return allQuests;
    }
    #endregion

    #region  Quest Management
    // Methoden für jede einzelne Quest
    //var qm = FindFirstObjectByType<QuestManager>();
    //    if (qm != null && qm.IsActive("HubQuest_02"))
    //    {
    //        // Aktion erlauben (z. B. Mission starten, Szene laden)
    //    }


    #endregion


}
