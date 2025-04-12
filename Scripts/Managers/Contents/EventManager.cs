using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    public List<EventSequence> eventSequences;
    private int currentEventSequenceIndex = 0;

    private void Awake()
    {
        foreach (var eventSequence in eventSequences)
        {
            eventSequence.eventManager = this;
        }
    }

    void Start()
    {
        StartEventSequence(currentEventSequenceIndex);
    }

    bool CheckEventSequence(int index)
    {
        if (index < eventSequences.Count)
        {
            return eventSequences[index].ExitCheckCondition();
        }

        return false;
    }

    void StartEventSequence(int index)
    {
        if (index < eventSequences.Count)
        {
            eventSequences[index].StartSequence();
        }
    }

    void StopEventSequence(int index)
    {
        if (index < eventSequences.Count)
        {
            eventSequences[index].StopSequence();
        }
    }


    public void NextEventSequence()
    {
        int nextIndex  = currentEventSequenceIndex + 1;
        if (nextIndex < eventSequences.Count)
        {
            StopEventSequence(currentEventSequenceIndex);
            StartEventSequence(nextIndex);
            currentEventSequenceIndex++;
        }
        else
        {
            // 튜토리얼 완료
        }
    }
}
