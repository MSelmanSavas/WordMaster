using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShareLevel : MonoBehaviour
{
    string BlackSquareUnicode = "\U00002B1B";
    string OrangeSquareUnicode = "\U0001F7E7";
    string GreenSquareUnicode = "\U0001F7E9";

    public void ShareCurrentLevel()
    {
        string shareString = "";


        GamefieldData CurrentGameFieldData = GameManager.Instance.GetCurrentGamefieldData();

        switch (LevelDataCarrier.Instance.GetCurrentLevelData().Type)
        {
            case LevelType.Daily:
                {
                    shareString = $"WordMaster'da bugünün kelimesini nasıl çözdüm baksana!";
                    shareString += " " + DailyWordsManager.Instance.GetDailyWordNumber();
                    break;
                }
            default:
                {
                    shareString = "WordMaster'da kelimeyi nasıl çözdüm baksana!";
                    break;
                }
        }

        if (GameStateManager.Instance.CurrentGamestate == GameState.Win)
            shareString += " " + (CurrentGameFieldData.CurrentTryGroup + 1) + "/" + CurrentGameFieldData.MaxTryAmount;
        else if (GameStateManager.Instance.CurrentGamestate == GameState.Lose)
            shareString += " X/" + CurrentGameFieldData.MaxTryAmount;

        shareString += "\n";

        for (int i = 0; i <= CurrentGameFieldData.CurrentTryGroup; i++)
        {
            for (int j = 0; j < CurrentGameFieldData.WordLength; j++)
            {

                switch (CurrentGameFieldData.Groups[i].GroupData[j].CurrentState)
                {
                    case LetterState.WrongLetter:
                        {
                            shareString += BlackSquareUnicode;
                            break;
                        }
                    case LetterState.FoundButWrongIndex:
                        {
                            shareString += OrangeSquareUnicode;
                            break;
                        }
                    case LetterState.FoundAndCorrectIndex:
                        {
                            shareString += GreenSquareUnicode;
                            break;
                        }
                }
            }

            shareString += "\n";
        }

        shareString += "Sende denemek istersen burdan indir! ->";

        new NativeShare().SetText(shareString).SetUrl("https://play.google.com/store/apps/details?id=com.MehmetSelmanSavas.WordMaster")
              .SetCallback((result, shareTarget) => Debug.Log("Share result: " + result + ", selected app: " + shareTarget))
              .Share();
    }

}
