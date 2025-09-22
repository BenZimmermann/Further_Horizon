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
        if (questManager.OnInteract(quest))
        { 
            Activate();
            return true;
        }
        return false;
    }
}
