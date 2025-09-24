//using System.Collections.Generic;
//using UnityEngine;

//[CreateAssetMenu(menuName = "Quest/QuestData")]
//public class Quest : ScriptableObject
//{
//    public string questName;
//    [TextArea] public string description;

//    [Tooltip("Diese Quest muss abgeschlossen werden, um fortzufahren.")]
//    public bool isRequired;
//    public int Amount;
//    public Dictionary<QuestObject, Requirements> Requirements = new Dictionary<QuestObject, Requirements>();

//    public Dictionary<QuestObject, int> Progress = new Dictionary<QuestObject, int>();

//    /// <summary>
//    /// Fortschritt für ein bestimmtes QuestObject erhöhen.
//    /// </summary>
//    public bool DoQuestProgress(QuestObject questObj)
//    {
//        if (Requirements == null || !Requirements.ContainsKey(questObj))
//            return false; 

//        var requirement = Requirements[questObj];

//        if (!Progress.ContainsKey(questObj))
//            Progress[questObj] = 0;

//        Progress[questObj]++;

//        Debug.Log($"Quest {questName}: {questObj.name} {Progress[questObj]}/{requirement.Amount}");

//        return Progress[questObj] >= requirement.Amount;
//    }

//    /// <summary>
//    /// Prüft, ob ALLE Anforderungen erfüllt sind.
//    /// </summary>
//    public bool IsComplete()
//    {
//        if (Requirements == null || Requirements.Count == Amount)
//            return true;

//        foreach (var kvp in Requirements)
//        {
//            var questObj = kvp.Key;
//            var requirement = kvp.Value;

//            if (!Progress.ContainsKey(questObj) || Progress[questObj] < requirement.Amount)
//                return false;
//        }

//        return true;
//    }
//}


// Yusuf- Version weil Problem beim Speichern 
// Neuer Versuch um Quests zu erstellen, abfragen und speichern bzw. laden zu können

using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class QuestRequirement
{
    public string itemId;        // Eindeutige ID des Items (z. B. "lumineszenz")
    public int requiredAmount;   // Wie viele Items benötigt werden
    public int currentAmount;    // Fortschritt (wie viele schon gesammelt)
}

[CreateAssetMenu(fileName = "NewQuest", menuName = "Quests/Quest")]
public class Quest : ScriptableObject
{
    [Header("Quest Info")]
    public string questId;        // Eindeutige ID der Quest
    public string questName;      // Lesbarer Name
    [TextArea] public string description;

    [Header("Requirements")]
    public List<QuestRequirement> requirements = new List<QuestRequirement>();

    [Header("Rewards")]
    public string rewardItemId;   // optional: Belohnung als Item
    public int rewardAmount = 1;

    [Header("State")]
    public bool isCompleted = false;

    // --- Methoden ---
    public void AddProgress(string itemId, int amount = 1)
    {
        if (isCompleted) return;

        foreach (var req in requirements)
        {
            if (req.itemId == itemId)
            {
                req.currentAmount = Mathf.Min(req.currentAmount + amount, req.requiredAmount);
            }
        }

        // Prüfen, ob alle Requirements erfüllt sind
        CheckCompletion();
    }

    private void CheckCompletion()
    {
        foreach (var req in requirements)
        {
            if (req.currentAmount < req.requiredAmount)
                return; // Noch nicht fertig
        }

        isCompleted = true;
#if UNITY_EDITOR
        Debug.Log($"[Quest] '{questName}' abgeschlossen!");
#endif
    }

    public float GetProgressNormalized()
    {
        int totalRequired = 0;
        int totalCurrent = 0;

        foreach (var req in requirements)
        {
            totalRequired += req.requiredAmount;
            totalCurrent += Mathf.Min(req.currentAmount, req.requiredAmount);
        }

        if (totalRequired == 0) return 0f;
        return (float)totalCurrent / totalRequired;
    }
}

