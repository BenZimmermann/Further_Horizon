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
using UnityEngine;

public class QuestObject : MonoBehaviour
{
    [SerializeField] private QuestType type = QuestType.Interact;

    private bool isActive = false;

    public void Activate()
    {
        isActive = true;
    }

    public bool Interact(Quest quest)
    {
        if (isActive)
        {
            isActive = false;
            return true;
        }
        return false;
    }
}
