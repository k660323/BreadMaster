using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Counter : Gimmick, IRegisterable
{
    // 줄 시작 위치
    [SerializeField]
    Transform startHoldPos;
    // 줄 간격
    [SerializeField]
    float zInterval = 1.5f;

    // 대기중인 고객 큐
    Queue<Customer> customers = new Queue<Customer>();

    // 계산여부 변수
    bool isCalculatealbe;
    // 계산 코루틴
    IEnumerator calculateCor;

    [SerializeField]
    Transform packingTransform;

    [SerializeField]
    GameObject wrappingPaper;
    
    [SerializeField]
    CounterWadOfMoneySpawnPos cWadOfMoneySP;

    public override void Init()
    {
        type = Define.GimmickType.Counter;
        Managers.Game.GimmickDic.Add(type, this);
    }

    public Vector3 GetPlace()
    {
        return new Vector3(startHoldPos.position.x, startHoldPos.position.y, startHoldPos.position.z + (customers.Count - 1) * zInterval);
    }

    public void Register(Customer customer)
    {
        customers.Enqueue(customer);
        customer.CustomerController.targetPos = GetPlace();
    }

    public void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent(out Player player) == false)
            return;

        isCalculatealbe = true;
        // 계산 코루틴
        if (calculateCor == null)
        {
            calculateCor = CalculateCor();
            StartCoroutine(calculateCor);
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out Player player) == false)
            return;

        isCalculatealbe = false;
    }

    IEnumerator CalculateCor()
    {
        if (customers.Count > 0 && Vector3.Distance(customers.Peek().transform.position, startHoldPos.transform.position) < 0.1f)
        {
            // 포장지 생성
            GameObject wpObject = Managers.Resource.Instantiate(wrappingPaper.name);
            wpObject.transform.position = packingTransform.position;
            wpObject.transform.rotation = Quaternion.Euler(new Vector3(0, 90.0f, 0));
            PaperBag paperBag = wpObject.GetComponent<PaperBag>();
            yield return null;
            // 먼저온 고객 음식을 포장에 넣는 애니메이션
            Customer customer = customers.Peek();
            CustomerStat cStat = customer.CustomerStat;
            CustomerController cController = customer.CustomerController;
            while (cStat.IsPopAble())
            {
                Food food = cStat.PopFood();
                Vector3 wpPos = wpObject.transform.position;
                Vector3 center = Vector3.Lerp(food.transform.position, wpPos, 0.5f);

                // 베지어 곡선을 이용해 이동
                food.transform.rotation = Quaternion.Euler(new Vector3(0.0f, 90.0f, 0.0f));
                {
                    float t = 0.0f;
                    while (t < 1.0f)
                    {
                        food.transform.position = Util.BezierCurves(food.transform.position, 1.0f, wpPos, 1.0f, t);
                        food.transform.rotation = Quaternion.Euler(Util.BezierCurves(new Vector3(0.0f, 90.0f, 0.0f), new Vector3(0.0f, 45.0f, 0.0f), new Vector3(0.0f, 0.0f, 0.0f), t));
                        t += 3 * Time.deltaTime;
                        yield return null;
                    }

                    food.transform.position = Util.BezierCurves(food.transform.position, 0.5f, wpPos, 0.5f, 1.0f);
                    food.transform.rotation = Quaternion.Euler(Util.BezierCurves(new Vector3(0.0f, 90.0f, 0.0f), new Vector3(0.0f, 45.0f, 0.0f), new Vector3(0.0f, 0.0f, 0.0f), 1.0f));
                }
               

                // 도착하면 음식 오브젝트 삭제
                Managers.Resource.Destroy(food.gameObject);
                yield return null;
            }
            yield return new WaitForSeconds(0.1f);

            // 모든 음식을 다 넣었으면 포장지 닫는 애니메이션 출력
            paperBag.anim.SetBool("IsClose", true);

            yield return new WaitForSeconds(0.5f);
            paperBag.transform.SetParent(cStat.StackPos);
            paperBag.transform.rotation = Quaternion.Euler(new Vector3(0.0f, 90.0f, 0.0f));

            // 포장지 고객 손에 가도록 이동
            {
                float t = 0.0f;
                while (t < 1.0f)
                {
                    paperBag.transform.position = Util.BezierCurves(paperBag.transform.position, 0.5f, cStat.StackPos.transform.position, 0.5f, t);
                    paperBag.transform.rotation = Quaternion.Euler(Util.BezierCurves(new Vector3(0.0f, 90.0f, 0.0f), new Vector3(0.0f, 45.0f, 0.0f), new Vector3(0.0f, 0.0f, 0.0f), t));
                    t += 2 * Time.deltaTime;
                    yield return null;
                }

                paperBag.transform.position = Util.BezierCurves(paperBag.transform.position, 0.5f, cStat.StackPos.transform.position, 0.5f, 1.0f);
                paperBag.transform.rotation = Quaternion.Euler(Util.BezierCurves(new Vector3(0.0f, 90.0f, 0.0f), new Vector3(0.0f, 45.0f, 0.0f), new Vector3(0.0f, 0.0f, 0.0f), 1.0f));
            }

            // 고객 한테 권한 넘김
            cStat.SetPaperBag(paperBag);

            // 돌려보낸다.
            cController.State = Define.State.ToExit;

            callbackAction?.Invoke();

            // 돈 생성
            cWadOfMoneySP.SpawnWadOfMoney();

            customers.Dequeue();

            // 고객 앞으로 땡기기
            int index = 0;
            foreach (var c in customers)
            {
                Vector3 nextPos = new Vector3(startHoldPos.position.x, startHoldPos.position.y, startHoldPos.position.z + index * zInterval);
                c.CustomerController.StartCoroutine(c.CustomerController.MoveTo(nextPos));
                index++;
            }
        }

        yield return null;

        // 다시 계산
        if (isCalculatealbe)
        {
            calculateCor = CalculateCor();
            StartCoroutine(calculateCor);
        }
        else
        {
            calculateCor = null;
        }
    }
}
