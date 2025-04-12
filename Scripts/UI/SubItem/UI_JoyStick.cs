using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_JoyStick : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler, IPointerDownHandler, IDragHandler, IPointerUpHandler, IPointerExitHandler
{
    [SerializeField]
    Image background;
    [SerializeField]
    Image handler;
    [SerializeField]
    Image pointer;

    float handlerRadius;

    Vector2 startPos;
    Vector2 moveDir;

    CanvasGroup canvasGroup;

    // Start is called before the first frame update
    void Awake()
    {
        handlerRadius = background.GetComponent<RectTransform>().sizeDelta.y / 2;
        canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0.0f;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {

    }

    public void OnPointerClick(PointerEventData eventData)
    {
      
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        background.transform.position = eventData.position;
        handler.transform.position = eventData.position;
        pointer.transform.position = eventData.position;
        startPos = eventData.position;
        canvasGroup.alpha = 1.0f;
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 dragGap = eventData.position - startPos;
        float dragDis = dragGap.magnitude;
        Vector2 dragDir = dragGap.normalized;

        if (dragDis > handlerRadius)
        {
            handler.transform.position = startPos + dragDir * handlerRadius;
        }
        else
        {
            handler.transform.position = startPos + dragDir * dragDis;
        }

        pointer.transform.position = eventData.position;

        Managers.Input.inputDir = dragDir;
    }

    void IPointerUpHandler.OnPointerUp(PointerEventData eventData)
    {
        handler.transform.position = startPos;
        moveDir = Vector2.zero;

        Managers.Input.inputDir = Vector2.zero;
        canvasGroup.alpha = 0.0f;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        
    }
}
