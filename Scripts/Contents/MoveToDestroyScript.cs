using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveToDestroyScript : MonoBehaviour
{


    public void MoveLerp(Vector3 destoryPos, float t = 0.5f)
    {
        StartCoroutine(MoneyMoveCor(destoryPos, t));
    }

    IEnumerator MoneyMoveCor(Vector3 destoryPos, float t)
    {
        while (Vector3.Distance(destoryPos, transform.position) > 0.1f)
        {
            transform.position = Vector3.Lerp(transform.position, destoryPos, t);
            yield return null;
        }

        Managers.Resource.Destroy(gameObject);
    }
}
