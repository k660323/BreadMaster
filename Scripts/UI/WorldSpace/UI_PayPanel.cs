using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_PayPanel : UI_Base
{
    protected Panel panel;

    enum Texts
    {
        CostText
    }

    public override void Init()
    {
        Bind<Text>(typeof(Texts));
        panel = GetComponentInParent<Panel>();
    }

    private void Start()
    {
        if(panel)
        {
            panel.upgradeAction -= UpdateCostText;
            panel.upgradeAction += UpdateCostText;
            UpdateCostText(panel.UpgradeCost);
        }
    }

    public void UpdateCostText(int value)
    {
        Get<Text>((int)Texts.CostText).text = value.ToString();
    }
}
