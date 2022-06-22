using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System.Data;

public class ViewSimple : MonoBehaviour
{
    [SerializeField] Text fishName;
    [SerializeField] Image fishImage;
    [SerializeField] string selectedFish;

    private void Start() {
        SetData();
        ViewDetail();
    }

    public void SetData ()
    {
        fishName.text = selectedFish;
        fishImage.sprite = Resources.Load<Sprite>("FishSprites/" + selectedFish);
    }

    public void ViewDetail() 
    {
        LibraryManager.Instance.View(selectedFish);
    }
}
