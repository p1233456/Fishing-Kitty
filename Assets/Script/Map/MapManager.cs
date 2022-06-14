using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MapManager : MonoBehaviour
{
    public void LoadTitle()
    {
        SceneManager.LoadScene("Title");
    }

    //스테이지로 이동
    public void LoadStage(string stageName)
    {
        if (CheckCanMoveStage(stageName))
        {
            SceneManager.LoadScene(stageName);
        }
    }

    //이동 가능한 스테이지인지 체크
    private bool CheckCanMoveStage(string stageName)
    {
        return true;
    }
}
