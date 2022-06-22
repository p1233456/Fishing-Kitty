using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LibraryManager : MonoBehaviour
{
    public static LibraryManager Instance {get; private set;}
    
    [SerializeField] ViewDetail detail;

    public ViewDetail Detail{ get {return detail;} }
    
    private void Awake() {
        if(LibraryManager.Instance == null) 
        {
            LibraryManager.Instance = this;
        }
        else 
        {
            Destroy(gameObject);
        }
    }

    private void Start() {
        detail.gameObject.SetActive(false);
    }

    public void View(string fishName)
    {
        Detail.gameObject.SetActive(true);
        Detail.SetData(fishName);
    }

    public void Cancle()
    {
        Detail.gameObject.SetActive(false);
    }

    public void ToMain()
    {
        SceneManager.LoadScene("Title");
    }

    private void Update() {
        #if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.Q))
        {
            ToMain();
        }
        #endif
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            ToMain();
        }
    }
}
