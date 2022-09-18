using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadGameScene : MonoBehaviour
{
    public void LoadGame()
    {
        SceneManager.Instance.ReloadGameScene();
    }
}
