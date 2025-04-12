using System;
using System.Collections;
using UnityEngine;

public class Panel : MonoBehaviour, ICallbackAction
{
    [SerializeField]
    protected Define.PanelType panelType;

    // UI 정보
    protected UI_PayPanel payPanel;

    [SerializeField]
    protected int upgradeCost = 30;

    public int UpgradeCost { get { return upgradeCost; } set {  upgradeCost = value;  upgradeAction?.Invoke(value); } }

    public Action<int> upgradeAction;
    protected Action callbackAction;

    [SerializeField]
    protected GameObject spawnObj;

    protected IEnumerator stopCheckCor;

    [SerializeField]
    protected GameObject moneyPrefab;

    [SerializeField]
    protected bool IsVisible = true;

    protected virtual void Awake()
    {
        payPanel = GetComponentInChildren<UI_PayPanel>();
        Managers.Game.PanelDic.Add(panelType, this);

        if (!IsVisible)
            gameObject.SetActive(false);
    }


    private void OnTriggerEnter(Collider other)
    {
        other.TryGetComponent(out Player player);
        if (player == null)
            return;

        if (stopCheckCor != null)
            return;

        stopCheckCor = StopCheckCor();
        StartCoroutine(stopCheckCor);
    }

    private void OnTriggerExit(Collider other)
    {
        other.TryGetComponent(out Player player);
        if (player == null)
            return;

        if(stopCheckCor != null)
            StopCoroutine(stopCheckCor);

        stopCheckCor = null;
    }

    IEnumerator StopCheckCor()
    {
        while(true)
        {
            if (Managers.Input.inputDir == Vector2.zero)
            {
                if (Managers.Game.player.PlayerStat.Money >= UpgradeCost)
                {
                    Managers.Sound.Play2D("Cost_Money");

                    // 애니메이션 출력
                    Vector3 destroyPos = new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z);
                    while(UpgradeCost > 0)
                    {
                        GameObject moneyObj = Managers.Resource.Instantiate(moneyPrefab.name, null, true);
                        moneyObj.transform.position = Managers.Game.player.transform.position;
                        moneyObj.GetComponent<MoveToDestroyScript>().MoveLerp(destroyPos, 0.1f);
                        yield return new WaitForSeconds(0.05f);
                        UpgradeCost--;
                        Managers.Game.player.PlayerStat.Money--;
                    }

                    Managers.Sound.Play2D("Success");
                    GameObject restObj = Managers.Resource.Instantiate(spawnObj.gameObject.name);
                    restObj.transform.position = transform.position;
                    callbackAction?.Invoke();
                    gameObject.SetActive(false);
                    yield break;
                }
            }

            yield return new WaitForSeconds(0.1f);
        }
    }

    public void RegisterCallbackAction(Action action)
    {
        callbackAction -= action;
        callbackAction += action;
    }

    public void CallbackAction()
    {
        callbackAction?.Invoke();
    }

    public void RemoveCallbackAction(Action action)
    {
        callbackAction -= action;
    }

    public void RemoveAllCallbackAction()
    {
        callbackAction = null;
    }
}
