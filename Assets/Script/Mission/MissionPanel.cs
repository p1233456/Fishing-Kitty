using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MissionPanel : MonoBehaviour
{
    Mission mission;
    [SerializeField] Text targetNameText;
    [SerializeField] Image targetImage;
    [SerializeField] Text rewardText;
    [SerializeField] Text missionExplanationText;

    private void Update()
    {
        UpdateMissionData();
    }

    private void UpdateMissionData()
    {
        if (mission == null)
            return;
        Debug.Log(mission.targetName);
        targetNameText.text = mission.targetName;
        Sprite test = Resources.Load<Sprite>("FishSprites/" + mission.targetName);
        targetImage.sprite = test;
        switch (mission.missionType)
        {
            case MissionType.Count:
                missionExplanationText.text = mission.targetName + " : " + mission.missionGoal;
                break;
            case MissionType.Size:
                missionExplanationText.text = mission.targetName + " : " + mission.missionGoal;
                break;
        }

        switch (mission.reward)
        {
            case RewardType.Money:
                rewardText.text = (int)mission.rewardValue + "골드";
                break;
        }
    }

    public void SetMission(Mission mission)
    {
        this.mission = mission;
        Debug.Log(mission.targetName);
    }
}
