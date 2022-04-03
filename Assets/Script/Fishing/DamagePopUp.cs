using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class DamagePopUp : MonoBehaviour
{
    [SerializeField] float moveSpeed;
    private TextMeshPro meshPro;

    private void Awake()
    {
        meshPro = transform.GetComponent<TextMeshPro>();
    }

    void Start()
    {
        Invoke("DestoryThis", 0.5f);
    }

    private void DestoryThis()
    {
        Destroy(this.gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(new Vector3(0, moveSpeed));
    }

    public void SetDamage(int damage)
    {
        meshPro.text = damage.ToString();
    }
}
