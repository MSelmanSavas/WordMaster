using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnLoseState : GameStateChangeActionsBase
{
    protected override void OnStateChanged(GameState state)
    {
        if (state != this.StateToActivate)
            return;


        UserDataManager.Instance.UpdateUserDataForGivenLength(
         LevelDataCarrier.Instance.GetCurrentLevelData().Type,
         GameManager.Instance.GetCurrentWord(),
         StateToActivate,
         GameManager.Instance.GetCurrentGamefieldData().CurrentTryGroup);

        if (LevelDataCarrier.Instance.GetCurrentLevelData().Type == LevelType.Daily)
        {
            DailyWordsManager.Instance.SetTodaysDailyWordDate(LevelDataCarrier.Instance.GetCurrentLevelData().WordLength);
        }

        AudioManager.Instance.PlayLoseTheme();
        WindowManager.Instance.CheckGameStateAndInstantiateRelevantPopup(state);
    }
}
