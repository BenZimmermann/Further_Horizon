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

        // Titel and Description
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