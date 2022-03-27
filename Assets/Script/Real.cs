using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Real : MonoBehaviour, IPointerEnterHandler
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
}
