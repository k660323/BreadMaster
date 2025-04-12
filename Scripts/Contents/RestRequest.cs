using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class RestRequest : Gimmick, IRegisterable, ICallbackAction
{
    // 줄 간격
    [SerializeField]
    float zInterval = 1.5f;

    // 대기중인 고객 큐
    Queue<Customer> customers = new Queue<Customer>();

    public override void Init()
    {
        type = Define.GimmickType.RestRequest;
        Managers.Game.GimmickDic.Add(type, this);
        callbackAction -= OpenRestSpaceAction;
        callbackAction += OpenRestSpaceAction;
    }

    public Vector3 GetPlace()
    {
        return new Vector3(transform.position.x, transform.position.y, transform.position.z + (customers.Count - 1) * zInterval);

    }
    public void Register(Customer customer)
    {
        customers.Enqueue(customer);
        customer.CustomerController.targetPos = GetPlace();
    }
    

    public void OpenRestSpaceAction()
    {
        IRegisterable registerable = Managers.Game.GimmickDic[GimmickType.RestSpace] as IRegisterable;

        foreach (var c in customers)
        {
            c.Controller.State = Define.State.ToRestSpace;
        }

        customers.Clear();
    }
}
