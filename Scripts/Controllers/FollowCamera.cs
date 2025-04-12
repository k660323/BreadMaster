using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    [SerializeField]
    GameObject lookTarget;

    [SerializeField]
    float zDistance = 5.5f;

    [SerializeField]
    float yDistance = 10.0f;

    // Start is called before the first frame update
    void Awake()
    {
        lookTarget = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (lookTarget == null)
            return;

        Vector3 lookPos = new Vector3(lookTarget.transform.position.x, lookTarget.transform.position.y + yDistance, lookTarget.transform.position.z - zDistance);
        transform.position = Vector3.Lerp(transform.position, lookPos, 0.5f);
    }

    public void SetTarget(GameObject target)
    {
        lookTarget = target;
    }

    public void LookAtPostion(Vector3 targetPos, float speed)
    {
        StartCoroutine(LookAtPostionCor(targetPos, speed));
    }

    IEnumerator LookAtPostionCor(Vector3 targetPos, float speed)
    {
        GameObject cache = lookTarget;
        lookTarget = null;

        Vector3 lookPos = new Vector3(targetPos.x, targetPos.y + yDistance, targetPos.z - zDistance);
        while (Vector3.Distance(transform.position, lookPos) > 1.0f)
        {
            transform.position = Vector3.Lerp(transform.position, lookPos, speed * Time.deltaTime);
            yield return null;
            lookPos = new Vector3(targetPos.x, targetPos.y + yDistance, targetPos.z - zDistance);
        }

        yield return new WaitForSeconds(1.0f);

        lookPos = new Vector3(cache.transform.position.x, cache.transform.position.y + yDistance, cache.transform.position.z - zDistance);
        while (Vector3.Distance(transform.position, lookPos) > 1.0f)
        {
            transform.position = Vector3.Lerp(transform.position, lookPos, speed * Time.deltaTime);
            yield return null;
            lookPos = new Vector3(cache.transform.position.x, cache.transform.position.y + yDistance, cache.transform.position.z - zDistance);
        }
        transform.position = lookPos;

        lookTarget = cache;

    }
}
