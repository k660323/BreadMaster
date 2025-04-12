using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    public GameObject target;
    [SerializeField]
    SpriteRenderer arrowSR;

    private void Awake()
    {
        arrowSR = GetComponentInChildren<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (target == null)
            return;

        Vector3 lookVector = (target.transform.position - transform.position).normalized;
        transform.rotation = Quaternion.LookRotation(new Vector3(lookVector.x, 0.0f, lookVector.z));
    }

    public void SetTarget(GameObject target)
    {
        if (target)
            arrowSR.enabled = true;
        else
            arrowSR.enabled = false;

        this.target = target;
    }
}
