using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Gimmick : MonoBehaviour, ICallbackAction
{
    [SerializeField]
    protected Define.GimmickType type;

    public Define.GimmickType GetGType {  get { return type; } }


    protected Action callbackAction;

    private void Awake()
    {
        Init();
    }

    public abstract void Init();

    public void RegisterCallbackAction(Action action)
    {
        callbackAction -= action;
        callbackAction += action;
    }

    public void CallbackAction()
    {
        callbackAction?.Invoke();
    }

    public void RemoveCallbackAction(Action action)
    {
        callbackAction -= action;
    }

    public void RemoveAllCallbackAction()
    {
        callbackAction = null;
    }
}
