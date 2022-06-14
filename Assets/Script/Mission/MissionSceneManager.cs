using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Data;


public class Mission
{
    public string targetName;
    public MissionType missionType;
    public float missionGoal;
    public RewardType reward;
    public float rewardValue;
}

public enum MissionType
{
    Size,
    Count
}

public enum RewardType
{
    Money
}

public class MissionSceneManager : MonoBehaviour
{
    [SerializeField] Text leftTimeText; //초기화까지 남은시간 표시 텍스트
    [SerializeField] Text resetOpportunityText; //초기화 가능 횟수 표시 텍스트
    const int opportunityRimit = 3;
    int resetOpportunity;

    private void Start()
    {
        resetOpportunity = 3;
    }

    private void Update()
    {
        resetOpportunityText.text = resetOpportunity.ToString();
    }

    public void AddOportunity()
    {
        //이전 방문시기와 비교했을때 시간이 지나있으면 그 시간 차이만큼 기회 추가
        //현재 시간을 방문시기로 초기화
    }

    public void ResetMissionPanel(MissionPanel missionPanel)
    {//리셋 기회가 남아있는경우
        if (resetOpportunity >= 1)
        {
            Mission mission = new Mission();
            List<string> fishNames = new List<string>();
            foreach (var row in FishingData.FishDataTable.AsEnumerable())
            {
                fishNames.Add(row.Field<string>("Name"));
            }
            mission.targetName = fishNames[Random.Range(0, fishNames.Count)];

            //추후 변경
            mission.missionType = MissionType.Count;
            mission.missionGoal = 10;
            mission.reward = RewardType.Money;
            mission.rewardValue = 100;
            missionPanel.SetMission(mission);
        }
    }
}
