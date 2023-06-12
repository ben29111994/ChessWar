using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    private void Awake()
    {
        instance = this;
    }

    public GameObject inGameUI;
    public GameObject completeUI;
    public GameObject failUI;
    public GameObject title;
    public GameObject playBtn;

    public void OnReplay()
    {
        Show_InGameUI();
        GameManager.instance.GenerateLevel();
    }

    public void OnReset()
    {
        GameManager.instance.ResetLevel();
    }
    public void Continue()
    {
        Show_InGameUI();
        GameManager.instance.GenerateLevel();
    }

    public void OnNext()
    {
    //    GameManager.instance.levelGame++;
     //   Show_InGameUI();
        GameManager.instance.Complete();
    }

    public void OnPrev()
    {
        GameManager.instance.levelGame--;
        Show_InGameUI();
        GameManager.instance.GenerateLevel();
    }

    public void Show_InGameUI()
    {
        inGameUI.SetActive(true);
        completeUI.SetActive(false);
        failUI.SetActive(false);
    }

    public void OnPlayBtn()
    {
        title.SetActive(false);
        playBtn.SetActive(false);
    }

    public void Show_CompleteUI()
    {
         completeUI.SetActive(true);
    }
    public void Show_FailUI()
    {
        failUI.SetActive(true);
    }
}
