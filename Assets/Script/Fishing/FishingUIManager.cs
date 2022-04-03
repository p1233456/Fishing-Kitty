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

    [SerializeField] private Image tensionImage;
    [SerializeField] GameObject resultPanel;
    [SerializeField] Text timingText;

    private void Awake()
    {
        Instance = this;        
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
            FishingManager.Instance.BiteFish.Size, "A");
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
}
