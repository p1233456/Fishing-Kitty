using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FishingUIManager : MonoBehaviour
{
    public static FishingUIManager Instance { get; private set; }
    public GameObject[] preparationUI;
    public GameObject[] castingUI;
    public GameObject[] hookingUI;
    public GameObject[] fightingUI;
    public GameObject[] resultUI;

    [SerializeField] private Text[] missionTexts;
    [SerializeField] private Image tensionImage;
    [SerializeField] GameObject resultPanel;
    [SerializeField] Text timingText;

    [SerializeField] GameObject retryPannel;
    [SerializeField] GameObject bitePannel;

    [SerializeField] GameObject missionStamp;

    [SerializeField] Text price;
    private void Awake()
    {
        Instance = this;
        HideRetryButton();
        HideBitePanel();
    }

    private void Update()
    {
        //Debug.Log(FishingManager.Instance.State);
        switch (FishingManager.Instance.State)
        {
            case GameState.Preparation:
                Appear(preparationUI);
                Hide(castingUI);
                Hide(hookingUI);
                Hide(fightingUI);
                Hide(resultUI);
                ShowMissions();
                break;
            case GameState.Hooking:
                Appear(hookingUI);
                Hide(castingUI);
                Hide(preparationUI);
                Hide(fightingUI);
                Hide(resultUI);
                break;
            case GameState.Fighting:
                Appear(fightingUI);
                Hide(castingUI);
                Hide(hookingUI);
                Hide(preparationUI);
                Hide(resultUI);
                ViewTensionGage();
                break;
            case GameState.Result:
                Appear(resultUI);
                Hide(castingUI);
                Hide(hookingUI);
                Hide(fightingUI);
                Hide(preparationUI);
                UpdateResult();
                break;
        }
    }

    private void Hide(GameObject[] targets)
    {
        if (targets.Length == 0)
            return;
        if (!targets[0].activeInHierarchy)
            return;

        foreach (GameObject target in targets)
        {
            target.SetActive(false);
        }
    }

    private void Appear(GameObject[] targets)
    {
        if (targets.Length == 0)
            return;
        if (targets[0].activeInHierarchy)
            return;

        foreach (GameObject target in targets)
        {
            target.SetActive(true);
        }
    }

    private void ViewTensionGage()
    {
        if(FishingManager.Instance.TensionRate < FishingManager.Instance.MaxTensionRate)
        {
            tensionImage.fillAmount = FishingManager.Instance.TensionRate;
        }
    }

    private void UpdateResult()
    {
        resultPanel.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
        resultPanel.GetComponent<ResultPanel>().SetPanel(FishingManager.Instance.BiteFish.FishName, 
            FishingManager.Instance.BiteFish.Size, "신선해요!");
    }

    public void ViewHookTiming(string result)
    {
        timingText.text = result;
        Invoke("HideTimingText", 0.5f);
    }

    private void HideTimingText()
    {
        timingText.gameObject.SetActive(false);
    }

    public void ViewRetryButton()
    {
        retryPannel.SetActive(true);
        Time.timeScale = 0;
    }

    public void HideRetryButton()
    {
        retryPannel.SetActive(false);
        Time.timeScale = 1;
    }

    public void ViewBitePanel()
    {
        bitePannel.SetActive(true);
    }

    public void HideBitePanel()
    {
        bitePannel.SetActive(false);
    }

    public void ShowMissions()
    {
        for(int i =0; i < missionTexts.Length; i++)
        {
            //Debug.Log($"{MissionManager.Instance.AssignedMissions[i].TargetName} ({MissionManager.Instance.AssignedMissions[i].CurrentCount}");
            missionTexts[i].text = $"{MissionManager.Instance.AssignedMissions[i].TargetName} ({MissionManager.Instance.AssignedMissions[i].CurrentCount} / {MissionManager.Instance.AssignedMissions[i].GoalCount})";
        }
    }

    public void ShowStamp()
    {
        missionStamp.SetActive(true);
    }

    public void HideStamp()
    {
        missionStamp.SetActive(false);
    }

    public void SetPrice(int price)
    {
        this.price.text = price + "원";
    }
}
