using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_GameScene : UI_Scene
{
    GameScene gameScene;

    enum Texts
    {
        MoneyText,
    }

    enum GameObjects
    {
        TouchText
    }


    public override void Init()
    {
        base.Init();

        Bind<Text>(typeof(Texts));
        Bind<GameObject>(typeof(GameObjects));

        Get<GameObject>((int)GameObjects.TouchText).BindEvent((data) => { Get<GameObject>((int)GameObjects.TouchText).gameObject.SetActive(false); });
    }

    private void Start()
    {
        Player player = Managers.Game.player;
        if (player)
        {
            player.PlayerStat.goldCallbackAction -= UpdateMoneyText;
            player.PlayerStat.goldCallbackAction += UpdateMoneyText;
            UpdateMoneyText(player.PlayerStat.Money);
        }
    }

    public void UpdateMoneyText(int val)
    {
        Get<Text>((int)Texts.MoneyText).text = val.ToString();
    }

    public void OnDestroy()
    {
        if(Managers.Game != null)
        {
            Player player = Managers.Game.player;
            if (player)
                player.PlayerStat.goldCallbackAction -= UpdateMoneyText;
        }
      
    }
}
