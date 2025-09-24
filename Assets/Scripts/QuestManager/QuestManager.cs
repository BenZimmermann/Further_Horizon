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


public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance { get; private set; }
    //public QuestUIManager questUIManager;

    [Header("Quest Liste")]
    [SerializeField] private List<Quest> allQuests = new List<Quest>();

    // Aktive Quests, die noch nicht abgeschlossen sind
    private List<Quest> activeQuests = new List<Quest>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Starte mit allen Quests oder nur bestimmten – hier alle
        activeQuests = allQuests.Where(q => !q.isCompleted).ToList();
    }

    /// <summary>
    /// Wird aufgerufen, wenn der Spieler ein Item einsammelt.
    /// </summary>
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


    /// <summary>
    /// Schließt eine Quest ab und entfernt sie aus der aktiven Liste.
    /// </summary>
    private void CompleteQuest(Quest quest)
    {
        if (quest == null) return;

        quest.isCompleted = true;
        activeQuests.Remove(quest);

        Debug.Log($"[QuestManager] Quest abgeschlossen: {quest.questId}");
        UpdateUI();
    }

    /// <summary>
    /// Gibt dir Zugriff auf den Status einer bestimmten Quest.
    /// </summary>
    public bool IsQuestCompleted(string questId)
    {
        var quest = allQuests.FirstOrDefault(q => q.questId == questId);
        return quest != null && quest.isCompleted;
    }

    /// <summary>
    /// Falls du UI-Updates hast → hier einhängen.
    /// </summary>
    private void UpdateUI()
    {
        Debug.Log("[QuestManager] UI Update triggered.");
        // Platzhalter: später mit deinem QuestUIManager verbinden
        //    if (Current != null)
        //        questUIManager.ShowQuest(Current);
        //    else
        //        questUIManager.ClearQuest();
    }

    /// <summary>
    /// Gibt eine Liste aller aktiven Quests zurück (z. B. für UI).
    /// </summary>
    public List<Quest> GetActiveQuests()
    {
        return activeQuests;
    }

    /// <summary>
    /// Gibt eine Liste aller Quests zurück (z. B. für Save/Load).
    /// </summary>
    public List<Quest> GetAllQuests()
    {
        return allQuests;
    }
}
