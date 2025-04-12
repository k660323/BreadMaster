using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Counter : Gimmick, IRegisterable
{
    // �� ���� ��ġ
    [SerializeField]
    Transform startHoldPos;
    // �� ����
    [SerializeField]
    float zInterval = 1.5f;

    // ������� �� ť
    Queue<Customer> customers = new Queue<Customer>();

    // ��꿩�� ����
    bool isCalculatealbe;
    // ��� �ڷ�ƾ
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
        // ��� �ڷ�ƾ
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
            // ������ ����
            GameObject wpObject = Managers.Resource.Instantiate(wrappingPaper.name);
            wpObject.transform.position = packingTransform.position;
            wpObject.transform.rotation = Quaternion.Euler(new Vector3(0, 90.0f, 0));
            PaperBag paperBag = wpObject.GetComponent<PaperBag>();
            yield return null;
            // ������ �� ������ ���忡 �ִ� �ִϸ��̼�
            Customer customer = customers.Peek();
            CustomerStat cStat = customer.CustomerStat;
            CustomerController cController = customer.CustomerController;
            while (cStat.IsPopAble())
            {
                Food food = cStat.PopFood();
                Vector3 wpPos = wpObject.transform.position;
                Vector3 center = Vector3.Lerp(food.transform.position, wpPos, 0.5f);

                // ������ ��� �̿��� �̵�
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
               

                // �����ϸ� ���� ������Ʈ ����
                Managers.Resource.Destroy(food.gameObject);
                yield return null;
            }
            yield return new WaitForSeconds(0.1f);

            // ��� ������ �� �־����� ������ �ݴ� �ִϸ��̼� ���
            paperBag.anim.SetBool("IsClose", true);

            yield return new WaitForSeconds(0.5f);
            paperBag.transform.SetParent(cStat.StackPos);
            paperBag.transform.rotation = Quaternion.Euler(new Vector3(0.0f, 90.0f, 0.0f));

            // ������ �� �տ� ������ �̵�
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

            // �� ���� ���� �ѱ�
            cStat.SetPaperBag(paperBag);

            // ����������.
            cController.State = Define.State.ToExit;

            callbackAction?.Invoke();

            // �� ����
            cWadOfMoneySP.SpawnWadOfMoney();

            customers.Dequeue();

            // �� ������ �����
            int index = 0;
            foreach (var c in customers)
            {
                Vector3 nextPos = new Vector3(startHoldPos.position.x, startHoldPos.position.y, startHoldPos.position.z + index * zInterval);
                c.CustomerController.StartCoroutine(c.CustomerController.MoveTo(nextPos));
                index++;
            }
        }

        yield return null;

        // �ٽ� ���
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
