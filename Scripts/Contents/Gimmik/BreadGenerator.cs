using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreadGenerator : Gimmick
{
    // 생성할 오브젝트
    [SerializeField]
    Food bread;

    [SerializeField]
    Transform createPos;

    // 생성한 오브젝트를 담는 큐
    Queue<Food> breadQ = new Queue<Food>();

    // 생성 시간
    [SerializeField]
    float createTime = 1.0f;

    // 최대 생성 갯수
    [SerializeField]
    int MaxCreateCount = 10;

    [SerializeField]
    float firePower = 5.0f;

    IEnumerator getBreadCor;
    IEnumerator createbreadCor;
    // 빵 생성


    // 대기 시간
    WaitForSeconds waitCreateTime;
    WaitForSeconds waitHoldTime;
    WaitForSeconds waitBreadPopTime;


    private void Awake()
    {
        Init();
    }

    public override void Init()
    {
        type = Define.GimmickType.BreadGenerator;
        Managers.Game.GimmickDic.Add(type, this);
        waitCreateTime = new WaitForSeconds(createTime);
        waitHoldTime = new WaitForSeconds(0.5f);
        waitBreadPopTime = new WaitForSeconds(0.1f);
    }

    // Start is called before the first frame update
    void Start()
    {
        createbreadCor = CreatebreadCor();
        StartCoroutine(createbreadCor);
    }

    IEnumerator CreatebreadCor()
    {
        while (breadQ.Count < MaxCreateCount)
        {
            // 빵 생성
            GameObject cloneBread = Managers.Resource.Instantiate(bread.gameObject.name, null, true);
            cloneBread.transform.position = createPos.position;
            cloneBread.transform.rotation = Quaternion.identity;
            cloneBread.TryGetComponent(out Bread breadComponent);

            if(breadComponent != null)
            {
                breadComponent.rb.isKinematic = true;
                breadComponent.rb.velocity = Vector3.zero;
                breadComponent.rb.useGravity = false;

                yield return waitHoldTime;

                if (breadComponent.Owner == null)
                {
                    breadQ.Enqueue(breadComponent);
                    breadComponent.rb.isKinematic = false;
                    breadComponent.rb.AddForce(Vector3.back * firePower, ForceMode.Impulse);
                    breadComponent.rb.useGravity = true;
                }
            }
            else
            {
                yield return waitHoldTime;
            }

            yield return waitCreateTime;
        }

        createbreadCor = null;
    }

    private void OnTriggerEnter(Collider other)
    {
        other.gameObject.TryGetComponent(out Player player);
        if (player == null)
            return;

        if (player.PlayerStat.IsPushAble() == false)
            return;

        if (breadQ.Count == 0)
            return;

        if (getBreadCor != null)
            return;

        callbackAction?.Invoke();

        getBreadCor = GetBreadCor(player);
        StartCoroutine(getBreadCor);
    }

    private void OnTriggerExit(Collider other)
    {
        other.gameObject.TryGetComponent(out Player player);
        if (player == null)
            return;

        if (getBreadCor != null)
            StopCoroutine(getBreadCor);
        getBreadCor = null;

        // 빵 생성
        if (breadQ.Count < MaxCreateCount && createbreadCor == null)
        {
            createbreadCor = CreatebreadCor();
            StartCoroutine(createbreadCor);
        }
    }

    // 빵 가져가기
    IEnumerator GetBreadCor(Player player)
    {
        // 플레이어가 들 수 있는 빵 갯수가 한도 초과일 경우 종료
        while (player.PlayerStat.IsPushAble())
        {
            if (breadQ.Count == 0)
            {
                yield return null;
            }
            else
            {
                Food bread = breadQ.Dequeue();
                player.PlayerStat.PushFood(bread);
                yield return waitBreadPopTime;
            }
        }

        // 빵 생성
        if (breadQ.Count < MaxCreateCount && createbreadCor == null)
        {
            createbreadCor = CreatebreadCor();
            StartCoroutine(createbreadCor);
        }



        getBreadCor = null;
    }
}
