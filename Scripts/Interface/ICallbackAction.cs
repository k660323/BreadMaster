using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICallbackAction
{
    public void RegisterCallbackAction(Action action);
    public void CallbackAction();
    public void RemoveCallbackAction(Action action);
    public void RemoveAllCallbackAction();
}
