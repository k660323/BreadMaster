using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class RestSpace : Gimmick, IRegisterable, IColliderinteraction
{
    // 줄 시작 위치
    [SerializeField]
    Transform chairPos;
    // 줄 간격
    [SerializeField]
    float xInterval = 1.5f;

    // 대기중인 고객 큐
    Queue<Customer> customers = new Queue<Customer>();

    [SerializeField]
    GameObject playParticle;

    // 사용후
    [SerializeField]
    bool isUsed = false;

    [SerializeField]
    GameObject trash;

    [SerializeField]
    Transform foodTransform;
    [SerializeField]
    GameObject trashPrefab;

    IEnumerator eatingCor;

    [SerializeField]
    GameObject moneyGroupPrefab;

    WadOfMoney wadOfMoney;

    [SerializeField]
    Transform wadOfMoneySpawnPos;

    public override void Init()
    {
        type = Define.GimmickType.RestSpace;
        Managers.Game.GimmickDic.Add(type, this);
    }

    void OnEnable()
    {
        GameObject particleObj = Managers.Resource.Instantiate(playParticle.name, null, true);
        particleObj.transform.position = transform.position;

        ICallbackAction callbackAction = Managers.Game.GimmickDic[Define.GimmickType.RestRequest] as ICallbackAction;
        callbackAction.CallbackAction();
    }

    public Vector3 GetPlace()
    {
        return new Vector3(chairPos.position.x + (customers.Count) * xInterval,
            chairPos.position.y,
            chairPos.position.z);
    }

    public void Register(Customer customer)
    {
        customers.Enqueue(customer);
        customer.CustomerController.targetPos = GetPlace();
        if (eatingCor == null)
        {
            eatingCor = EatingCor();
            StartCoroutine(eatingCor);
        }
    }

    IEnumerator EatingCor()
    {
        if(customers.Count ==0)
        {
            eatingCor = null;
            yield break;
        }    

        // 먼저 와 있는 고객 앞에 올때 까지 대기
        Customer customer = customers.Peek();
        customer.CustomerController.targetPos = chairPos.position;
        
        while (Vector3.Distance(customers.Peek().transform.position, chairPos.transform.position) > 0.1f)
        {
            yield return null;
        }

        // 앉기
        customer.GetRigidBody.isKinematic = true;
        customer.Nav.enabled = false;
        customer.transform.position = new Vector3(chairPos.position.x , chairPos.position.y + 0.5f, chairPos.position.z);
        customer.transform.forward = chairPos.transform.forward;
        customer.anim.SetTrigger("tSitting");


        // 빵 탁자위로
        int index = 0;
        List<GameObject> foodList = new List<GameObject>();
        while (customer.stat.IsPopAble())
        {
            Food food = customer.stat.PopFood();
            food.transform.SetParent(null);
            food.rb.isKinematic = true;
            food.transform.position = new Vector3(foodTransform.position.x, foodTransform.position.y + index++ * 0.5f, foodTransform.position.z);
            food.transform.rotation = Quaternion.Euler(new Vector3(0, 90.0f, 0));
            foodList.Add(food.gameObject);
        }

        yield return new WaitForSeconds(3.0f);

        isUsed = true;
        // 의자 돌리기
        chairPos.transform.rotation = Quaternion.Euler(new Vector3(0, 230.0f, 0));

        // 빵 제거 쓰레기 생성
        for (int i = 0; i < foodList.Count; i++)
        {
            Managers.Resource.Destroy(foodList[i]);
        }
        foodList.Clear();
       
        trash = Managers.Resource.Instantiate(trashPrefab.name, null, true);
        trash.transform.position = foodTransform.position;

        Managers.Sound.Play2D("trash");

        // 돈 생성
        int money = Random.Range(10, 14);
        if (wadOfMoney == null)
        {
            GameObject moneyGroup = Managers.Resource.Instantiate(moneyGroupPrefab.name, null, true);
            moneyGroup.transform.position = wadOfMoneySpawnPos.position;
            moneyGroup.TryGetComponent(out wadOfMoney);
            wadOfMoney.SetMoney(money);
            wadOfMoney.destoryAction += () => { wadOfMoney = null; };
        }
        else
        {
            wadOfMoney.SetMoney(wadOfMoney.Money + money);

        }
        // 고객 나감
        customers.Dequeue();
        customer.Controller.State = Define.State.ToRestSpaceExit;

        eatingCor = null;
    }

    // 청소
    public void ColliderInteraction()
    {
        if (!isUsed)
            return;

        isUsed = false;
        chairPos.transform.rotation = Quaternion.Euler(new Vector3(0, 180.0f, 0));
        Managers.Resource.Destroy(trash);
        trash = null;

        if (customers.Count > 0)
        {
            // 고객 앞으로 땡기기
            int index = 0;
            foreach (var c in customers)
            {
                Vector3 nextPos = new Vector3(chairPos.position.x + index * xInterval, chairPos.position.y, chairPos.position.z);
                c.CustomerController.StartCoroutine(c.CustomerController.MoveTo(nextPos));
                index++;
            }

            if(eatingCor == null)
            {
                eatingCor = EatingCor();
                StartCoroutine(eatingCor);
            }
        }
    }
}
