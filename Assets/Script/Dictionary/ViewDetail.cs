using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System.Data;

public class ViewDetail : MonoBehaviour
{
    [SerializeField] Text fishName;
    [SerializeField] Image fishImage;
    [SerializeField] Text explanation;

    public void SetData (string selectedFish)
    {
        fishName.text = selectedFish;
        fishImage.sprite = Resources.Load<Sprite>("FishSprites/" + selectedFish);
        explanation.text = (from row in FishingData.FishDataTable.AsEnumerable()
                              where (string)row["Name"] == selectedFish
                              select row.Field<string>("Explanation")).ElementAt(0);
    }
}
