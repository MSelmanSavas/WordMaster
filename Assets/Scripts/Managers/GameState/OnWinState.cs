using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnWinState : GameStateChangeActionsBase
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

        AudioManager.Instance.PlayWinTheme();
        WindowManager.Instance.CheckGameStateAndInstantiateRelevantPopup(state);
    }
}
