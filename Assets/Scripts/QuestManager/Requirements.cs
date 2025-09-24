using UnityEngine;

public enum QuestType
{
    Interact,
    Collect
}

[System.Serializable]
public class Requirements
{
    public QuestType Type;
    public int Amount;
}
