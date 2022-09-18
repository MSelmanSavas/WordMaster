using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;

public class StatisticsPanelInitializer : MonoBehaviour
{
    [SerializeField]
    int CurrentWordLength;

    [SerializeField]
    LevelType CurrentLevelType;

    [SerializeField]
    UserDataPerLength CurrentLevelTypeUserData;

    [SerializeField]
    TextMeshProUGUI StatisticsHeader;

    [SerializeField]
    TextMeshProUGUI LetterCount;

    [SerializeField]
    TextMeshProUGUI TotalPlayed;

    [SerializeField]
    TextMeshProUGUI WinPercentage;

    [SerializeField]
    TextMeshProUGUI CurrentStreak;

    [SerializeField]
    TextMeshProUGUI BestStreak;

    [SerializeField]
    Transform GuessDistrubionParent;

    [SerializeField]
    GameObject GuessDistributionBarPrefab;

    private void Start()
    {
        CurrentWordLength = LevelDataCarrier.Instance.GetCurrentLevelData().WordLength;
        CurrentLevelType = LevelDataCarrier.Instance.GetCurrentLevelData().Type;

        CurrentLevelTypeUserData = UserDataManager.Instance.GetUserDataEntryForLength(CurrentWordLength);

        StatisticsHeader.text = "İSTATİSTİK";
        LetterCount.text = CurrentWordLength + " HARFLİ";

        UserDataInfo CurrentLevelTypeInfo = null;

        switch (CurrentLevelType)
        {
            case LevelType.Daily:
                {
                    CurrentLevelTypeInfo = CurrentLevelTypeUserData.DailyInfo;
                    break;
                }
            default:
                {
                    CurrentLevelTypeInfo = CurrentLevelTypeUserData.NormalInfo;
                    break;
                }
        }

        TotalPlayed.text = CurrentLevelTypeInfo.PlayedSessions.ToString();
        float WinPercentageValue = (float)(CurrentLevelTypeInfo.PlayedSessions - CurrentLevelTypeInfo.FailedSessions) / (float)CurrentLevelTypeInfo.PlayedSessions;
        WinPercentageValue *= 100f;
        WinPercentage.text = Mathf.RoundToInt(Mathf.Clamp(WinPercentageValue, 0f, WinPercentageValue)).ToString();
        CurrentStreak.text = CurrentLevelTypeInfo.CurrentStreak.ToString();
        BestStreak.text = CurrentLevelTypeInfo.BestStreak.ToString();

        float GuessDistributionMinHeight = 75f;
        float GuessDistributionMaxHeight = GuessDistrubionParent.GetComponent<RectTransform>().rect.height;


        List<GuessDistributionsWithObject> GuessDistributionObjects = new List<GuessDistributionsWithObject>();

        int MaxGuessAmount = 0;
        int MinGuessAmount = 0;

        for (int i = 0; i < CurrentLevelTypeInfo.GuessDistribution.Count; i++)
        {
            GuessDistributionObjects.Add(new GuessDistributionsWithObject
            (CurrentLevelTypeInfo.GuessDistribution[i], Instantiate(GuessDistributionBarPrefab, Vector3.zero, Quaternion.identity, GuessDistrubionParent.transform)));

            if (CurrentLevelTypeInfo.GuessDistribution[i] > MaxGuessAmount)
                MaxGuessAmount = CurrentLevelTypeInfo.GuessDistribution[i];

            if (CurrentLevelTypeInfo.GuessDistribution[i] < MinGuessAmount)
                MinGuessAmount = CurrentLevelTypeInfo.GuessDistribution[i];
        }

        float StepSize = Mathf.Clamp((GuessDistributionMaxHeight - GuessDistributionMinHeight) / (MaxGuessAmount), 0, 1000000f);

        //GuessDistributionObjects = GuessDistributionObjects.OrderBy(x => x.GuessAmount).ToList();

        for (int i = 0; i < GuessDistributionObjects.Count; i++)
        {
            GuessDistributionObjects[i].GuessDistributionObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = GuessDistributionObjects[i].GuessAmount.ToString();

            GuessDistributionObjects[i].GuessDistributionObject.GetComponent<RectTransform>().sizeDelta = new Vector2(
                         GuessDistributionObjects[i].GuessDistributionObject.GetComponent<RectTransform>().rect.width,
                         GuessDistributionMinHeight + GuessDistributionObjects[i].GuessAmount * StepSize
                         );

            // if (GuessDistributionObjects[i].GuessAmount == 0)
            // {
            //     GuessDistributionObjects[i].GuessDistributionObject.GetComponent<RectTransform>().sizeDelta = new Vector2(
            //                   GuessDistributionObjects[i].GuessDistributionObject.GetComponent<RectTransform>().rect.width,
            //                   GuessDistributionMinHeight
            //               );
            //     continue;
            // }

            // GuessDistributionObjects[i].GuessDistributionObject.GetComponent<RectTransform>().sizeDelta = new Vector2(
            //     GuessDistributionObjects[i].GuessDistributionObject.GetComponent<RectTransform>().rect.width,
            //     GuessDistributionMinHeight + StepSize * i
            // );
        }
    }

    public class GuessDistributionsWithObject
    {
        public int GuessAmount;
        public GameObject GuessDistributionObject;

        public GuessDistributionsWithObject(int guessAmount, GameObject guessDistributionObject)
        {
            GuessAmount = guessAmount;
            GuessDistributionObject = guessDistributionObject;
        }
    }
}
