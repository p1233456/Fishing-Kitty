using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuyingPanel : MonoBehaviour
{
    int haveItemNumber; //보유중인 아이템 갯수
    int wantItemNumber; //구매를 원하는 아이템 갯수
    [SerializeField] string itemName;   //아이템 이름
    [SerializeField] InputField wantItemNumberInputField; //구매하길 원하는 수량 입력 필드
    [SerializeField] Text haveItemNumberText;
    [SerializeField] GameObject itemInfoPanel;

    private void Start()
    {
        Debug.Log(itemName);
        if (!PlayerPrefs.HasKey(itemName))
        {
            PlayerPrefs.SetInt(itemName, 0);
        }
        HideItemInformation();
    }

    private void Update()
    {
        haveItemNumber = PlayerPrefs.GetInt(itemName);
        haveItemNumberText.text = haveItemNumber.ToString();
    }

    public void SetWantItemNumber()
    {
        Debug.Log(wantItemNumberInputField.text.ToString());
        wantItemNumber = int.Parse(wantItemNumberInputField.text);
    }


    //아이템 구매
    public void BuyItem()
    {
        PlayerPrefs.SetInt(itemName, wantItemNumber);
    }

    //아이템 정보 출력
    public void ShowItemInformation()
    {
        itemInfoPanel.SetActive(true);
    }

    //아이템 정보 숨기기
    public void HideItemInformation()
    {
        itemInfoPanel.SetActive(false);
    }

    //원하는 아이템 갯수 1개 증가
    public void IncreaseWantItem()
    {
        wantItemNumber++;
    }

    //원하는 아이템 갯수 1개 감소
    public void DecreaseOneWantItem()
    {
        wantItemNumber--;
    }
}
    