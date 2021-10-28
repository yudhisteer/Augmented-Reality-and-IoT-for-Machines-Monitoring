using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeARScript : MonoBehaviour
{
    public void livescene()
    {
        SceneManager.LoadScene("LiveScene");
    }

    public void dashscene()
    {
        SceneManager.LoadScene("DashScene");
    }

    public void exitgame()
    {
        Application.Quit();
        Debug.Log("Exit Button pressed");
    }
}
