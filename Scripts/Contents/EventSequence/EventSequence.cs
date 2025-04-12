using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EventSequence : MonoBehaviour
{
    public GameObject targetObject;

    [HideInInspector]
    public EventManager eventManager;

    public virtual bool ExitCheckCondition() { return true; }

    public abstract void StartSequence();

    public abstract void StopSequence();
}
