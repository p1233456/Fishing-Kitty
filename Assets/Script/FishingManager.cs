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

    int lastFishingTime = 0;
    float fishingTime = 0f;  //경과 시간

    public Fish BiteFish { get; private set; }

    Dictionary<string, KeyValuePair<float, float>> stageProbability;

    float tensionRate = 0f;
    public float TensionRate { get { return tensionRate; } }
    float maxTensionRate = 1f;

    [SerializeField] Transform handle;

    [SerializeField] Transform rotatePoint;

    bool isRealling = false;

    [SerializeField] int damage;

    [SerializeField] GameObject fishShadow;
    [SerializeField] GameObject rode;
    float distance;
    [SerializeField] Transform landinPoint;

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
                tensionRate = 0f;
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
        if (gameState == GameState.Fighting)
        {
            MoveRode();
            distance = Vector2.Distance(landinPoint.position, BiteFish.transform.position);
            fishingTime += Time.deltaTime;
            if (isRealling)
            {
                Realling();
                if (Input.GetMouseButtonUp(0))
                {
                    RealingEnd();
                }
            }
            else
            {
                NotRealing();
            }

            if (tensionRate > maxTensionRate + 0.5f)
                Fail();

            if (lastFishingTime + 1 < fishingTime)
            {
                DamageFish();
                lastFishingTime = Mathf.FloorToInt(fishingTime);
                //Debug.Log("경과 : " + lastFishingTime + "거리 : " + distance);  
            }
            if(distance <= 0.1f)
            {
                GetFish();
            }
        }

        if (Input.GetMouseButtonDown(0))
        {
            if (gameState == GameState.Hooking)
                Hook();
        }
        if (Input.GetMouseButtonUp(0))
        {

        }
        if(gameState == GameState.Result)
        {
            
        }
    }

    private void Fail()
    {
        fishingTime = 0f;
        lastFishingTime = 0;
        gameState = GameState.Preparation;
        if(BiteFish != null)
        {
            Destroy(BiteFish.gameObject);
            BiteFish = null;
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

        foreach (var probability in stageProbability)
        {
            if (random > probability.Value.Key && random < probability.Value.Value)
                fishName = probability.Key;
        }
        BiteFish = Instantiate(fishShadow).GetComponent<Fish>();
        BiteFish.SetFish(fishName);
        Debug.Log(BiteFish.FishName);
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
        else if (hookPoint > 1f)
        {
            Debug.Log("great");
            HookingSuccess(1);
        }
        else if (hookPoint > 0.5f)
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
        Fail();
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
        Debug.Log(BiteFish.Size);
        fishingTime = 0f;
        gameState = GameState.Fighting;
    }

    private void SetMaxTensionRate(float maxTension)
    {
        this.maxTensionRate = maxTension;
    }

    public void RealingStart()
    {
        isRealling = true;
    }

    public void RealingEnd()
    {
        isRealling = false;
    }

    public void Realling (){
        tensionRate += 1f * Time.deltaTime;
        MoveHandle();
        BiteFish.Move(landinPoint.position);
    }

    public void NotRealing()
    {
        tensionRate -= 0.5f * Time.deltaTime;
    }

    private void MoveHandle()
    {
        //Debug.Log("MoveHandle");
        handle.RotateAround(rotatePoint.position, Vector3.back, 10f);
    }

    private void DamageFish()
    {
        BiteFish.GetDamage(Mathf.FloorToInt(tensionRate * damage));
    }

    private void MoveRode()
    {
        Vector2 dir = new Vector2(BiteFish.transform.position.x - landinPoint.transform.position.x,
            BiteFish.transform.position.y - landinPoint.transform.position.y);
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        rode.transform.rotation = Quaternion.AngleAxis(angle - 90, new Vector3(0,0,1));
    }

    private void GetFish()
    {
        Destroy(BiteFish.gameObject);
        NextState();    //result로 State 전환
    }

    private void TryStunSnap(Vector2 direction)
    {
        if(direction.x > 0)
        {
            if(BiteFish.transform.position.x < 0)
            {
                StunSnap(Mathf.Abs(BiteFish.transform.position.x));
            }
        }
        else
        {
            if (BiteFish.transform.position.x > 0)
            {
                StunSnap(Mathf.Abs(BiteFish.transform.position.x));
            }
        }
    }

    private void StunSnap(float power)
    {
        Debug.Log("스턴");
    }
}
