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

    public GameState State { get; set; }

    #region 케스팅 관련 변수
    [SerializeField] private GameObject fishingFloat;   //낚시 찌 프레펩
    [SerializeField] private Transform floatPosition;   //찌 위치
    private GameObject thrownFloat;                     //찌
    private Sequence sequence;                          //두트윈 
    #endregion

    #region 후킹 관련 변수
    #endregion

    #region 파이팅 관련 변수
    float distance;
    [SerializeField] float fightingTime = 0f;  //경과 시간
    #endregion

    #region 결과 관련 변수
    #endregion
    public Fish BiteFish { get; private set; }

    Dictionary<string, KeyValuePair<float, float>> stageProbability;

    public float TensionRate { get; private set; }
    public float MaxTensionRate { get; private set; }

    [SerializeField] Transform handle;

    [SerializeField] Transform rotatePoint;

    bool isRealling = false;

    [SerializeField] int damage;
    float lastDamageTime = 0f;

    [SerializeField] GameObject fishShadow;
    [SerializeField] GameObject rode;
    [SerializeField] Transform landinPoint;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        State = GameState.Preparation;
        SetStageProbability("UpStream");
    }

    private void Update()
    {
        switch (State)
        {
            case GameState.Preparation:
                break;
            case GameState.Waiting:
                break;
            case GameState.Hooking:
                if (Input.GetMouseButtonDown(0))
                {
                    Hook();
                }
                break;
            case GameState.Fighting:
                MoveRode();
                distance = Vector2.Distance(landinPoint.position, BiteFish.transform.position);
                fightingTime += Time.deltaTime;

                //릴링중
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

                //텐션과다
                if (TensionRate > MaxTensionRate + 0.2f)
                {
                    FisnishFighting();
                    State = GameState.Preparation;
                }

                //1초 경과마다 데미지 주기
                if (lastDamageTime + 1 < fightingTime)
                {
                    lastDamageTime = Mathf.FloorToInt(fightingTime);
                    DamageFish();
                }

                //거리가 가까워지면 물고기 잡기
                if (distance <= 0.1f)
                {
                    FisnishFighting();
                    State = GameState.Result;
                }
                break;
            case GameState.Result:
                break;
        }
    }

    //파이팅 종료
    private void FisnishFighting()
    {
        lastDamageTime = 0f;
        fightingTime = 0f;
        TensionRate = 0f;
        if(BiteFish != null)
        {
            Destroy(BiteFish.gameObject);
            //BiteFish = null;
        }
        rode.transform.rotation = Quaternion.Euler(Vector3.zero);
    }

    //케스팅
    public void Casting()
    {
        if (State != GameState.Preparation)
            return;
        thrownFloat = Instantiate(fishingFloat);
        thrownFloat.transform.position = floatPosition.position;
        State = GameState.Hooking;
        ZoomInFloat();
    }

    //찌 줌인
    private void ZoomInFloat()
    {
        Camera.main.transform.DOLocalMove(new Vector3(floatPosition.transform.position.x, floatPosition.transform.position.y,
            Camera.main.transform.position.z), 1f).OnComplete(GetBite);
        Camera.main.DOOrthoSize(3, 1f);
    }

    //찌 줌아웃
    private void ZoomOutFloat()
    {
        Camera.main.transform.DOLocalMove(new Vector3(0, 0,
              Camera.main.transform.position.z), 1f);
        Camera.main.DOOrthoSize(10, 1f);
    }

    //물고기가 미끼 물었을때
    private void GetBite()
    {
        //최대 내려가는거 2
        sequence = DOTween.Sequence();
        sequence.Append(thrownFloat.transform.DOLocalMoveY(thrownFloat.transform.position.y - 1, 0.5f));
        sequence.Append(thrownFloat.transform.DOLocalMoveY(thrownFloat.transform.position.y, 0.5f));
        sequence.Append(thrownFloat.transform.DOLocalMoveY(thrownFloat.transform.position.y - 1, 0.5f));
        sequence.Append(thrownFloat.transform.DOLocalMoveY(thrownFloat.transform.position.y, 0.5f));
        sequence.Append(thrownFloat.transform.DOLocalMoveY(thrownFloat.transform.position.y - 2, 0.5f));
        sequence.OnComplete(HookingFail);
        sequence.Play();

        GetRandomFish();
    }

    //확률에 따른 물고기 배정
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

    //스테이지 확률 테이블 세팅
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

    //후킹 시도
    public void Hook()
    {
        sequence.Pause();
        sequence.Kill();
        float hookPoint = floatPosition.position.y - thrownFloat.transform.position.y;
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
            HookingFail();
        }
    }

    //후킹 실패
    private void HookingFail()
    {
        ZoomOutFloat();
        sequence.Kill();
        DestroyImmediate(thrownFloat, true);
        FisnishFighting();
        State = GameState.Preparation;
    }

    //후킹 성공
    private void HookingSuccess(int level)
    {
        State = GameState.Fighting;
        switch (level)
        {
            case 0: //perfect
                SetMaxTensionRate(1f);
                break;
            case 1: //great
                SetMaxTensionRate(0.8f);
                break;
            case 2: //good
                SetMaxTensionRate(0.6f);
                break;
        }
        ZoomOutFloat();
        Destroy(thrownFloat);
        //Debug.Log(BiteFish.Size);
        fightingTime = 0f;
        State = GameState.Fighting;
    }

    //최대 텐션 설정
    private void SetMaxTensionRate(float maxTension)
    {
        this.MaxTensionRate = maxTension;
    }

    //릴링 시작
    public void RealingStart()
    {
        isRealling = true;
    }

    //릴링 중단
    public void RealingEnd()
    {
        isRealling = false;
    }
    
    //릴링
    private void Realling (){
        TensionRate += 1f * Time.deltaTime;
        MoveHandle();
        BiteFish.Move(landinPoint.position);
    }

    //릴링중이 아닐때
    private void NotRealing()
    {
        if (TensionRate > 0f) {
            TensionRate -= 0.5f * Time.deltaTime;
        }
    }

    private void MoveHandle()
    {
        //Debug.Log("MoveHandle");
        handle.RotateAround(rotatePoint.position, Vector3.back, 10f);
    }

    //물고기에게 데미지
    private void DamageFish()
    {
        BiteFish.GetDamage(Mathf.FloorToInt(TensionRate * damage));
    }

    //Rode 움직임
    private void MoveRode()
    {
        Vector2 dir = new Vector2(BiteFish.transform.position.x - landinPoint.transform.position.x,
            BiteFish.transform.position.y - landinPoint.transform.position.y);
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        rode.transform.rotation = Quaternion.AngleAxis(angle - 90, new Vector3(0,0,1));
    }

    public void TryStunSnap(Vector2 direction)
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

    public void CloseResultPannel ()
    {
        State = GameState.Preparation; 
        fightingTime = 0f;
        State = GameState.Preparation;
        if (BiteFish != null)
        {
            Destroy(BiteFish.gameObject);
            BiteFish = null;
        }
    }
}
