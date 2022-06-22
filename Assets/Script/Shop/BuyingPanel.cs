using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuyingPanel : MonoBehaviour
{
    int haveItemNumber; //보유중인 아이템 갯수
    [SerializeField] string itemName;   //아이템 이름
    [SerializeField] Text haveItemNumberText;
    [SerializeField] int price;

    private void Start()
    {
        Debug.Log(itemName);
        if (!PlayerPrefs.HasKey(itemName))
        {
            PlayerPrefs.SetInt(itemName, 0);
        }
    }

    private void Update()
    {
        haveItemNumber = PlayerPrefs.GetInt(itemName);
        haveItemNumberText.text = haveItemNumber.ToString();
    }


    //아이템 구매
    public void BuyItem()
    {
        if(PlayerPrefs.GetInt("Money",0) >= price )
        {
            PlayerPrefs.SetInt(itemName, PlayerPrefs.GetInt(itemName) + 1);
            PlayerPrefs.SetInt("Money", PlayerPrefs.GetInt("Money") - price);
        }
    }
}
    