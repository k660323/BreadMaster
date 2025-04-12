using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Thinking : UI_Base
{
    enum GameObjects
    {
        UI_FoodRequest,
        UI_Counter,
        UI_Emotion,
        UI_Rest
    }

    enum Images
    {
        FoodImage
    }

    enum Texts
    {
        FoodText
    }

    CanvasGroup canvasGroup;
    LookAtCameraUI lookAtCameraUI;

    Customer Owner;

    public override void Init()
    {
        Bind<GameObject>(typeof(GameObjects));
        Bind<Image>(typeof(Images));
        Bind<Text>(typeof(Texts));

        TryGetComponent(out canvasGroup);
        TryGetComponent(out lookAtCameraUI);

        Owner = GetComponentInParent<Customer>();

        DisableUI();
    }

    private void Start()
    {
        Owner.CustomerStat.requireFoodCallbackAction -= UpdateFoodRequireUI;
        Owner.CustomerStat.requireFoodCallbackAction += UpdateFoodRequireUI;
    }

    public void ActiveCounterUI()
    {
        canvasGroup.alpha = 1.0f;
        lookAtCameraUI.enabled = true;
        Get<GameObject>((int)GameObjects.UI_FoodRequest).SetActive(false);
        Get<GameObject>((int)GameObjects.UI_Counter).SetActive(true);
        Get<GameObject>((int)GameObjects.UI_Emotion).SetActive(false);
        Get<GameObject>((int)GameObjects.UI_Rest).SetActive(false);
    }

    public void ActiveEmotionUI()
    {
        canvasGroup.alpha = 1.0f;
        lookAtCameraUI.enabled = true;
        Get<GameObject>((int)GameObjects.UI_FoodRequest).SetActive(false);
        Get<GameObject>((int)GameObjects.UI_Counter).SetActive(false);
        Get<GameObject>((int)GameObjects.UI_Emotion).SetActive(true);
        Get<GameObject>((int)GameObjects.UI_Rest).SetActive(false);
    }

    public void ActiveFoodRequestUI(Define.FoodType foodType, int val)
    {
        canvasGroup.alpha = 1.0f;
        lookAtCameraUI.enabled = true;
        Get<GameObject>((int)GameObjects.UI_FoodRequest).SetActive(true);
        Get<GameObject>((int)GameObjects.UI_Counter).SetActive(false);
        Get<GameObject>((int)GameObjects.UI_Emotion).SetActive(false);
        Get<GameObject>((int)GameObjects.UI_Rest).SetActive(false);
        UpdateFoodRequireUI(foodType, val);
    }

    public void ActiveRestUI()
    {
        canvasGroup.alpha = 1.0f;
        lookAtCameraUI.enabled = true;
        Get<GameObject>((int)GameObjects.UI_FoodRequest).SetActive(false);
        Get<GameObject>((int)GameObjects.UI_Counter).SetActive(false);
        Get<GameObject>((int)GameObjects.UI_Emotion).SetActive(false);
        Get<GameObject>((int)GameObjects.UI_Rest).SetActive(true);
    }

    public void DisableUI()
    {
        canvasGroup.alpha = 0.0f;
        lookAtCameraUI.enabled = false;
        Get<GameObject>((int)GameObjects.UI_Emotion).SetActive(false);
    }

    public void UpdateFoodRequireUI(Define.FoodType foodType, int val)
    {
        // Get<Image>((int)Images.FoodImage).sprite =

        Get<Text>((int)Texts.FoodText).text = val.ToString();
    }

    private void OnDestroy()
    {
        if (Owner)
            Owner.CustomerStat.requireFoodCallbackAction -= UpdateFoodRequireUI;
    }
}
