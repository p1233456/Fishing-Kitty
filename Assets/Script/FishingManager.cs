using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Data;

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

    private Vector2 clickPoint;
    private Vector2 unClickPoint;
    private Sequence sequence;

    float fishingTime;  //경과 시간
    bool isFighting;   //파이팅 중 여부

    Fish biteFish;

    Dictionary<string, KeyValuePair<float, float>> stageProbability;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        gameState = GameState.Preparation;
        SetStageProbability("UpStream");
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

    private void Fail()
    {
        gameState = GameState.Preparation;
    }

    private void Update()
    {
        if (isFighting)
        {
            fishingTime += Time.deltaTime;
        }
        if (Input.GetMouseButtonDown(0))
        {
            if (gameState == GameState.Hooking)
                Hook();
        }
        if (Input.GetMouseButtonUp(0))
        {

        }
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
            Camera.main.transform.position.z), 1f).OnComplete(GetBite);
        Camera.main.DOOrthoSize(3, 1f);
    }

    private void ZoomOut()
    {
        Camera.main.transform.DOLocalMove(new Vector3(0, 0,
              Camera.main.transform.position.z), 1f);
        Camera.main.DOOrthoSize(10, 1f);
    }

    //최대 내려가는거 2
    private void GetBite()
    {
        sequence = DOTween.Sequence();
        sequence.Append(thrownFloat.transform.DOLocalMoveY(thrownFloat.transform.position.y - 1, 0.5f));
        sequence.Append(thrownFloat.transform.DOLocalMoveY(thrownFloat.transform.position.y, 0.5f));
        sequence.Append(thrownFloat.transform.DOLocalMoveY(thrownFloat.transform.position.y - 1, 0.5f));
        sequence.Append(thrownFloat.transform.DOLocalMoveY(thrownFloat.transform.position.y, 0.5f));
        sequence.Append(thrownFloat.transform.DOLocalMoveY(thrownFloat.transform.position.y - 2, 0.5f));
        sequence.OnComplete(HookingMiss);
        sequence.Play();

        GetRandomFish();
    }

    private void GetRandomFish()
    {
        float random = Random.Range(0f, 1f);

        string fishName = null;

        foreach(var probability in stageProbability)
        {
            if (random > probability.Value.Key && random < probability.Value.Value)
                fishName = probability.Key;                 
        }
        biteFish = new Fish(fishName);
        Debug.Log(biteFish.FishName);
    }

    private void SetStageProbability(string stageName)
    {
        stageProbability = new Dictionary<string, KeyValuePair<float, float>>();
        float amount = 0;
        switch (stageName)
        {
            case "UpStream":
                foreach (var row in FishingData.UpStream.AsEnumerable())
                {
                    stageProbability.Add(row.Field<string>("Name"), new KeyValuePair<float, float>(amount, amount + row.Field<float>("Probability")));
                    amount += row.Field<float>("Probability");
                }
                break;
            default:
                Debug.Log("잘못된 스테이지명 입력");
                break;
        }
    }

    public void Hook()
    {
        sequence.Pause();
        float hookPoint = floatPosition.position.y - thrownFloat.transform.position.y;
        Debug.Log(hookPoint);
        Debug.Log(floatPosition.position.y + " " + thrownFloat.transform.position.y);
        if (hookPoint > 1.5f)
        {
            Debug.Log("perfect");
            HookingSuccess(0);
        }
        else if(hookPoint > 1f)
        {
            Debug.Log("great");
            HookingSuccess(1);
        }
        else if(hookPoint > 0.5f)   
        {
            Debug.Log("good");
            HookingSuccess(2);
        }
        else
        {
            Debug.Log("miss");
            HookingMiss();
        }
    }

    private void HookingMiss()
    {
        gameState = GameState.Preparation;
        ZoomOut();
        sequence.Kill(); 
        DestroyImmediate(thrownFloat, true);
    }

    private void HookingSuccess(int level)
    {
        NextState();
        switch (level)
        {
            case 0: //perfect
                break;
            case 1: //great
                break;
            case 2: //good
                break;  
        }
        ZoomOut();
        Destroy(thrownFloat);
        Debug.Log(biteFish.Size);
        fishingTime = 0f;
        isFighting = true;
    }
}
