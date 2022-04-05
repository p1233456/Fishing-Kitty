using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//�̳� ����
public class BiteManager : MonoBehaviour
{
    public void SelectBite(string bite)
    {
        FishingManager.Instance.SelectedBite = bite;
        //���� ����
        FishingManager.Instance.SetStageProbability("UpStream");
    }
    public void UseBite(string bite)
    {
        PlayerPrefs.SetInt(bite, PlayerPrefs.GetInt(bite) - 1);
    }
}
