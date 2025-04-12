using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Food : MonoBehaviour
{
    protected Define.FoodType foodType;

    public Define.FoodType GetFoodType {  get { return foodType; } }

    protected Creature owner;
    public Creature Owner { get { return owner; } set { owner = value; } }

    protected IEnumerator moveCor;

    [HideInInspector]
    public Rigidbody rb;
    [HideInInspector]
    public Collider col;

    private void Awake()
    {
        Init();
    }

    public abstract void Init();

    public void MoveToPosition(Transform targetTransform, Vector3 offsetPos)
    {
        if (moveCor != null)
            StopCoroutine(moveCor);

        moveCor = MoveCor(targetTransform, offsetPos);
        StartCoroutine(moveCor);
    }

    IEnumerator MoveCor(Transform targetTransform, Vector3 offsetPos)
    {
        Vector3 destination = targetTransform.transform.position + offsetPos;
        float distance = (destination - transform.position).magnitude;

        while (distance > 0.1f)
        {
            transform.position = Vector3.Lerp(transform.position, destination, 0.1f);
            yield return null;

            destination = targetTransform.transform.position + offsetPos;
            distance = (destination - transform.position).magnitude;
        }

        transform.position = destination;
        moveCor = null;
    }
}
