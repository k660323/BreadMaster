using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.XR;

public abstract class Stat : MonoBehaviour
{
    [SerializeField]
    protected Creature owner;
    public Creature Owner { get { return owner; } }

    [SerializeField]
    protected int _level;
    [SerializeField]
    protected float _moveSpeed;

    // 현재 리스트에 있는 음식
    [SerializeField]
    protected LinkedList<Food> foodLList = new LinkedList<Food>();

    // 최대 빵 스택
    [SerializeField]
    protected int _maxStack;

    public int Level { get { return _level; } set { _level = value; } }
    public float MoveSpeed { get { return _moveSpeed; } set { _moveSpeed = value; } }
    public int MaxStack { get { return _maxStack;} set { _maxStack = value; } }

    // 빵 위치
    [SerializeField]
    protected Transform stackPos;

    public Transform StackPos { get { return stackPos; } }

    // 스택 당 y 포지션
    protected float yPos = 0.5f;

    public PaperBag paperBag;

    private void Awake()
    {
        TryGetComponent(out owner);
    }

    protected virtual void OnEnable()
    {
        
    }

    protected virtual void OnDisable()
    {
        if(paperBag)
        {
            paperBag.transform.parent = null;
            Managers.Resource.Destroy(paperBag.gameObject);
            paperBag = null;
        }
    }

    private void Start()
    {
        _level = 1;
        _moveSpeed = 5f;
    }

    public abstract int GetCurrentSize();

    public abstract bool IsPushAble();

    public abstract void PushFood(Food food);

    public abstract bool IsPopAble();

    public abstract Food PopFood();

    public virtual void SetPaperBag(PaperBag pBag)
    {
        paperBag = pBag;
      
        owner.anim.SetBool("bStack", true);
    }

    public void RemoveAllFood()
    {
        if (IsPopAble() == false)
            return;

        while(foodLList.Count > 0)
        {
            Food food = foodLList.Last.Value;
            foodLList.RemoveLast();
            Managers.Resource.Destroy(food.gameObject);
        }

        if (IsPopAble() == false)
        {
            owner.anim.SetBool("bStack", false);
        }
    }
}
