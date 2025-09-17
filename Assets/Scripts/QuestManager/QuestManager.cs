using UnityEngine;
using System.Collections.Generic;
public class QuestManager : MonoBehaviour
{
    [SerializeField] private List<QuestData> quests;
    [SerializeField] private QuestEventChannel _questEventChannel;

    private QuestData currentQuest;

    public delegate void QuestEvent(QuestData quest);
    public event QuestEvent OnQuestStarted;
    public event QuestEvent OnQuestCompleted;

    private void OnEnable()
    {
        _questEventChannel.OnEventRaised += HandleQuestEvent;
        StartNextQuest();
    }
    private void OnDisable()
    {
        _questEventChannel.OnEventRaised -= HandleQuestEvent;
    }
    private void HandleQuestEvent(string eventName) 
    {
        if(currentQuest != null && currentQuest.requiredEvents == eventName)
        {
            OnQuestCompleted?.Invoke(currentQuest);
            quests.Remove(currentQuest);
            StartNextQuest();
        }
    }
    private void StartNextQuest()
    {
        if (quests.Count > 0)
        {
            currentQuest = quests[0];
            OnQuestStarted?.Invoke(currentQuest);
        }
        else
        {
            currentQuest = null;
        }
    }
}
// die quest data bestimmt den inhalt der quests
// die quest event channel ist für die events zuständig, die quests starten und beenden
// der quest manager verwaltet die quests und deren zustände
// der quest ui manager zeigt die quests im ui an

// wie alles verbinden?
//-> mit unity events?

