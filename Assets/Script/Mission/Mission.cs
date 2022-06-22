using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum RewardType
{
    Money
}

public class Mission
{
    public string TargetName { get; set;}   //대성어종
    public int GoalCount { get; set;}   //목표마릿수
    public int CurrentCount  {get; set;}
    public string[] Condition{ get; set; }  //
    public int RewardValue {get; set;}   //보상

    public void Initialize(string targetName)
    {
        TargetName = targetName;
        CurrentCount = 0;
        GoalCount = 3;
        RewardValue = 100;
    }
}
