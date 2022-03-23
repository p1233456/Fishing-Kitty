using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public enum GameState
{
    Preparation,
    Waiting,
    Hooking,
    Fighting,
    Result
}

public class FishingManager : MonoBehaviour
{
    static private FishingManager instance;
    static public FishingManager Instance
    {
        get { return instance; }
    }
    [SerializeField] private GameObject fishingFloat;
    [SerializeField] private Transform floatPosition;
    public GameState gameState;
    private GameObject thrownFloat;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        gameState = GameState.Preparation;
    }

    private void NextState()
    {
        switch (gameState)
        {
            case GameState.Preparation:
                gameState = GameState.Hooking;
                break;
            case GameState.Hooking:
                gameState = GameState.Fighting;
                break;
            case GameState.Fighting:
                gameState = GameState.Result;
                break;
            case GameState.Result:
                gameState = GameState.Preparation;
                break;
        }
    }

    private void Update()
    {

    }

    public void Casting()
    {
        if (gameState != GameState.Preparation)
            return;
        //Vector3 screenPos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10); 
        //Vector3 worldPos = Camera.main.ScreenToWorldPoint(screenPos);
        thrownFloat = Instantiate(fishingFloat);
        thrownFloat.transform.position = floatPosition.position;
        NextState();
        ZoomIn();
    }

    private void ZoomIn()
    {
        Camera.main.transform.DOLocalMove(new Vector3(floatPosition.transform.position.x, floatPosition.transform.position.y,
            Camera.main.transform.position.z), 1f);
        Camera.main.DOOrthoSize(3, 1f).OnComplete(()=> Debug.Log("³¡"));
    }

    private void ZoomOut()
    {
        Camera.main.transform.DOLocalMove(new Vector3(0, 0,
              Camera.main.transform.position.z), 1f);
        Camera.main.DOOrthoSize(10, 1f).OnComplete(() => Debug.Log("³¡"));
    }

    private void GetBite()
    {

    }
}
