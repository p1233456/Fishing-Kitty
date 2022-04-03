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
    //[SerializeField] float speed = 1f;
    [SerializeField] Sprite leftSprite;
    [SerializeField] Sprite rightSprite;
    private SpriteRenderer spriteRenderer;

    public void SetFish(string fishName)
    {
        if (fishName == null)
        {
            Debug.LogError("잘못된 물고기명 입력");
            return;
        }
        this.fishName = fishName;
        this.size = Mathf.Round(Random.Range((from row in FishingData.FishDataTable.AsEnumerable()
                                              where (string)row["Name"] == fishName
                                              select row.Field<float>("MinSize")).ElementAt<float>(0),
                             (from row in FishingData.FishDataTable.AsEnumerable()
                              where (string)row["Name"] == fishName
                              select row.Field<float>("MaxSize")).ElementAt<float>(0)) * 100f) / 100f;
        maxHP = (from row in FishingData.FishDataTable.AsEnumerable()
                 where (string)row["Name"] == fishName
                 select row.Field<int>("MaxHP")).ElementAt<int>(0);
    }

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        currentHP = maxHP;
        StartCoroutine(SetRandDirection());
    }

    private void Update()
    {
        if (FishingManager.Instance.State == GameState.Fighting && currentHP > 0)
        {
            Debug.Log((randDirection));
        }
        else
        {
            randDirection = Vector2.zero;
        }

        //가는방향 바라보기
        if(randDirection.normalized.x > 0)
        {
            spriteRenderer.sprite = rightSprite;
        }
        else
        {
            spriteRenderer.sprite = leftSprite;
        }
        transform.Translate((direction.normalized + randDirection.normalized).normalized * Time.deltaTime);
        direction = Vector2.zero;
    }

    IEnumerator SetRandDirection()
    {
        while (currentHP > 0)
        {
            randDirection = new Vector2(Mathf.FloorToInt(Random.Range(0, 2) == 1 ? 1 : -1), 1);
            yield return new WaitForSeconds(Random.Range(0.5f, 1.5f));
        }
    }

    public void GetDamage(int damage)
    {
        if (currentHP > 0)
        {
            currentHP -= damage;
            //Debug.Log("데미지 : " + damage);
            Instantiate(damageText, transform.position, Quaternion.identity).GetComponent<DamagePopUp>().SetDamage(damage);
        }
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
