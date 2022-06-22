using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//미끼 관리
public class BiteManager : MonoBehaviour
{
    public void SelectBite(string bite)
    {
        FishingManager.Instance.SelectedBite = bite;
        //추후 변경
        FishingManager.Instance.SetStageProbability("UpStream");
    }
}
