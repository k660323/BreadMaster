using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UIElements;

public class DisplayStand : Gimmick, IAllocatePlace, IFoodInfo, IRegisterable
{
    // 빵을 놓는 코루틴
    IEnumerator inputBreadCor;

    // 음식 타입
    [SerializeField]
    Define.FoodType foodType;

    // 빵을 관리하는 스택
    Stack<Food> breads = new Stack<Food>();

    // 최대 담을 수 있는 빵 갯수
    [SerializeField]
    int maxCount = 10;

    // 빵을 넣는 딜레이
    WaitForSeconds waitInputHold;

    // 빵 진열대 소켓
    [SerializeField]
    List<GameObject> socketArr;

    // 리스트에 무작위로 소켓 배치
    List<int> randomSocket = new List<int>();

    // 각 소켓 index로 인해 randomSocket 순회
    int index = 0;

    // 대기 소켓
    [SerializeField]
    List<GameObject> holdList;

    // 고객 큐
    Queue<Customer> customers = new Queue<Customer>();

    // 빵 넣기 대기 코루틴 실행
    IEnumerator waitInputFoodCor;

    // 빵을 주는 코루틴 실행
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

        // 제공 중일 경우 다 가져갈때 까지 대기
        if (provideFoodCor != null)
        {
            waitInputFoodCor = WaitInputFoodCor(player);
            StartCoroutine(waitInputFoodCor);
        }
        // 아무도 없으면 바로 빵을 넣는다.
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
        // 플레이어가 가지고 있는 빵이 있다면
        while (player.PlayerStat.IsPopAble())
        {
            // 담을 공간이 없다면 대기
            if (breads.Count >= maxCount)
            {
                yield return null;
            }
            // 담을 공간이 있다면
            else
            {
                player.SetMaxCanvas(false, Vector3.zero);

                Food food = player.PlayerStat.PopFood();
                // 소켓에 넣기
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

    // 일정 주기마다 빵이 있으면 고객에게 제공
    // 원하는 만큼 제공된 고객은 상태 전환
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

            // 빵 있고 목표치에 도달 못했다
            while(breads.Count > 0 && cStat.GetRequireFoodValue(foodType) > 0)
            {
                Food food = breads.Pop();
                cStat.SetRequireFoodValue(foodType, cStat.GetRequireFoodValue(foodType) - 1);
                cStat.PushFood(food);
                yield return waitFoodPopTime;
            }

            // 고객의 요구한 빵을 다채웠을 경우
            if(cStat.GetRequireFoodValue(foodType) == 0)
            {
                // 해당 고객 돌려보낸다.
                customers.Dequeue();
                outCustomers.Enqueue(customer);
            }
            // 빵이 없을 경우
            else
            {
                provideFoodCor = null;
                break;
            }

            yield return null;
        }

        // 나갈 고객 큐
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
