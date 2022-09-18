using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneManager : MonoBehaviour
{
    public static SceneManager Instance;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(this);
            return;
        }

        Instance = this;
    }

    public void LoadGameScene(LevelData data)
    {
        LevelDataCarrier.Instance.SetCurrentLevelData(data);
        UnityEngine.SceneManagement.SceneManager.LoadScene("Game");
    }

    public void ReloadGameScene()
    {
        LevelDataCarrier.Instance.SetCurrentLevelData(LevelDataCarrier.Instance.GetCurrentLevelData());
        UnityEngine.SceneManagement.SceneManager.LoadScene("Game");
    }
}
