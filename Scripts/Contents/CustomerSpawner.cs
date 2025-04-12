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

    // 큐에 데이터가 있을때 마다 꺼내 씀
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


    // 고객이 가게를 나갈때 호출
    int cnt2 = 0;
    public void CheckOutCustomer()
    {
        // 최대 스폰 초과시
        if (spawnIndex == maxSpawnIndex)
        {
            int histroyCount = histroyQueue.Peek();
            cnt2++;
            // 히스토리 큐의 Peek만 만큼의 고객이 나가면 다시 스폰
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
    // 고객이 생각 상태일때 추가 영입
    public void RequestSpawn()
    {
        // 최대 스폰 횟수 초과 시
        // 더 이상 스폰하지 않음
        if (spawnIndex == maxSpawnIndex)
        {
            cnt = 0;
        }
        else
        {
            cnt++;

            // 해당 큐만큼 cnt가 카운팅 되면 다시 예약 시작
            if (cnt == histroyQueue.Peek())
            {
                cnt = 0;

                registerQueue.Enqueue(Random.Range(2, 4));
            }
        }
           
    }
}
