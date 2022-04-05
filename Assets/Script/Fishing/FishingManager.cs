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

    #region �ɽ��� ���� ����
    [SerializeField] private GameObject fishingFloat;   //���� �� ������
    [SerializeField] private Transform floatPosition;   //�� ��ġ
    private GameObject thrownFloat;                     //��
    private Sequence sequence;                          //��Ʈ�� 
    #endregion

    #region ��ŷ ���� ����
    #endregion

    #region ������ ���� ����
    float distance;
    [SerializeField] float fightingTime = 0f;  //��� �ð�
    #endregion

    #region ��� ���� ����
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

    public string SelectedBite { get; set; }

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
                MoveRode();
                distance = Vector2.Distance(landinPoint.position, BiteFish.transform.position);
                fightingTime += Time.deltaTime;

                //������
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

                //�ټǰ���
                //Debug.Log(BiteFish.HPRate);
                if (TensionRate > MaxTensionRate + 0.2f && BiteFish.HPRate > 0)
                {
                    FailFishing();
                }
                //�ټǰ���
                if (TensionRate < -0.2f && BiteFish.HPRate > 0)
                {
                    FailFishing();
                }

                //1�� ������� ������ �ֱ�
                if (lastDamageTime + 1 < fightingTime)
                {
                    lastDamageTime = Mathf.FloorToInt(fightingTime);
                    DamageFish();
                }

                //�Ÿ��� ��������� ����� ���
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

    public void DiscarRetry()
    {
        State = GameState.Preparation;
        FinishFighting();
    }

    private void SuccessFishing()
    {
        State = GameState.Result;
        FinishFighting();
    }

    //������ ����
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
        rode.transform.rotation = Quaternion.Euler(Vector3.zero);
    }

    //�ɽ���
    public void Casting()
    {
        if (State != GameState.Preparation)
            return;
        thrownFloat = Instantiate(fishingFloat);
        thrownFloat.transform.position = floatPosition.position;
        State = GameState.Hooking;
        ZoomInFloat();
    }

    //�� ����
    private void ZoomInFloat()
    {
        Camera.main.transform.DOLocalMove(new Vector3(floatPosition.transform.position.x, floatPosition.transform.position.y,
            Camera.main.transform.position.z), 1f).OnComplete(GetBite);
        Camera.main.DOOrthoSize(3, 1f);
    }

    //�� �ܾƿ�
    private void ZoomOutFloat()
    {
        Camera.main.transform.DOLocalMove(new Vector3(0, 0,
              Camera.main.transform.position.z), 1f);
        Camera.main.DOOrthoSize(10, 1f);
    }

    //����Ⱑ �̳� ��������
    private void GetBite()
    {
        //�ִ� �������°� 2
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

    //Ȯ���� ���� ����� ����
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
    }

    //�������� Ȯ�� ���̺� ����
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
                Debug.Log("�߸��� ���������� �Է�");
                break;
        }
    }

    //��ŷ �õ�
    public void Hook()
    {
        sequence.Pause();
        sequence.Kill();
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

    //��ŷ ����
    private void HookingFail()
    {
        ZoomOutFloat();
        sequence.Kill();
        DestroyImmediate(thrownFloat, true);
        FinishFighting();
        State = GameState.Preparation;
    }

    //��ŷ ����
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

    //�ִ� �ټ� ����
    private void SetMaxTensionRate(float maxTension)
    {
        this.MaxTensionRate = maxTension;
    }

    //���� ����
    public void RealingStart()
    {
        isRealling = true;
    }

    //���� �ߴ�
    public void RealingEnd()
    {
        isRealling = false;
    }
    
    //����
    private void Realling (){
        TensionRate += 1f * Time.deltaTime;
        MoveHandle();
        BiteFish.Move(landinPoint.position);
    }

    //�������� �ƴҶ�
    private void NotRealing()
    {
        TensionRate -= 0.5f * Time.deltaTime;
    }

    private void MoveHandle()
    {
        //Debug.Log("MoveHandle");
        handle.RotateAround(rotatePoint.position, Vector3.back, 10f);
    }

    //����⿡�� ������
    private void DamageFish()
    {
        BiteFish.GetDamage(Mathf.FloorToInt(TensionRate * damage));
    }

    //Rode ������
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

    //���� ��Ű��
    private void StunSnap(float power)
    {
        BiteFish.GetDamage(Mathf.FloorToInt(power));
        Debug.Log("����");
    }

    //���â �ݱ�
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
