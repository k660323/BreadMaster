using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Creature
{
    PlayerStat playerStat;
    public PlayerStat PlayerStat { get { return playerStat; } protected set { playerStat = value; } }

    // 인터페이스로 바꿀 예정
    [SerializeField]
    protected CharacterController characterController;
    public CharacterController CharacterController { get { return characterController; } }

    [SerializeField]
    GameObject MaxCanvas;

    Arrow arrow;

    protected override void Init()
    {
        base.Init();
        TryGetComponent(out characterController);
        arrow = GetComponentInChildren<Arrow>();
        ObjectType = Define.ObjectType.Player;
        PlayerStat = stat as PlayerStat;
        Managers.Game.player = this;
    }

    public void SetMaxCanvas(bool isActive, Vector3 pos)
    {
        MaxCanvas.transform.localPosition = pos;
        MaxCanvas.SetActive(isActive);
    }

    public void SetArrowTarget(GameObject target)
    {
        arrow.SetTarget(target);
    }

}
