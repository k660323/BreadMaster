using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UIElements;

public class DisplayStand : Gimmick, IAllocatePlace, IFoodInfo, IRegisterable
{
    // ���� ���� �ڷ�ƾ
    IEnumerator inputBreadCor;

    // ���� Ÿ��
    [SerializeField]
    Define.FoodType foodType;

    // ���� �����ϴ� ����
    Stack<Food> breads = new Stack<Food>();

    // �ִ� ���� �� �ִ� �� ����
    [SerializeField]
    int maxCount = 10;

    // ���� �ִ� ������
    WaitForSeconds waitInputHold;

    // �� ������ ����
    [SerializeField]
    List<GameObject> socketArr;

    // ����Ʈ�� �������� ���� ��ġ
    List<int> randomSocket = new List<int>();

    // �� ���� index�� ���� randomSocket ��ȸ
    int index = 0;

    // ��� ����
    [SerializeField]
    List<GameObject> holdList;

    // �� ť
    Queue<Customer> customers = new Queue<Customer>();

    // �� �ֱ� ��� �ڷ�ƾ ����
    IEnumerator waitInputFoodCor;

    // ���� �ִ� �ڷ�ƾ ����
    IEnumerator provideFoodCor;

    WaitForSeconds waitFoodPopTime;

    WaitForSeconds waitCustomOutTime;

    bool isPush;
    public Define.FoodType GetFoodType { 
        get 
        {
            return foodType;
        } 
    }

    public override void Init()
    {
        type = Define.GimmickType.DisplayStand;
        Managers.Game.GimmickDic.Add(type, this);
        waitInputHold = new WaitForSeconds(0.1f);
        waitFoodPopTime = new WaitForSeconds(0.1f);
        waitCustomOutTime = new WaitForSeconds(0.5f);

        int i = 0;
        HashSet<int> intHash = new HashSet<int>();
        while (i < holdList.Count)
        {
            int randomVal = Random.Range(0, holdList.Count);
            if(intHash.Contains(randomVal) == false)
            {
                randomSocket.Add(randomVal);
                intHash.Add(randomVal);
                i++;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Player player) == false)
            return;

        callbackAction?.Invoke();

        // ���� ���� ��� �� �������� ���� ���
        if (provideFoodCor != null)
        {
            waitInputFoodCor = WaitInputFoodCor(player);
            StartCoroutine(waitInputFoodCor);
        }
        // �ƹ��� ������ �ٷ� ���� �ִ´�.
        else
        {
            isPush = player.PlayerStat.IsPopAble();
            inputBreadCor = InputBreadCor(player);
            StartCoroutine(inputBreadCor);
        }     
    }

    IEnumerator WaitInputFoodCor(Player player)
    {
        while (provideFoodCor == null)
            yield return null;

        waitInputFoodCor = null;

        inputBreadCor = InputBreadCor(player);
        StartCoroutine(inputBreadCor);
    }



    public void Register(Customer customer)
    {
        customers.Enqueue(customer);

        // 
        if (provideFoodCor == null)
        {
            provideFoodCor = ProvideFoodCor();
            StartCoroutine(provideFoodCor);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        other.TryGetComponent(out Player player);
        if (player == null)
            return;

        if (inputBreadCor != null)
            StopCoroutine(inputBreadCor);
        inputBreadCor = null;

        if (waitInputFoodCor != null)
            StopCoroutine(waitInputFoodCor);
        waitInputFoodCor = null;

        if (provideFoodCor == null && isPush)
        {
            isPush = false;
            provideFoodCor = ProvideFoodCor();
            StartCoroutine(provideFoodCor);
        }
    }

    IEnumerator InputBreadCor(Player player)
    {
        // �÷��̾ ������ �ִ� ���� �ִٸ�
        while (player.PlayerStat.IsPopAble())
        {
            // ���� ������ ���ٸ� ���
            if (breads.Count >= maxCount)
            {
                yield return null;
            }
            // ���� ������ �ִٸ�
            else
            {
                player.SetMaxCanvas(false, Vector3.zero);

                Food food = player.PlayerStat.PopFood();
                // ���Ͽ� �ֱ�
                Transform socektTrans = socketArr[breads.Count].transform;
                food.transform.SetParent(null);
                food.MoveToPosition(socektTrans, Vector3.zero);
                food.transform.rotation = socektTrans.rotation;
                breads.Push(food);
                yield return waitInputHold;
            }
        }

        if (provideFoodCor == null && isPush)
        {
            provideFoodCor = ProvideFoodCor();
            StartCoroutine(provideFoodCor);
        }

        inputBreadCor = null;
    }

    // ���� �ֱ⸶�� ���� ������ ������ ����
    // ���ϴ� ��ŭ ������ ���� ���� ��ȯ
    IEnumerator ProvideFoodCor()
    {
        Queue<Customer> outCustomers = new Queue<Customer>();
        while (customers.Count > 0 && breads.Count > 0)
        {
            Customer customer = customers.Peek();
            if (customer.Controller.State != Define.State.DisplayStandHold)
            {
                yield return null;
                continue;
            }
            CustomerStat cStat = customer.CustomerStat;

            // �� �ְ� ��ǥġ�� ���� ���ߴ�
            while(breads.Count > 0 && cStat.GetRequireFoodValue(foodType) > 0)
            {
                Food food = breads.Pop();
                cStat.SetRequireFoodValue(foodType, cStat.GetRequireFoodValue(foodType) - 1);
                cStat.PushFood(food);
                yield return waitFoodPopTime;
            }

            // ���� �䱸�� ���� ��ä���� ���
            if(cStat.GetRequireFoodValue(foodType) == 0)
            {
                // �ش� �� ����������.
                customers.Dequeue();
                outCustomers.Enqueue(customer);
            }
            // ���� ���� ���
            else
            {
                provideFoodCor = null;
                break;
            }

            yield return null;
        }

        // ���� �� ť
        while(outCustomers.Count > 0)
        {
            yield return waitCustomOutTime;
            Customer outCustomer = outCustomers.Dequeue();
            outCustomer.Controller.State = Define.State.ToMoveCenter;

        }

        provideFoodCor = null;
    }

    public Vector3 GetPlace()
    {
        // GameObject holdSocket = holdList[Random.Range(0, holdList.Count)];
        int pIndex = index;
        index = (index + 1) % holdList.Count;
        GameObject holdSocket = holdList[randomSocket[pIndex]];
        Vector3 pos = holdSocket.transform.position;
        return pos;
    }
}
