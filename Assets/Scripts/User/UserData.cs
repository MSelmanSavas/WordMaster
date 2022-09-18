using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class UserData
{
    public GameLanguages Language;
    public List<UserDataPerLength> UserDataEntries;
}

[System.Serializable]
public class UserDataPerLength
{
    public int WordLength = 0;

    [SerializeField]
    public UserDataInfo DailyInfo;

    [SerializeField]
    public UserDataInfo NormalInfo;
}

[System.Serializable]
public class UserDataInfo
{
    public List<WordData> PlayedWords = new List<WordData>();
    public int PlayedSessions = 0;
    public int FailedSessions = 0;
    public int CurrentStreak = 0;
    public int BestStreak = 0;
    public List<int> GuessDistribution = new List<int>();

    public void IncreaseGuessDistributon(int GuessAmount)
    {
        GuessDistribution[GuessAmount]++;
    }
}
