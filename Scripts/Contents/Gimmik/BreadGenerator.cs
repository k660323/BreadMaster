using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreadGenerator : Gimmick
{
    // ������ ������Ʈ
    [SerializeField]
    Food bread;

    [SerializeField]
    Transform createPos;

    // ������ ������Ʈ�� ��� ť
    Queue<Food> breadQ = new Queue<Food>();

    // ���� �ð�
    [SerializeField]
    float createTime = 1.0f;

    // �ִ� ���� ����
    [SerializeField]
    int MaxCreateCount = 10;

    [SerializeField]
    float firePower = 5.0f;

    IEnumerator getBreadCor;
    IEnumerator createbreadCor;
    // �� ����


    // ��� �ð�
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
            // �� ����
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

        // �� ����
        if (breadQ.Count < MaxCreateCount && createbreadCor == null)
        {
            createbreadCor = CreatebreadCor();
            StartCoroutine(createbreadCor);
        }
    }

    // �� ��������
    IEnumerator GetBreadCor(Player player)
    {
        // �÷��̾ �� �� �ִ� �� ������ �ѵ� �ʰ��� ��� ����
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

        // �� ����
        if (breadQ.Count < MaxCreateCount && createbreadCor == null)
        {
            createbreadCor = CreatebreadCor();
            StartCoroutine(createbreadCor);
        }



        getBreadCor = null;
    }
}
