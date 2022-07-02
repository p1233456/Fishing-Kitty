using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

public class Real : MonoBehaviour, IPointerEnterHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    bool isOn = false;
    private RectTransform rect;

    private void Start() {
        rect = GetComponent<RectTransform>();
    }

    private void Update()
    {
        if (isOn)
        {
            if (Input.GetMouseButtonDown(0))
            {
                FishingManager.Instance.RealingStart();
            }
        }
        if (Input.GetMouseButtonUp(0))
        {
            FishingManager.Instance.RealingEnd();
        }
    }
    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        isOn = true;
    }

    Vector2 dragStartPosition;

    public void OnBeginDrag(PointerEventData pointerEventData)
    {
        dragStartPosition = rect.anchoredPosition;
        Debug.Log(dragStartPosition);
    }

    public void OnEndDrag(PointerEventData pointerEventData)
    {

        FishingManager.Instance.TryStunSnap(rect.anchoredPosition - dragStartPosition);
        GetComponent<RectTransform>().anchoredPosition = dragStartPosition;
    }

    public void OnDrag(PointerEventData pointerEventData)
    {
        Vector2 mousePos = Input.mousePosition;
        //Debug.Log(mousePos);
        rect.position = new Vector3(mousePos.x,mousePos.y,rect.position.z);//(Camera.main.ScreenToViewportPoint(mousePos).x,
        //    Camera.main.ScreenToViewportPoint(mousePos).y);
        //float w = transform_icon.rect.width;
        //float h = transform_icon.rect.height;
        //transform_icon.position = transform_cursor.position + (new Vector3(w, h) * 0.5f);
    }
}
