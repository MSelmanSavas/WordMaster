using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenTutorialWindow : MonoBehaviour
{
    private void OnEnable()
    {
        if (PlayerPrefs.HasKey("OpenedTutorial"))
            return;

        PlayerPrefs.SetInt("OpenedTutorial", 1);
        OpenPanel();
    }
    public void OpenPanel()
    {
        WindowManager.Instance.OpenTutorialPanel();
        GameStateManager.Instance.CurrentGamestate = GameState.Pause;
    }
}
