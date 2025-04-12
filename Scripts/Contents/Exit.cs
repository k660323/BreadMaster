using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Exit : Gimmick, IAllocatePlace
{
    public Vector3 GetPlace()
    {
        return gameObject.transform.position;
    }

    public override void Init()
    {
        type = Define.GimmickType.Exit;
        Managers.Game.GimmickDic.Add(type, this);
    }
}
