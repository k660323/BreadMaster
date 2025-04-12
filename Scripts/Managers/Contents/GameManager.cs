using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameManager
{
    public Player player;

    public Dictionary<Define.GimmickType, Gimmick> GimmickDic = new Dictionary<Define.GimmickType, Gimmick>();
    public Dictionary<Define.PanelType, Panel> PanelDic = new Dictionary<Define.PanelType, Panel>();
    public HashSet<GameObject> customerDic = new HashSet<GameObject>();

    public bool restOpen = false;

}
