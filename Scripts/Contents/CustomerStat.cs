using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class CustomerStat : Stat
{
    // 원하는 음식 갯수
    protected Dictionary<Define.FoodType, int> RequireFoodDic = new Dictionary<Define.FoodType, int>();

    public Action<Define.FoodType, int> requireFoodCallbackAction;

    public int GetRequireFoodValue(Define.FoodType type)
    {
        return RequireFoodDic[type];
    }

    public void SetRequireFoodValue(Define.FoodType type, int val)
    {
        RequireFoodDic[type] = val;

        requireFoodCallbackAction?.Invoke(type, val);
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        RequireFoodDic.Add(Define.FoodType.Bread, UnityEngine.Random.Range(1, 4));
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        RequireFoodDic.Clear();
    }

    private void Start()
    {
        _level = 1;
        _moveSpeed = 5f;
        _maxStack = 10;
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
    }

    public override bool IsPopAble()
    {
        return foodLList.Count > 0;
    }

    public override Food PopFood()
    {
        if (IsPopAble() == false)
            return null;

        // Managers.Sound.Play2D("Put_Object");

        Food food = foodLList.Last.Value;
        foodLList.RemoveLast();

        if (IsPopAble() == false)
        {
            owner.anim.SetBool("bStack", false);
        }

        food.Owner = null;
        return food;
    }
}
