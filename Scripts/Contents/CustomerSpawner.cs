using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomerSpawner : MonoBehaviour
{
    [SerializeField]
    GameObject spawnObject;

    IEnumerator spawnCor;

    WaitForSeconds spawnHoldTime;

    Queue<int> registerQueue = new Queue<int>();
    Queue<int> histroyQueue = new Queue<int>();
    int curSpawnCount = 0;
    int spawnIndex = 0;
    int maxSpawnIndex = 2;
    

    private void Awake()
    {
        spawnHoldTime = new WaitForSeconds(2.0f);
    }

    // Start is called before the first frame update
    void Start()
    {
        registerQueue.Enqueue(Random.Range(2, 4));
        spawnCor = SpawnCor();
        StartCoroutine(spawnCor);
    }

    // ť�� �����Ͱ� ������ ���� ���� ��
    IEnumerator SpawnCor()
    {
        while(true)
        {
            if(registerQueue.Count > 0)
            {
                int spawnCount = registerQueue.Dequeue();
                spawnIndex++;
                histroyQueue.Enqueue(spawnCount);
                curSpawnCount += spawnCount;
                while (spawnCount > 0)
                {
                    spawnCount--;
                    GameObject spawnObj = Managers.Resource.Instantiate(spawnObject.name, null, true);
                    spawnObj.transform.position = gameObject.transform.position;

                    yield return spawnHoldTime;
                }
            }

            yield return new WaitForFixedUpdate();
        }
    }


    // ���� ���Ը� ������ ȣ��
    int cnt2 = 0;
    public void CheckOutCustomer()
    {
        // �ִ� ���� �ʰ���
        if (spawnIndex == maxSpawnIndex)
        {
            int histroyCount = histroyQueue.Peek();
            cnt2++;
            // �����丮 ť�� Peek�� ��ŭ�� ���� ������ �ٽ� ����
            if (histroyCount == cnt2)
            {
                histroyQueue.Dequeue();

                registerQueue.Enqueue(Random.Range(2, 4));

                spawnIndex--;

                cnt2 = 0;
            }
        }
    }

    int cnt = 0;
    // ���� ���� �����϶� �߰� ����
    public void RequestSpawn()
    {
        // �ִ� ���� Ƚ�� �ʰ� ��
        // �� �̻� �������� ����
        if (spawnIndex == maxSpawnIndex)
        {
            cnt = 0;
        }
        else
        {
            cnt++;

            // �ش� ť��ŭ cnt�� ī���� �Ǹ� �ٽ� ���� ����
            if (cnt == histroyQueue.Peek())
            {
                cnt = 0;

                registerQueue.Enqueue(Random.Range(2, 4));
            }
        }
           
    }
}
