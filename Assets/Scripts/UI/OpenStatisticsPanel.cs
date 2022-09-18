using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenStatisticsPanel : MonoBehaviour
{
    public void OpenPanel()
    {
        WindowManager.Instance.OpenStatisticsPanel();
        GameStateManager.Instance.CurrentGamestate = GameState.Pause;
    }
}
