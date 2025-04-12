using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Define;

public class CustomerController : BaseController
{

    public Vector3 targetPos;

    public State nextState;

    public override State State
    {
        get { return _state; }
        set
        {
            _state = value;
            Gimmick gimmick;
            switch (_state)
            {
                case State.Idle:
                    Owner.anim.SetBool("bMove", false);
                    break;
                case State.Moving:
                    Owner.anim.SetBool("bMove", true);
                    break;
                case State.EnterToMoveCenter:
                    gimmick = Managers.Game.GimmickDic[GimmickType.Center];
                    targetPos = gimmick.transform.position;
                    Owner.anim.SetBool("bMove", true);
                    nextState = Define.State.ToMoveDisplayStand;
                    break;
                case State.ToMoveDisplayStand:
                    IAllocatePlace allocatePlace = Managers.Game.GimmickDic[GimmickType.DisplayStand] as IAllocatePlace;
                    targetPos = allocatePlace.GetPlace();
                    Owner.anim.SetBool("bMove", true);
                    nextState = State.DisplayStandHold;
                    break;
                case State.DisplayStandHold:
                    gimmick = Managers.Game.GimmickDic[GimmickType.DisplayStand];
                    transform.LookAt(gimmick.transform);
                    if (gimmick is IFoodInfo foodInfo)
                        customer.ActiveFoodRequestUI(foodInfo.GetFoodType, true);
                    if (gimmick is IRegisterable registerable1)
                        registerable1.Register(customer);

                    customer.Nav.enabled = false;
                    Owner.anim.SetBool("bMove", false);
                    break;
                case State.ToMoveCenter:
                    customer.Nav.enabled = true;
                    gimmick = Managers.Game.GimmickDic[GimmickType.Center];
                    targetPos = gimmick.transform.position;
                    Owner.anim.SetBool("bMove", true);
                    customer.ActiveCounterUI(true);
                    nextState = State.ToThink;
                    break;
                case State.ToThink:
                    Owner.anim.SetBool("bMove", true);

                    if (Managers.Scene.CurrentScene is GameScene gameScene)
                        gameScene.RequestSpawn();

                    if (Managers.Game.restOpen)
                    {
                        float toCounterPercent = Random.Range(0.0f, 1.0f);
                        // 80% 확률로 카운터
                        if (toCounterPercent <= 0.8f)
                        {
                            State = State.ToCounter;
                        }
                        // 20% 확률은 쉼터
                        else
                        {
                            customer.ActiveResetUI(true);
                            // 휴식 패널 오픈 여부
                            if (Managers.Game.GimmickDic.ContainsKey(GimmickType.RestSpace))
                            {
                                State = State.ToRestSpace;
                            }
                            else
                            {
                                Gimmick counter = Managers.Game.GimmickDic[GimmickType.RestRequest];
                                if (counter is IRegisterable registerable)
                                    registerable.Register(customer);

                                State = State.ToRestRequest;
                            }
                        }
                    }
                    else
                    {
                        State = State.ToCounter;
                    }
                   
                    break;
                case State.ToCounter:
                    Owner.anim.SetBool("bMove", true);
                    gimmick = Managers.Game.GimmickDic[GimmickType.Counter];
                    if (gimmick is IRegisterable registerable2)
                        registerable2.Register(customer);
                    nextState = State.CounterHold;
                    break;
                case State.CounterHold:
                    gimmick = Managers.Game.GimmickDic[GimmickType.Counter];
                    transform.forward = -gimmick.transform.forward;
                    Owner.anim.SetBool("bMove", false);
                    break;
                case State.ToExit:
                    IAllocatePlace allocatePlaceExit = Managers.Game.GimmickDic[GimmickType.Exit] as IAllocatePlace;
                    targetPos = allocatePlaceExit.GetPlace();
                    Owner.anim.SetBool("bMove", true);
                    customer.ActiveEmotionUI(true);
                    nextState = State.Idle;
                    customer.Nav.obstacleAvoidanceType = UnityEngine.AI.ObstacleAvoidanceType.HighQualityObstacleAvoidance;
                    break;
                case State.ToRestRequest:
                    Owner.anim.SetBool("bMove", true);
                    nextState = State.ToRestRequestHold;
                    break;
                case State.ToRestRequestHold:
                    Owner.anim.SetBool("bMove", false);
                    gimmick = Managers.Game.GimmickDic[GimmickType.RestRequest];
                    customer.transform.forward = -gimmick.transform.forward;
                    break;
                case State.ToRestSpace:
                    customer.DisableUI();
                    Owner.anim.SetBool("bMove", true);
                    gimmick = Managers.Game.GimmickDic[GimmickType.RestSpace];
                    if (gimmick is IRegisterable registerable3)
                        registerable3.Register(customer);
                    nextState = State.ToRestSpaceHold;
                    break;
                case State.ToRestSpaceHold:
                    Owner.anim.SetBool("bMove", false);
                    break;
                case State.ToRestSpaceExit:
                    Owner.anim.SetTrigger("tSittingOut");
                    Owner.anim.SetBool("bMove", true);
                    customer.GetRigidBody.isKinematic = false;
                    customer.Nav.enabled = true;
                    customer.ActiveEmotionUI(true);
                    IAllocatePlace allocatePlaceExit2 = Managers.Game.GimmickDic[GimmickType.Exit] as IAllocatePlace;
                    targetPos = allocatePlaceExit2.GetPlace();
                    nextState = State.Idle;
                    customer.Nav.obstacleAvoidanceType = UnityEngine.AI.ObstacleAvoidanceType.HighQualityObstacleAvoidance;
                    break;
            }
        }
    }

    Customer customer;
    public override void Init()
    {
        TryGetComponent(out owner);
        customer = Owner as Customer;
    }

    void OnEnable()
    {
        State = State.EnterToMoveCenter;
    }

    void Update()
    {
        switch (State)
        {
            case State.Idle:
                UpdateIdle();
                break;
            case State.Moving:
                UpdateMove();
                break;
            case State.EnterToMoveCenter:
                UpdateEnterAndMoveCenter();
                break;
            case State.ToMoveDisplayStand:
                UpdateCenterToMoveDisplayStand();
                break;
            case State.DisplayStandHold:
                // 진열대에서 처리...
                break;
            case State.ToMoveCenter:
                UpdateDisplayStandToMoveCenter();
                break;
            case State.ToThink:
               // State에서 바로 바로 처리
                break;
            case State.ToCounter:
                UpdateCenterToMoveCounter();
                break;
            case State.CounterHold:
                // 카운터에서 처리...
                break;
            case State.ToRestRequest:
                UpdateCenterToRestRequest();
                break;
            case State.ToRestRequestHold:
                // 외부에서 처리
                break;
            case State.ToRestSpace:
                UpdateToRestSpace();
                break;
            case State.ToRestSpaceHold:
                // 외부에서 처리
                break;
            case State.ToRestSpaceExit:
                UpdateToRestSpaceExit();
                break;
            case State.ToExit:
                UpdateCounterHoldToExit();
                break;

        }
    }

    void UpdateIdle()
    {

    }

    void UpdateMove()
    {
        MoveTo(targetPos, State.Idle);
    }

    void UpdateEnterAndMoveCenter()
    {
        MoveTo(targetPos, nextState);
    }

    void UpdateCenterToMoveDisplayStand()
    {
        MoveTo(targetPos, nextState);
    }

    void UpdateDisplayStandToMoveCenter()
    {
        MoveTo(targetPos, nextState);
    }

    void UpdateCenterToMoveCounter()
    {
        MoveTo(targetPos, nextState);
    }

    void UpdateCenterToRestRequest()
    {
        MoveTo(targetPos, nextState);
    }

    void UpdateToRestSpace()
    {
        MoveTo(targetPos, nextState);
    }

    void UpdateToRestSpaceExit()
    {
        MoveTo(targetPos, nextState);
    }

    void UpdateCounterHoldToExit()
    {
        MoveTo(targetPos, nextState);
    }

    void MoveTo(Vector3 destPos, State nextState)
    {
        float distance = (destPos - transform.position).magnitude;
        if (distance <= 0.1f)
        {
            transform.position = destPos;
            customer.Nav.isStopped = true;

            State = nextState;
        }
        else
        {
            customer.Nav.SetDestination(destPos);
            customer.Nav.isStopped = false;
        }
    }

    public IEnumerator MoveTo(Vector3 destPos)
    {
        while (true)
        {
            float distance = (destPos - transform.position).magnitude;
            if (distance <= 0.1f)
            {
                transform.position = destPos;
                Owner.anim.SetBool("bMove", false);
                customer.Nav.isStopped = true;
                yield break;
            }
            else
            {
                customer.Nav.SetDestination(destPos);
                Owner.anim.SetBool("bMove", true);
                customer.Nav.isStopped = false;
            }
            yield return null;
        }
    }
}
