using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloseStatisticsPanel : MonoBehaviour
{
    public void ClosePanel(GameObject Panel)
    {
        GameStateManager.Instance.CurrentGamestate = GameState.Play;
        Destroy(Panel);
    }
}
