using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Globalization;

public class DailyWordsManager : MonoBehaviour
{
    public static DailyWordsManager Instance;

    [SerializeField]
    bool isCorrectlyInitialized = false;

    [SerializeField]
    string DailyWordsStartDateString;
    DateTime DailyWordsStartDate;

    [SerializeField]
    List<LastSolvedDailyWordDatesByLength> LastSolvedDates = new List<LastSolvedDailyWordDatesByLength>();

    [SerializeField]
    DateTime RealTodaysTime;

    [SerializeField]
    public Action OnDailyWordsManagerInitialize;

    string SolvedDateDataKey = "LastDateSolved";


    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        isCorrectlyInitialized = false;
        DailyWordsStartDate = DateTime.Parse(DailyWordsStartDateString, System.Globalization.CultureInfo.InvariantCulture);
        Debug.LogError(DailyWordsStartDate.ToString(System.Globalization.CultureInfo.InvariantCulture));
        LoadLastSolvedDates();
    }


    private void OnEnable()
    {
        NetConnectionManager.Instance.OnInternetConnectionEstabilished += OnDownloadedRealDate;
        NetConnectionManager.Instance.OnInternetConnectionLost += OnDownloadedDataLost;
    }

    private void OnDisable()
    {
        NetConnectionManager.Instance.OnInternetConnectionEstabilished -= OnDownloadedRealDate;
        NetConnectionManager.Instance.OnInternetConnectionLost -= OnDownloadedDataLost;
    }

    void LoadLastSolvedDates()
    {

        for (int i = 0; i < LastSolvedDates.Count; i++)
        {
            if (!PlayerPrefs.HasKey(SolvedDateDataKey + LastSolvedDates[i].WordLength))
            {
                DateTime Yesterday = DateTime.Today.AddDays(-1);
                PlayerPrefs.SetString(SolvedDateDataKey + LastSolvedDates[i].WordLength, Yesterday.ToString(System.Globalization.CultureInfo.InvariantCulture));
            }

            LastSolvedDates[i].LastSolvedDate = DateTime.Parse(PlayerPrefs.GetString(SolvedDateDataKey + LastSolvedDates[i].WordLength), System.Globalization.CultureInfo.InvariantCulture);
        }
    }

    void OnDownloadedRealDate()
    {
        isCorrectlyInitialized = true;
        RealTodaysTime = NetConnectionManager.Instance.realLocalDateTime;
        OnDailyWordsManagerInitialize?.Invoke();
        Debug.LogError(GetDailyWordNumber());
    }

    void OnDownloadedDataLost()
    {
        isCorrectlyInitialized = false;
    }

    public int GetTodaysDailyWordIndex()
    {
        return DateTime.Now.Date.Subtract(DailyWordsStartDate.Date).Days - 1;
    }

    public int GetDailyWordNumber()
    {
        if (!isCorrectlyInitialized)
            return -1;

        return RealTodaysTime.Date.Subtract(DailyWordsStartDate.Date).Days;
    }
    public bool GetCanSolveTodaysDailyWordByLength(int WordLength)
    {
        if (!isCorrectlyInitialized)
            return false;

        TimeSpan PassedTime = RealTodaysTime.Date.Subtract(LastSolvedDates.Where(x => x.WordLength == WordLength).First().LastSolvedDate.Date);

        if (PassedTime.Days < 1)
            return false;

        return true;
    }

    public void SetTodaysDailyWordDate(int WordLength)
    {
        LastSolvedDates.Where(x => x.WordLength == WordLength).First().LastSolvedDate = DateTime.Now;
        PlayerPrefs.SetString(SolvedDateDataKey + LastSolvedDates.Where(x => x.WordLength == WordLength).First().WordLength, DateTime.Now.ToString(System.Globalization.CultureInfo.InvariantCulture));
    }
}

[System.Serializable]
public class LastSolvedDailyWordDatesByLength
{
    public int WordLength;
    public DateTime LastSolvedDate;
}
