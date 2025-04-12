using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Table : MonoBehaviour
{
    [SerializeField]
    Gimmick gimmick;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent(out Player player) == false)
            return;

        if (gimmick is IColliderinteraction colliderinteraction)
        {
            colliderinteraction.ColliderInteraction();
        }
    }
}
