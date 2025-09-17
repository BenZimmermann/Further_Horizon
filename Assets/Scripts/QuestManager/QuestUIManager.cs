using UnityEngine;
using TMPro;
public class QuestUIManager : MonoBehaviour
{
    [SerializeField] private GameObject questUIPanel;
    [SerializeField] private TMP_Text questTitle;
    [SerializeField] private TMP_Text questDescription;
    [SerializeField] private QuestManager QuestManager;
    private void OnEnable()
    { 
        QuestManager.OnQuestStarted += ShowQuest;
        QuestManager.OnQuestCompleted += ClearQuest;
    }
    private void OnDisable()
    {
        QuestManager.OnQuestStarted -= ShowQuest;
        QuestManager.OnQuestCompleted -= ClearQuest;
    }

    private void ShowQuest(QuestData quest)
    {
        questTitle.text = quest.questName;
        questDescription.text = quest.description;
    }
    private void ClearQuest(QuestData quest)
    {
        questTitle.text = "";
        questDescription.text = "";
    }
}
