using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

public class Real : MonoBehaviour, IPointerEnterHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    bool isOn = false;

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
        Debug.Log("Real");
    }

    Vector2 dragStartPosition;

    public void OnBeginDrag(PointerEventData pointerEventData)
    {
        dragStartPosition = GetComponent<RectTransform>().anchoredPosition;
        Debug.Log(dragStartPosition);
    }

    public void OnEndDrag(PointerEventData pointerEventData)
    {

        GetComponent<RectTransform>().anchoredPosition = dragStartPosition;
    }

    public void OnDrag(PointerEventData pointerEventData)
    {
        Vector2 mousePos = Input.mousePosition;
        GetComponent<RectTransform>().position = new Vector2(Camera.main.ScreenToWorldPoint(mousePos).x,
            Camera.main.ScreenToWorldPoint(mousePos).y);
        //float w = transform_icon.rect.width;
        //float h = transform_icon.rect.height;
        //transform_icon.position = transform_cursor.position + (new Vector3(w, h) * 0.5f);
    }
}
