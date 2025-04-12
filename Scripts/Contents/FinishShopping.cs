using Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinishShopping : MonoBehaviour
{
    CustomerSpawner customerSpawner;

    private void Awake()
    {
        customerSpawner = GetComponentInParent<CustomerSpawner>();
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.TryGetComponent(out Customer customer) == false)
            return;

        Vector3 outDir = (other.transform.position - transform.position).normalized;
        Vector3 forward = transform.forward;

        float dot = Vector3.Dot(forward, outDir);
        float radian = Mathf.Acos(dot);
        float angle = radian * Mathf.Rad2Deg;

        if(0 <= angle && angle <= 90.0f)
        {
            customerSpawner.CheckOutCustomer();
            Managers.Resource.Destroy(other.gameObject);
        }

    }
}
