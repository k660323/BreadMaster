using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ES_MoveToGimmik : EventSequence
{
    [SerializeField]
    Define.GimmickType gimmickType;

    [SerializeField]
    Vector3 arrowPosOffset;

    [SerializeField]
    GameObject Arrow3DPrefab;

    GameObject Arrow3D;

    Action callback;

    public override void StartSequence()
    {
        // �ش� ��ǥ�� ȭ��ǥ ������Ʈ ��ġ
        Gimmick gimmick = Managers.Game.GimmickDic[gimmickType];
        targetObject = gimmick.gameObject;
        Arrow3D = Managers.Resource.Instantiate(Arrow3DPrefab.name, null, true);
        Arrow3D.transform.position = targetObject.transform.position + arrowPosOffset;

        // ȭ��ǥ�� ��ǥ�� ����Ű���� ����
        Player player = Managers.Game.player;
        player.SetArrowTarget(gimmick.gameObject);

        ICallbackAction callbackAction = gimmick as ICallbackAction;

        // �ݹ� �̺�Ʈ ���
        callback = () => {
            if (ExitCheckCondition() == false)
                return;

            callbackAction.RemoveCallbackAction(callback);
            player.SetArrowTarget(null);
            Managers.Resource.Destroy(Arrow3D);
            eventManager.NextEventSequence(); 
        };

        callbackAction.RegisterCallbackAction(callback);
    }

    public override void StopSequence()
    {
        
    }
}
