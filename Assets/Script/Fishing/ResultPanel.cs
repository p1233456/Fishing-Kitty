using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResultPanel : MonoBehaviour
{
    [SerializeField] private Image image;
    [SerializeField] private Text fishName;
    [SerializeField] private Text length;
    [SerializeField] private Text freshMent;

    public void SetPanel(string fishName, float length, string freshMentClass)
    {
        Sprite test = Resources.Load<Sprite>("FishSprites/" + fishName);
        image.sprite = Resources.Load<Sprite>("FishSprites/" + fishName);
        this.fishName.text = fishName;
        this.length.text = length.ToString();
        this.freshMent.text = freshMentClass;
    }
}
