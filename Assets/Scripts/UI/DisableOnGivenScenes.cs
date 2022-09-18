using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableOnGivenScenes : MonoBehaviour
{
    [SerializeField]
    List<string> SceneNamesToDisable = new List<string>();

    private void OnEnable()
    {
        string currentSceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        if (SceneNamesToDisable.Contains(currentSceneName))
            gameObject.SetActive(false);
    }
}
