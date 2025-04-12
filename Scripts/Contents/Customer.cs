using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Customer : Creature
{
    CustomerStat customerStat;
    public CustomerStat CustomerStat { get { return customerStat; } protected set { customerStat = value; } }

    NavMeshAgent nav;

    public NavMeshAgent Nav { get { return nav; } protected set { nav = value; } }

    private CustomerController customerController;
    public CustomerController CustomerController { get { return customerController; } }

    private Rigidbody rb;
    public Rigidbody GetRigidBody { get { return rb; } }

    protected UI_Thinking ui_Thinking;

    protected override void Init()
    {
        base.Init();
        TryGetComponent(out nav);
        TryGetComponent(out rb);
        ui_Thinking = GetComponentInChildren<UI_Thinking>();
        ObjectType = Define.ObjectType.Customer;
        CustomerStat = stat as CustomerStat;
        customerController = Controller as CustomerController;
    }

    private void OnEnable()
    {
        Managers.Game.customerDic.Add(gameObject);
    }


    private void OnDisable()
    {
        ui_Thinking.DisableUI();
        Nav.obstacleAvoidanceType = UnityEngine.AI.ObstacleAvoidanceType.NoObstacleAvoidance;
        
        if (Application.isPlaying && Managers.Game.customerDic.Contains(gameObject))
            Managers.Game.customerDic.Remove(gameObject);
    }


    public void ActiveFoodRequestUI(Define.FoodType foodType, bool isActive)
    {
        if (isActive)
            ui_Thinking.ActiveFoodRequestUI(foodType, customerStat.GetRequireFoodValue(foodType));
        else
            ui_Thinking.DisableUI();
    }

    public void ActiveEmotionUI(bool isActive)
    {
        if (isActive)
            ui_Thinking.ActiveEmotionUI();
        else
            ui_Thinking.DisableUI();
    }

    public void ActiveCounterUI(bool isActive)
    {
        if (isActive)
            ui_Thinking.ActiveCounterUI();
        else
            ui_Thinking.DisableUI();
    }

    public void ActiveResetUI(bool isActive)
    {
        if (isActive)
            ui_Thinking.ActiveRestUI();
        else
            ui_Thinking.DisableUI();
    }

    public void DisableUI()
    {
        ui_Thinking.DisableUI();
    }
}
