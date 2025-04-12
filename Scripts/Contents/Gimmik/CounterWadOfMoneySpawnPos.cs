using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class CounterWadOfMoneySpawnPos : Gimmick
{
    [SerializeField]
    GameObject moneyGroupPrefab;

    WadOfMoney wadOfMoney;

    int cumulativeMoney;

    Action<int> moneyAction;

    public override void Init()
    {
        type = Define.GimmickType.CounterWadOfMoneySP;
        Managers.Game.GimmickDic.Add(type, this);

        moneyAction -= CameraEvent;
        moneyAction += CameraEvent;
    }

   public void SpawnWadOfMoney()
    {
        // µ· »ý¼º
        int money = Random.Range(10, 14);
        if (wadOfMoney == null)
        {
            GameObject moneyGroup = Managers.Resource.Instantiate(moneyGroupPrefab.name, null, true);
            moneyGroup.transform.position = transform.position;
            moneyGroup.TryGetComponent(out wadOfMoney);
            wadOfMoney.SetMoney(money);
            wadOfMoney.destoryAction += () => { wadOfMoney = null; callbackAction?.Invoke(); };
            
        }
        else
        {
            wadOfMoney.SetMoney(wadOfMoney.Money + money);
        }

        moneyAction?.Invoke(money);
    }

    public void CameraEvent(int value)
    {
        cumulativeMoney += value;
        if (cumulativeMoney >= 30)
        {
            Managers.Game.restOpen = true;
            Panel panel = Managers.Game.PanelDic[Define.PanelType.RestPanel];
            Camera.main.GetComponent<FollowCamera>().LookAtPostion(panel.transform.position, 1.0f);
            moneyAction -= CameraEvent;
        }
    }
}
