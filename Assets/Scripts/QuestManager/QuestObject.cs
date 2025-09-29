using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class QuestObject : MonoBehaviour
{
    //[SerializeField] private QuestType type = QuestType.Interact;
    //[SerializeField] public MonoBehaviour InteractableScript;
    QuestManager questManager;
    [SerializeField] private Quest quest;
    // private Interactable interactable;
    private bool isActive = false;

    public void Awake()
    {
        questManager = FindFirstObjectByType<QuestManager>();
    }
    public void Activate()
    {
        isActive = true;
    }

    public bool IsInteracted()
    {
        Debug.Log("Interacted with QuestObject" + quest);
        if (questManager.ProgressQuest(quest.questId))
        {
            Activate();
            return true;
        }
        return false;
    }
}
