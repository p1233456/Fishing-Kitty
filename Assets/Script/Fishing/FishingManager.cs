using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Data;
using System.Linq;
using System.Data;

public enum GameState
{
    Preparation,
    Waiting,
    Hooking,
    Fighting,
    Result,
    Fail
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
    [SerializeField] private GameObject[] fishingFloats;   //낚시 찌 프레펩
    [SerializeField] private Transform floatPosition;   //찌 위치
    private GameObject thrownFloat;                     //찌

    [SerializeField] private Transform rodePosition;
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
    [SerializeField] Rode rode;
    [SerializeField] Transform landinPoint;

    public string SelectedBite { get; set; }

    private float? previousAngle = null;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        State = GameState.Preparation;
        SelectedBite = "Bite 1";
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
                CameraControl();
                rode.Move(BiteFish.transform.position);
                distance = Vector3.Distance(landinPoint.position, BiteFish.transform.position);
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
                //Debug.Log(BiteFish.HPRate);
                if (TensionRate > MaxTensionRate + 0.2f && BiteFish.HPRate > 0)
                {
                    FailFishing();
                }
                //텐션과소
                if (TensionRate < -0.2f && BiteFish.HPRate > 0)
                {
                    FailFishing();
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
                    SuccessFishing();
                }
                break;
            case GameState.Result:
                break;
        }
    }

    private void FailFishing()
    {
        State = GameState.Fail;
        FishingUIManager.Instance.ViewRetryButton();
    }

    public void Retry()
    {
        HookingSuccess(0);
        lastDamageTime = 0f;
        fightingTime = 0f;
        TensionRate = 0.3f;
        State = GameState.Fighting;
        rode.transform.rotation = Quaternion.Euler(Vector3.zero);
    }

    public void DiscardRetry()
    {
        State = GameState.Preparation;
        FinishFighting();
    }

    private void SuccessFishing()
    {
        State = GameState.Result;
        MissionManager.Instance.MissionFishCheck(BiteFish.FishName);
        int price = (from row in FishingData.FishDataTable.AsEnumerable()
                                              where (string)row["Name"] == BiteFish.FishName
                                              select row.Field<int>("Price")).ElementAt<int>(0);
        FishingUIManager.Instance.SetPrice(price);
        FinishFighting();
    }

    //파이팅 종료
    public void FinishFighting()
    {
        lastDamageTime = 0f;
        fightingTime = 0f;
        TensionRate = 0f;
        if(BiteFish != null)
        {
            Destroy(BiteFish.gameObject);
            //BiteFish = null;
        }
        rode.transform.rotation = Quaternion.Euler(new Vector3(36,0,rode.transform.rotation.z));
    }

    //케스팅
    public void Casting()
    {
        if (State != GameState.Preparation)
            return;
        if (PlayerPrefs.GetInt(SelectedBite,0) < 1)
            return;
        UseBite(SelectedBite);
        thrownFloat = Instantiate(fishingFloats[Random.Range(0,4)]);
        thrownFloat.transform.position = floatPosition.position;
        State = GameState.Hooking;
        ZoomInFloat();
    }

    //찌 줌인
    private void ZoomInFloat()
    {
        Camera.main.DOFieldOfView(15, 1f).OnComplete(GetBite);
        Camera.main.transform.DORotate(new Vector3(13,0,0), 1f);
        Camera.main.DOOrthoSize(3, 1f);
    }

    //찌 줌아웃
    private void ZoomOutFloat()
    {
        Camera.main.DOFieldOfView(60, 1f);
        Camera.main.transform.DORotate(new Vector3(10,0,0), 1f);
        Camera.main.DOOrthoSize(10, 1f);
    }

    //물고기가 미끼 물었을때
    private void GetBite()
    {
        GetRandomFish();
    }

    //확률에 따른 물고기 배정
    private void GetRandomFish()
    {
        float random = Random.Range(0f, 1f);

        string fishName = null;

        foreach (var probability in stageProbability)
        {
            if (random >= probability.Value.Key && random < probability.Value.Value)
                fishName = probability.Key;
        }
        Debug.Log(fishName);
        BiteFish = Instantiate(fishShadow).GetComponent<Fish>();
        BiteFish.SetFish(fishName);
        BiteFish.transform.position = new Vector3(floatPosition.position.x, floatPosition.position.y - 2, floatPosition.position.z);
    }

    //스테이지 확률 테이블 세팅
    public void SetStageProbability(string stageName)
    {
        stageProbability = new Dictionary<string, KeyValuePair<float, float>>();
        float amount = 0;
        switch (stageName)
        {
            case "UpStream":
                foreach (var row in FishingData.UpStream.AsEnumerable())
                {
                    stageProbability.Add(row.Field<string>("Name"), new KeyValuePair<float, float>(amount, amount + row.Field<float>(SelectedBite)));
                    amount += row.Field<float>(SelectedBite);
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
        //sequence.Pause();
        //sequence.Kill();
        float hookPoint = floatPosition.position.y - thrownFloat.transform.position.y;
        if (hookPoint > 1.5f)
        {
            Debug.Log("perfect");
            HookingSuccess(0);
            FishingUIManager.Instance.ViewHookTiming("perfect");
        }
        else if (hookPoint > 1f)
        {
            Debug.Log("great");
            HookingSuccess(1);
            FishingUIManager.Instance.ViewHookTiming("great");
        }
        else if (hookPoint > 0.5f)
        {
            Debug.Log("good");
            HookingSuccess(2);
            FishingUIManager.Instance.ViewHookTiming("good");
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
        DestroyImmediate(thrownFloat, true);
        FinishFighting();
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
        TensionRate = 0.3f;
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
        TensionRate -= 0.5f * Time.deltaTime;
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

    public void TryStunSnap(Vector2 direction)
    {
        TensionRate -= 0.5f;
        if(direction.x > 0)
        {
            rode.StunLeft();
            if(BiteFish.transform.position.x < 0)
            {
                TensionRate += 0.2f;
                StunSnap(Mathf.Abs(BiteFish.transform.position.x));
            }
        }
        else
        {
            rode.StunRight();
            if (BiteFish.transform.position.x > 0)
            {
                TensionRate += 0.2f;
                StunSnap(Mathf.Abs(BiteFish.transform.position.x));
            }
        }
    }

    //스턴 시키기
    private void StunSnap(float power)
    {
        BiteFish.GetDamage(Mathf.FloorToInt(power));
        Camera.main.DOShakePosition(0.5f, 1);
        Debug.Log("스턴");
    }

    //결과창 닫기
    public void CloseResultPannel ()
    {
        FishingUIManager.Instance.HideStamp();
        int price = (from row in FishingData.FishDataTable.AsEnumerable()
                                              where (string)row["Name"] == BiteFish.FishName
                                              select row.Field<int>("Price")).ElementAt<int>(0);
        PlayerPrefs.SetInt("Money", PlayerPrefs.GetInt("Money", 0) + price);
        State = GameState.Preparation; 
        fightingTime = 0f;
        State = GameState.Preparation;
        if (BiteFish != null)
        {
            Destroy(BiteFish.gameObject);
            BiteFish = null;
        }
    }

    private void CameraControl() 
    {
        Vector2 dir = new Vector2(BiteFish.transform.position.x - Camera.main.transform.position.x,
            BiteFish.transform.position.z - Camera.main.transform.position.z);
        float angle = 90 - (Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg);
        float cameraAngle = Camera.main.transform.rotation.y * Mathf.Rad2Deg;
        //Debug.Log($"각 : {angle} 이동해야하는 각  : {angle - cameraAngle}");

        if(angle > 10 || angle < -10) 
        {
            return;
        }
        
        if(angle - cameraAngle > 0.1f)
        {
             Camera.main.transform.Rotate(new Vector3(0, 0.1f, 0));
        }
        else if(angle - cameraAngle < 0.1f)
        {
             Camera.main.transform.Rotate(new Vector3(0, - 0.1f, 0));
        }
        else if (Mathf.Abs(angle - cameraAngle) > 0.01f)
        {
            Camera.main.transform.Rotate(new Vector3(0, angle - cameraAngle, 0));
        }
    }

    public void UseBite(string bite)
    {
        PlayerPrefs.SetInt(bite, PlayerPrefs.GetInt(bite) - 1);
    }
}
