using UnityEngine;
using System;
[CreateAssetMenu(menuName = "Quest/Quest Event Channel")]
public class QuestEventChannel : ScriptableObject
{
    public Action<string> OnEventRaised;
    public void Raise(string eventName)
    {
        OnEventRaised?.Invoke(eventName);
    }
}
