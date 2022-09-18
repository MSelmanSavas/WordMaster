using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenEndGamePanel : MonoBehaviour
{
    public void OpenPanel()
    {
        WindowManager.Instance.OpenEndGamePopup();
        GameStateManager.Instance.CurrentGamestate = GameState.Pause;
    }
}
