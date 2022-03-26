using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishingUIManager : MonoBehaviour
{
    public GameObject[] preparationUI;
    public GameObject[] castingUI;
    public GameObject[] hookingUI;
    public GameObject[] fightingUI;
    public GameObject[] resultUI;

    private void Update()
    {
        Debug.Log(FishingManager.Instance.gameState);
        switch (FishingManager.Instance.gameState)
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
                break;
            case GameState.Result:
                Appear(resultUI);
                Hide(castingUI);
                Hide(hookingUI);
                Hide(fightingUI);
                Hide(preparationUI);
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
}
