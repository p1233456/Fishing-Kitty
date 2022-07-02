using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    [SerializeField] bool isQ = true;
    [SerializeField] string sceneName;

    private void Update() {
        if(gameObject.activeSelf == true)
        {
            if(Input.GetKeyDown(KeyCode.Q) && isQ)
            {
                SceneChange();
            }
        }
    }

    public void SceneChange()
    {
        SceneManager.LoadScene(sceneName);
    }
}
