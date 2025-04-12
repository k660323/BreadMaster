using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RestPanel : Panel
{
    [SerializeField]
    GameObject nextEventObject;

    Action lamda;

    protected override void Awake()
    {
        base.Awake();

        lamda = () =>
        {
            Camera.main.GetComponent<FollowCamera>().LookAtPostion(nextEventObject.transform.position, 1.0f);
            nextEventObject.SetActive(true);
            callbackAction -= lamda;
        };

        callbackAction -= lamda;
        callbackAction += lamda;
    }
}
