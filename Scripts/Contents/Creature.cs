using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public abstract class Creature : MonoBehaviour
{
    public ObjectType ObjectType { get; protected set; }

    private BaseController controller;
    public BaseController Controller { get { return controller; } }

    public Animator anim;

    public Stat stat;

    private void Awake()
    {
        Init();
    }

    protected virtual void Init()
    {
        TryGetComponent(out controller);
        anim = GetComponentInChildren<Animator>();
        TryGetComponent(out stat);
    }
}
