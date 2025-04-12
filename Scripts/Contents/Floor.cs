using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Floor : MonoBehaviour
{
    Vector3 originScale;

    IEnumerator scaleCor;

    [SerializeField]
    float scaleRate = 1.1f;

    private void Awake()
    {
        originScale = transform.localScale;
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent(out Player player) == false)
            return;

        if (scaleCor != null)
            StopCoroutine(scaleCor);

        scaleCor = ScaleCor(new Vector3(originScale.x * scaleRate, originScale.y, originScale.z * scaleRate));
        StartCoroutine(scaleCor);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.TryGetComponent(out Player player) == false)
            return;

        if (scaleCor != null)
            StopCoroutine(scaleCor);

        scaleCor = ScaleCor(originScale);
        StartCoroutine(scaleCor);
    }

    IEnumerator ScaleCor(Vector3 targetScale)
    {
        while(Vector3.Distance(transform.localScale, targetScale) >= 0.1f)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, targetScale, 0.1f);
            yield return null;
        }
        transform.localScale = targetScale;

        scaleCor = null;
    }
}
