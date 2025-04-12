using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStat : Stat
{
    [SerializeField]
    protected int _exp;
    [SerializeField]
    protected int _money;
    public int Money { get { return _money; } set {  _money = value; goldCallbackAction.Invoke(value); } }

    public Action<int> goldCallbackAction;

    private void Start()
    {
        _level = 1;
        _moveSpeed = 5f;
        _exp = 0;
        Money = 0;
        _maxStack = 10;

        SetStat(_level);
    }

    public override int GetCurrentSize()
    {
        return foodLList.Count;
    }

    public override bool IsPushAble()
    {
        return foodLList.Count < _maxStack;
    }

    public override void PushFood(Food food)
    {
        if (IsPushAble() == false)
            return;

        Managers.Sound.Play2D("Get_Object");

        owner.anim.SetBool("bStack", true);

        food.Owner = owner;
        food.col.enabled = false;
        food.rb.isKinematic = true;
        food.rb.useGravity = false;
        food.rb.velocity = Vector3.zero;
        food.transform.SetParent(stackPos);
        food.transform.localRotation = Quaternion.Euler(0.0f, 90.0f, 0.0f);
        food.MoveToPosition(stackPos.transform, new Vector3(0, GetCurrentSize() * yPos, 0));
        
       
        foodLList.AddLast(food);

        if(IsPushAble() == false)
        {
            Player player = owner as Player;
            Vector3 topPos = new Vector3(0, stackPos.transform.position.y + GetCurrentSize() * yPos, 0);
            player.SetMaxCanvas(true, topPos);
        }
    }

    public override bool IsPopAble()
    {
        return foodLList.Count > 0;
    }

    public override Food PopFood()
    {
        if (IsPopAble() == false)
            return null;

        Managers.Sound.Play2D("Put_Object");

        Food food = foodLList.Last.Value;
        foodLList.RemoveLast();

        if (IsPopAble() == false)
        {
            owner.anim.SetBool("bStack", false);
        }

        food.Owner = null;
        return food;
    }

    public void SetStat(int level)
    {
        Dictionary<int, Data.Stat> dict = Managers.Data.StatDict;
        // Data.Stat stat = dict[level];

    }
}
