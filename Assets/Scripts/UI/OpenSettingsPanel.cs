using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenSettingsPanel : MonoBehaviour
{
    public void OpenPanel()
    {
        WindowManager.Instance.OpenSettingsPopup();

        if (GameStateManager.Instance)
            GameStateManager.Instance.CurrentGamestate = GameState.Pause;
    }
}
