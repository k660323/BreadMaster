using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CenterGimmick : Gimmick
{
    public override void Init()
    {
        type = Define.GimmickType.Center;
        Managers.Game.GimmickDic.Add(type, this);
    }
}
