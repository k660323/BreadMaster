using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameScene : BaseScene
{
    [SerializeField]
    CustomerSpawner spawner;

    protected override void Init()
    {
        base.Init();

        // æ¿ ≈∏¿‘ º≥¡§
        SceneType = Define.Scene.Game;

        Managers.Resource.LoadAllSync<UnityEngine.Object>("PreLoad");
        StartLoaded();
    }

    protected override void StartLoaded()
    {
        base.StartLoaded();
        Managers.Sound.Init();
        Managers.Data.Init();
        Managers.Input.Init();
        Managers.UI.ShowSceneUI<UI_GameScene>();
        spawner = GameObject.FindObjectOfType<CustomerSpawner>();
    }

    public void RequestSpawn()
    {
        spawner.RequestSpawn();
    }

    public override void Clear()
    {
        Debug.Log("GameScene Clear!");
    }
}
