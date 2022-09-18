using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class DailyWordButton : MonoBehaviour
{
    [SerializeField]
    LevelData LevelData;

    [SerializeField]
    UnityEngine.UI.Button Button;

    [SerializeField]
    TMPro.TextMeshProUGUI DailyText;


    private void OnEnable()
    {
        DailyWordsManager.Instance.OnDailyWordsManagerInitialize += CheckDailyButtonStatus;
    }

    private void OnDisable()
    {
        DailyWordsManager.Instance.OnDailyWordsManagerInitialize -= CheckDailyButtonStatus;
    }

    private void Start()
    {
        StartCoroutine(CheckInternetConnection());
    }


    public void OpenDailyLevel()
    {
        SceneManager.Instance.LoadGameScene(LevelData);
    }

    IEnumerator CheckInternetConnection()
    {
        Button.interactable = false;
        if (!NetConnectionManager.Instance.isCorrectDateLoaded)
            DailyText.text = "Internete Bağlanın...";

        yield return new WaitForSeconds(0.1f);


        yield return new WaitUntil(() => NetConnectionManager.Instance.isCorrectDateLoaded);
        CheckDailyButtonStatus();
    }

    void CheckDailyButtonStatus()
    {
        if (DailyWordsManager.Instance.GetCanSolveTodaysDailyWordByLength(LevelData.WordLength))
        {
            DailyText.text = LevelData.WordLength + " LETTERS";
            Button.interactable = true;
            return;
        }

        Button.interactable = false;

        StartCoroutine(TimerCountDown());
    }

    IEnumerator TimerCountDown()
    {
        DateTime RealDateTime = NetConnectionManager.Instance.realLocalDateTime;
        DateTime Tomorrow = DateTime.Now.AddDays(1).Date;

        if (DateTime.Now.Date.Subtract(RealDateTime.Date).Days > 0)
        {
            DailyText.text = "Yarını bekleyin...";
            yield break;
        }

        while (true)
        {
            if (Tomorrow.Subtract(DateTime.Now).Seconds < 0)
                break;

            DailyText.text = Tomorrow.Subtract(DateTime.Now).ToString(@"hh\:mm\:ss");
            yield return new WaitForEndOfFrame();
        }

        DailyText.text = "Yeniden Başlatın...";
    }

}
