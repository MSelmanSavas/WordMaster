using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelStartButton : MonoBehaviour
{
    [SerializeField]
    LevelData LevelData;

    public void StartLevel()
    {
        SceneManager.Instance.LoadGameScene(LevelData);
    }
}
