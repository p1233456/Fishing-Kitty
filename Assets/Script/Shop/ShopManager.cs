using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopManager : MonoBehaviour
{
    [SerializeField] Text havingMoney;

    // Update is called once per frame
    void Update()
    {
        havingMoney.text = PlayerPrefs.GetInt("Money", 0) + "원";
    }

    public void BuyItem()
    {
        
    }
}
