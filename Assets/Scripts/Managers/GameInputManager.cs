using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameInputManager : MonoBehaviour
{
    private void Update()
    {
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.W))
        {
            GameStateManager.Instance.CurrentGamestate = GameState.Win;
        }
        else if (Input.GetKeyDown(KeyCode.L))
        {
            GameStateManager.Instance.CurrentGamestate = GameState.Lose;
        }
#endif
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            GameStateManager.Instance.CurrentGamestate = GameState.Pause;
            WindowManager.Instance.OpenEndGamePopup();
        }
    }
}
