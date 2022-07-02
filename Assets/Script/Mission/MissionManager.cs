using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionManager : MonoBehaviour
{
    public static MissionManager Instance { get; private set; }
    
    private Mission[] assignedMissions = new Mission[2];
    public Mission[] AssignedMissions { get { return assignedMissions;}}

    [SerializeField] int tmp = 0;

    private void Awake() {
        if(MissionManager.Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start() {
        MissionEmptyCheck();
        MissionCompleteCheck();        
    }

    private void Update() {
        MissionEmptyCheck();
        MissionCompleteCheck();
    }

    public void MissionFishCheck(string fishName)
    {
        Debug.Log(fishName);
        for(int i = 0; i < assignedMissions.Length; i++)
        {
             if(assignedMissions[i].TargetName == fishName)
            {
                assignedMissions[i].CurrentCount += 1;
                FishingUIManager.Instance.ShowStamp();
            }
        }
    }

    public void MissionCompleteCheck()
    {
        for(int i = 0; i < assignedMissions.Length; i++)
        {
            if(assignedMissions[i].CurrentCount >= assignedMissions[i].GoalCount)
            {
                GetReward(assignedMissions[i]);
                assignedMissions[i] = AssignNewMission();
            }
        }
    }

    public void MissionEmptyCheck()
    {
        for(int i = 0; i < assignedMissions.Length; i++)
        {
            if(AssignedMissions[i] == null)
            {
                AssignedMissions[i] = AssignNewMission();
            }
        }
    }

    private Mission AssignNewMission()
    {
        Mission mission = new Mission();
        string fish = FishingData.UpStream.DefaultView.ToTable(false, "Name").Rows[Random.Range(0,FishingData.UpStream.Columns.Count)][0].ToString();
        mission.Initialize(fish);
        return mission;
    }

    private void GetReward(Mission mission)
    {
        PlayerPrefs.SetInt("Money", mission.RewardValue);
        return;
    }
}
