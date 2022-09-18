using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClosePopupAndSetPlay : MonoBehaviour
{
    public void ClosePanel(GameObject Panel)
    {
        if (GameStateManager.Instance)
            GameStateManager.Instance.CurrentGamestate = GameState.Play;

        Destroy(Panel);
    }
}
