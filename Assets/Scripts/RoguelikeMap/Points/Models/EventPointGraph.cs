using System;
using System.Linq;
using RoguelikeMap.Points.Models;
using UnityEngine;
using XNode;

[CreateAssetMenu]
public class EventPointGraph : NodeGraph
{
    [field: SerializeField]
    public EventInfo StartNode { get; private set; }
    
    public EventInfo Current { get; private set; }
    public bool IsContainsBattle => CheckContainsBattle();
    public event Action OnEventEnd;

    public void ResetGraph()
    {
        StartNode ??= FindStartNode();
        Current = StartNode;
    }

    private EventInfo FindStartNode()
    {
        return nodes.Select(x => (EventInfo)x).First(x => x.IsStart);
    }
    
    public void NextNode(int buttonIndex)
    {
        Current = Current.MoveNext(buttonIndex);
        
        if (Current.IsEnd || Current.IsBattle)
        {
            OnEventEnd?.Invoke();
            return;
        }
        
        if(Current.IsRandomFork)
            Current = Current.NextRandomNode();
    }

    private bool CheckContainsBattle()
    {
        return false;
        // return StartNode.IsFork 
        //     ? CheckContainsBattle(StartNode.exits)
        //return CheckContainsBattle(StartNode.exits);
    }

    // private bool CheckContainsBattle(IEnumerable<EventInfo> eventInfos)
    // {
    //     return eventInfos.Select(x => x).Any(CheckContainsBattle);
    // }
    //
    // private bool CheckContainsBattle(EventInfo eventInfo)
    // {
    //     if (eventInfo.IsEnd)
    //         return false;
    //     if (eventInfo.IsFork)
    //         return CheckContainsBattle(eventInfo.exits);
    //         
    //     return eventInfo.exits is null ? eventInfo.IsBattle : CheckContainsBattle(eventInfo.exits);
    // }
}