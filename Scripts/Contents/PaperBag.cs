using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaperBag : MonoBehaviour
{
    public Animator anim;

    private void Awake()
    {
        TryGetComponent(out anim);
    }

    private void OnEnable()
    {
        anim.SetBool("IsClose", false);
    }
}
