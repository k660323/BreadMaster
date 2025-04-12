using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ES_MoveToPanel : EventSequence
{
    [SerializeField]
    Define.PanelType panelType;

    [SerializeField]
    Vector3 arrowPosOffset;

    [SerializeField]
    GameObject Arrow3DPrefab;

    GameObject Arrow3D;

    Action callback;

    public override void StartSequence()
    {
        // 해당 목표에 화살표 오브젝트 배치
        Panel panel = Managers.Game.PanelDic[panelType];
        targetObject = panel.gameObject;
        Arrow3D = Managers.Resource.Instantiate(Arrow3DPrefab.name, null, true);
        Arrow3D.transform.position = targetObject.transform.position + arrowPosOffset;

        // 화살표가 목표를 가리키도록 설정
        Player player = Managers.Game.player;
        player.SetArrowTarget(panel.gameObject);

        ICallbackAction callbackAction = (panel as ICallbackAction);

        // 콜백 이벤트 등록
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
