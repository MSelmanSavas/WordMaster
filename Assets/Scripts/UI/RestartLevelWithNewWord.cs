using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RestartLevelWithNewWord : MonoBehaviour
{
    private void Start()
    {
        if (LevelDataCarrier.Instance.GetCurrentLevelData().Type == LevelType.Daily)
            gameObject.SetActive(false);
    }
    public void RestartLevelWithCurrentWordLength()
    {

        SceneManager.Instance.ReloadGameScene();
    }
}
