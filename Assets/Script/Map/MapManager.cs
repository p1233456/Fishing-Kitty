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

    //���������� �̵�
    public void LoadStage(string stageName)
    {
        if (CheckCanMoveStage(stageName))
        {
            SceneManager.LoadScene(stageName);
        }
    }

    //�̵� ������ ������������ üũ
    private bool CheckCanMoveStage(string stageName)
    {
        return true;
    }
}
