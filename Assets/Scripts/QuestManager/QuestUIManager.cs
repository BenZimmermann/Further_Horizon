using UnityEngine;
using TMPro;

public class QuestUIManager : MonoBehaviour
{
    [SerializeField] private GameObject questUIPanel;
    [SerializeField] private TMP_Text questTitle;
    [SerializeField] private TMP_Text questDescription;

    public void ShowQuest(Quest questData)
    {
        if (questData == null) return;
        questUIPanel.SetActive(true);
        questTitle.text = questData.questName;
        questDescription.text = questData.description;
    }

    public void ClearQuest()
    {
        questUIPanel.SetActive(false);
        questTitle.text = "";
        questDescription.text = "";
    }
}


//füge alle scripte zusammen, sodass es ein questsystem ergibt. das questobjekt ligt auf dem objekt welches am ende interagiert... dieses "hört" ob sich etwas tut wenn eine interaction stattfindet soll der bool auf true gesetzt werden und geht in den progress manager. dieser fragt im quest scriptable objekt ob und wie viel progress gemacht wurde. dieser progress wird auf jedem quest scriptable objekt festgelegt -> diehnt zb für die quest sammle 5 holz. außerdem bekommt quest aus der requirement klass enums (um auszuwählen um was für eine art von quest es sich handelt und den amount der eingesammelt wurde/werden kann) außerdem besitzt quest einen bool namens isrequired. dieser kann im inspector angecklickt werden um festzulegen ob die quest gemacht werden muss um weiter zu gehen oder ob sie übersprungen werden kann. zb haben wir quest 1,2,3,4 quest 1 und 2 sind nicht required heist sie können in beliebiger reihenfolge gemacht werden, werden aber trotzdem in der liste nach einander angezeigt. quest 3 ist required heist sie kann nicht übersprungen werden wenn quest 4 gemacht wurde. wenn aber quest 3 ausversehen vom spieler gemacht wurde und 1 und oder 2 noch aktiv sind werden sie aus der liste gelöscht. all das soll dann einheitlich im questUIManager angezeigt werden

//alle scripte sollten möglichst nur über parameter übergabe laufen. dies sind meine bespielscripte
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
//using UnityEngine;
//using UnityEngine.UI;

//public class QuestObject : MonoBehaviour
//{
//    Quest quest;

//    Requirements requirements;

//    [SerializeField] private QuestType Type = QuestType.Interact;

//    bool isActive = false;
//    public void Activate()
//    {
//        isActive = true;
//    }

//    public bool Interact(Quest _quest)
//    {
//        if (isActive)
//        {
//            isActive = false;
//            return true;
//        }
//        return false;
//    }
//}
// using UnityEngine;
//using System;

//enum QuestType
//{
//    Interact,
//    Collect
//}

//public class Requirements
//{
//    public int Amount { get; set; }
//    QuestType Type { get; set; }
//}
//using UnityEngine;
//using System.Collections.Generic;

//public class QuestManager : MonoBehaviour
//{
//    [SerializeField] private List<Quest> quests = new List<Quest>();
//    public QuestUIManager questUIManager;

//    private Quest Current => quests.Count > 0 ? quests[0] : null;

//    private void Start()
//    {
//        UpdateUI();
//    }
//    private void Update()
//    {
//        Debug.Log(quests.Count);
//    }

//    private void UpdateUI()
//    {
//        if (Current != null)
//            questUIManager.ShowQuest(Current);
//        else
//            questUIManager.ClearQuest();
//    }

//    public void OnInteract(GameObject target)//das als bool?
//    {
//        if (Current == null) return;

//        if (DoQuestProgress(Current, target))
//        {
//            CompleteQuest(Current);
//        }
//    }

//    private void CompleteQuest(Quest questData)
//    {
//        if (quests.Count == 0) return;
//        quests.RemoveAt(0);

//        while (quests.Count > 0 && (quests[0].Requirements == null || quests[0].Requirements.Count == 0))
//        {
//            quests.RemoveAt(0);
//        }

//        UpdateUI();
//    }

//    private bool DoQuestProgress(Quest questData, GameObject target)
//    {
//        if (questData == null || questData.Requirements == null) return true;

//        // Versuch, ob das Ziel ein QuestObject hat
//        var questObj = target.GetComponent<QuestObject>();
//        if (questObj == null) return false;

//        // Fortschritt machen
//        questData.DoQuestProgress(questObj);

//        // Wenn die Quest komplett erfüllt ist, true zurückgeben
//        return questData.IsComplete();
//    }
//}



////quests werden in die liste gelegt

////die aktuelle quest wird angezeigt
////wenn ein questobjekt enteagiert wird, wird überprüft ob sie requirement hat
////wenn dieser erfüllt wurde wird das objekt aus der liste entfernt
////und alle objekte die davor lagen welche kein requirement hatten auch entfernt
////dann wied die nächste quest angezeigt
//using UnityEngine;
//using TMPro;
//public class QuestUIManager : MonoBehaviour
//{
//    [SerializeField] private GameObject questUIPanel;
//    [SerializeField] private TMP_Text questTitle;
//    [SerializeField] private TMP_Text questDescription;
//    private void DoStuff() { }
//    public void ShowQuest(Quest questData)
//    {
//        if (questData == null) return;
//        questTitle.text = questData.questName;
//        questDescription.text = questData.description;
//    }
//    public void ClearQuest()
//    {
//        questTitle.text = "";
//        questDescription.text = "";
//    }
//}

