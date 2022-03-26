using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Data;

public class Fish
{    
    public string FishName { get; }
    public float Size { get; }
    int currentHP;
    int maxHP;
    public float HPRate { get { return (float)currentHP / (float)maxHP; } }

    public Fish(string fishName)
    {
        if (fishName == null)
        {
            Debug.LogError("잘못된 물고기명 입력");
            return;
        }
        this.FishName = fishName;
        Size = Random.Range((from row in FishingData.FishDataTable.AsEnumerable()
                             where (string)row["Name"] == fishName
                             select row.Field<float>("MinSize")).ElementAt<float>(0),
                             (from row in FishingData.FishDataTable.AsEnumerable()
                              where (string)row["Name"] == fishName
                              select row.Field<float>("MaxSize")).ElementAt<float>(0));
        maxHP = (from row in FishingData.FishDataTable.AsEnumerable()
                 where (string)row["Name"] == fishName
                 select row.Field<int>("MaxHP")).ElementAt<int>(0);
    }

    private void Start()
    {
        currentHP = maxHP;
    }

    public void GetDamage(int damage)
    {
        currentHP -= damage;
    }
}
