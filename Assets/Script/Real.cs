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
        dragStartPosition = transform.position;
        Debug.Log(dragStartPosition);
    }

    public void OnEndDrag(PointerEventData pointerEventData)
    {

        transform.DOLocalMove(dragStartPosition, 1f);
    }

    public void OnDrag(PointerEventData pointerEventData)
    {
        transform.position = Input.mousePosition;
    }
}
