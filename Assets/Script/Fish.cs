using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Data;
using UnityEngine.UI;
using DG.Tweening;

public class Fish : MonoBehaviour
{
    private string fishName;
    public string FishName { get { return fishName; } }

    private float size;
    public float Size { get { return size; } }
    int currentHP;
    int maxHP;
    public float HPRate { get { return (float)currentHP / (float)maxHP; } }

    [SerializeField] private GameObject damageText;

    public void SetFish(string fishName)
    {
        if (fishName == null)
        {
            Debug.LogError("잘못된 물고기명 입력");
            return;
        }
        this.fishName = fishName;
        this.size = Random.Range((from row in FishingData.FishDataTable.AsEnumerable()
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

    private void Update()
    {
        if (FishingManager.Instance.gameState == GameState.Fighting && currentHP > 0)
        {
            transform.Translate(new Vector3(0, 1, 0) * Time.deltaTime);
        }    
    }

    public void GetDamage(int damage)
    {
        currentHP -= damage;
        //Debug.Log("데미지 : " + damage);
        //Debug.Log("잔여 체력 : " + currentHP);
        Instantiate(damageText, transform.position, Quaternion.identity).GetComponent<DamagePopUp>().SetDamage(damage);
    }

    public void HealFish()
    {
        if(currentHP < maxHP)
        {
            
        }
    }

    public void Move(Vector3 target)
    {
        transform.Translate((target - transform.position).normalized * Time.deltaTime);
    }
}
