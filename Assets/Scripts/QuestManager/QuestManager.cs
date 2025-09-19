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

using UnityEngine;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEditor.PackageManager.Requests;

public class QuestManager : MonoBehaviour
{
    //public static QuestManager Instance { get; private set; }
    [SerializeField] private List<Quest> quests = new List<Quest>();
    public QuestUIManager questUIManager;

    private Quest Current => quests.Count > 0 ? quests[0] : null;

    private int questProgress;

    private void Start()
    {
        questProgress = 0;
        UpdateUI();
    }

    public bool OnInteract(Quest quest)
    {
        if (Current == null) return false;

        if (DoQuestProgress(quest))
        {
            CompleteQuest(quest);
            return true;
        }
        return false;
    }

    private void CompleteQuest(Quest _quest)
    {
        if (quests.Count == 0) return;

        var questIndex = quests.IndexOf(_quest);
        quests.RemoveRange(0, questIndex);
        if(_quest.Amount <= questProgress)
        {
            questProgress = 0;
            quests.RemoveAt(0);
        }
        UpdateUI();
    }

    private bool DoQuestProgress(Quest _quest)
    {
        if (_quest == null) return false;
        foreach (Quest quest in quests)
        {
            
            if (quest == _quest)
            {
                
                questProgress++;
                return true;
            }
            if (quest.isRequired) return false;

        }
        return false;
    }

    private void UpdateUI()
    {
        if (Current != null)
            questUIManager.ShowQuest(Current);
        else
            questUIManager.ClearQuest();
    }
}
