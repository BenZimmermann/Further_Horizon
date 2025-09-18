//using UnityEngine;
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
