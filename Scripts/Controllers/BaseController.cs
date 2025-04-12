using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public abstract class BaseController : MonoBehaviour
{
    [SerializeField]
    protected Define.State _state = Define.State.Idle;

    public virtual Define.State State
    {
        get { return _state; }
        set
        {
            _state = value;
            switch (_state)
            {
                case Define.State.Idle:

                    break;
                case Define.State.Moving:

                    break;
            }
        }
    }

    [SerializeField]
    protected Creature owner;
    public Creature Owner { get { return owner; } }
   

    public void Awake()
    {
        Init(); 
    }

    public abstract void Init();
}
