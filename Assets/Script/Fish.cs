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

    private Vector2 direction; 
    Vector2 randDirection;
     [SerializeField] float speed = 1f;

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
        if (FishingManager.Instance.State == GameState.Fighting && currentHP > 0)
        {
            Debug.Log((direction.normalized + randDirection.normalized).normalized);
            randDirection = new Vector2(Random.Range(-1, 1), 1);
        }
        else
        {
            randDirection = Vector2.zero;
        }
        transform.Translate((direction.normalized + randDirection.normalized).normalized * Time.deltaTime);
        direction = Vector2.zero;
    }

    public void GetDamage(int damage)
    {
        currentHP -= damage;
        Debug.Log("데미지 : " + damage);
        Instantiate(damageText, transform.position, Quaternion.identity).GetComponent<DamagePopUp>().SetDamage(damage);
    }

    public void HealFish()
    {
        if(currentHP < maxHP)
        {
            
        }
    }

    public void Move(Vector2 target)
    {
        direction = target - new Vector2(transform.position.x, transform.position.y);
    }
}
