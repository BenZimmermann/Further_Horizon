//using System.Collections.Generic;
//using UnityEngine;

//[CreateAssetMenu(menuName = "Quest/QuestData")]
//public class Quest : ScriptableObject
//{
//    public string questName;
//    [TextArea] public string description;

//    // Anforderungen (QuestObject -> benötigte Anzahl)
//    public Dictionary<QuestObject, Requirements> Requirements = new Dictionary<QuestObject, Requirements>();

//    // Fortschritt (QuestObject -> bereits gesammelt/erfüllt)
//    public Dictionary<QuestObject, int> Progress = new Dictionary<QuestObject, int>();

//    /// <summary>
//    /// Aktualisiert den Quest-Fortschritt, wenn mit einem QuestObject interagiert wurde.
//    /// </summary>
//    public bool DoQuestProgress(QuestObject questObj)
//    {
//        if (Requirements == null || !Requirements.ContainsKey(questObj))
//            return false; // Objekt gehört nicht zur Quest

//        var requirement = Requirements[questObj];

//        if (!Progress.ContainsKey(questObj))
//            Progress[questObj] = 0;

//        Progress[questObj]++;

//        Debug.Log($"Quest {questName}: {questObj.name} {Progress[questObj]}/{requirement.Amount}");

//        // true zurückgeben, wenn diese Anforderung erfüllt ist
//        return Progress[questObj] >= requirement.Amount;
//    }

//    /// <summary>
//    /// Prüft, ob ALLE Anforderungen erfüllt sind.
//    /// </summary>
//    public bool IsComplete()
//    {
//        if (Requirements == null || Requirements.Count == 0)
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
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Quest/QuestData")]
public class Quest : ScriptableObject
{
    public string questName;
    [TextArea] public string description;

    [Tooltip("Diese Quest muss abgeschlossen werden, um fortzufahren.")]
    public bool isRequired;

    public Dictionary<QuestObject, Requirements> Requirements = new Dictionary<QuestObject, Requirements>();

    public Dictionary<QuestObject, int> Progress = new Dictionary<QuestObject, int>();

    /// <summary>
    /// Fortschritt für ein bestimmtes QuestObject erhöhen.
    /// </summary>
    public bool DoQuestProgress(QuestObject questObj)
    {
        if (Requirements == null || !Requirements.ContainsKey(questObj))
            return false; 

        var requirement = Requirements[questObj];

        if (!Progress.ContainsKey(questObj))
            Progress[questObj] = 0;

        Progress[questObj]++;

        Debug.Log($"Quest {questName}: {questObj.name} {Progress[questObj]}/{requirement.Amount}");

        return Progress[questObj] >= requirement.Amount;
    }

    /// <summary>
    /// Prüft, ob ALLE Anforderungen erfüllt sind.
    /// </summary>
    public bool IsComplete()
    {
        if (Requirements == null || Requirements.Count == 0)
            return true;

        foreach (var kvp in Requirements)
        {
            var questObj = kvp.Key;
            var requirement = kvp.Value;

            if (!Progress.ContainsKey(questObj) || Progress[questObj] < requirement.Amount)
                return false;
        }

        return true;
    }
}
