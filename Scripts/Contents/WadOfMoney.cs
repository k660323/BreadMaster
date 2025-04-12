using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WadOfMoney : MonoBehaviour
{
    // 돈
    protected int money;
    public int Money { get { return money; } set { money = value; } }

    // 머니 좌표
    public static Vector3[] posArr = { new Vector3(-0.85f, 0.0f, 0.5f), new Vector3(0.0f, 0.0f, 0.5f), new Vector3(0.85f, 0.0f, 0.5f),
                                new Vector3(-0.85f, 0.0f, 0.0f), new Vector3(0.0f, 0.0f, 0.0f), new Vector3(0.85f, 0.0f, 0.0f),
                                new Vector3(-0.85f, 0.0f, -0.5f), new Vector3(0.0f, 0.0f, -0.5f), new Vector3(0.85f, 0.0f, -0.5f)};

    // 스폰할 게임 오브젝트
    [SerializeField]
    GameObject moneyPrefab;

    // 머니 게임 오브젝트
    Stack<GameObject> gameObjects = new Stack<GameObject>();

    // 머니 y축 간격
    [SerializeField]
    float yInterval = 0.2f;

    IEnumerator removeCor;

    Collider col;

    public Action destoryAction;

    private void Awake()
    {
        TryGetComponent(out col);
    }

    private void OnEnable()
    {
        col.enabled = true;
    }

    public void SetMoney(int value)
    {
        Managers.Sound.Play2D("cash");

        Money = value;

        int gap = Money - gameObjects.Count;
        for (int i = 0; i < gap; i++)
        {
            int y = gameObjects.Count / 9;
            int x = gameObjects.Count % 9;

            GameObject moneyObj = Managers.Resource.Instantiate(moneyPrefab.name, null, true);
            moneyObj.transform.SetParent(gameObject.transform);
            Vector3 pos = new Vector3(transform.position.x, transform.position.y + y * yInterval, transform.position.z) + posArr[x];
            moneyObj.transform.position = pos;
            gameObjects.Push(moneyObj);
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Player player) == false)
            return;

        col.enabled = false;

        player.PlayerStat.Money += money;

        MoneyRemove();
    }


    public void MoneyRemove()
    {
        if (removeCor != null)
            return;
        destoryAction?.Invoke();
        destoryAction = null;
        removeCor = MoneyRemoveCor();
        StartCoroutine(removeCor);
    }

    IEnumerator MoneyRemoveCor()
    {
        float destoryY = (gameObjects.Count / 10) * yInterval + 3.0f;
        Vector3 destoryPos = new Vector3(transform.position.x, destoryY, transform.position.z);
        while (gameObjects.Count > 0)
        {
            gameObjects.Pop().GetComponent<MoveToDestroyScript>().MoveLerp(destoryPos, 0.3f);
            yield return new WaitForSeconds(0.01f);
        }

        yield return new WaitForSeconds(1.0f);

        Managers.Resource.Destroy(gameObject);
    }

    private void OnDisable()
    {
        money = 0;
        removeCor = null;
    }

}
