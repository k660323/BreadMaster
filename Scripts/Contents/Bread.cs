using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Bread : Food
{

    public override void Init()
    {
        TryGetComponent(out rb);
        TryGetComponent(out col);
        foodType = Define.FoodType.Bread;
    }

    private void OnDisable()
    {
        Owner = null;
        col.enabled = true;
        rb.isKinematic = false;
        rb.useGravity = true;
        rb.velocity = Vector3.zero;
    }
}
