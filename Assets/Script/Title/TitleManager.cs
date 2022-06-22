using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleManager : MonoBehaviour
{
    public void LoadMapScene()
    {
        SceneManager.LoadScene("Map");
    }

    public void LoadDictionary()
    {
        SceneManager.LoadScene("Dictionary");
    }

    public void LoadSetting()
    {
        
    }
}
