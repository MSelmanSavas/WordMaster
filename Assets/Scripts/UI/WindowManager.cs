using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindowManager : MonoBehaviour
{
    public static WindowManager Instance;

    [SerializeField]
    Canvas CurrentCanvas;

    [SerializeField]
    GameObject WinScreenPrefab;

    [SerializeField]
    GameObject LoseScreenPrefab;

    [SerializeField]
    GameObject UserStatisticsPrefab;

    [SerializeField]
    GameObject TutorialPrefab;

    [SerializeField]
    GameObject EndGamePopup;

    [SerializeField]
    GameObject SettingsPopupPrefab;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(this);
            return;
        }

        Instance = this;
    }

    private void OnEnable()
    {
        CurrentCanvas = FindObjectOfType<Canvas>();
    }

    public void CheckGameStateAndInstantiateRelevantPopup(GameState state)
    {
        switch (state)
        {
            case GameState.Win:
                {
                    Instantiate(WinScreenPrefab, Vector3.zero, Quaternion.identity, CurrentCanvas.transform);
                    break;
                }
            case GameState.Lose:
                {
                    Instantiate(LoseScreenPrefab, Vector3.zero, Quaternion.identity, CurrentCanvas.transform);
                    break;
                }
        }
    }

    public void OpenStatisticsPanel()
    {
        Instantiate(UserStatisticsPrefab, Vector3.zero, Quaternion.identity, CurrentCanvas.transform);
    }

    public void OpenTutorialPanel()
    {
        Instantiate(TutorialPrefab, Vector3.zero, Quaternion.identity, CurrentCanvas.transform);
    }

    public void OpenEndGamePopup()
    {
        Instantiate(EndGamePopup, Vector3.zero, Quaternion.identity, CurrentCanvas.transform);
    }

    public void OpenSettingsPopup()
    {
        Instantiate(SettingsPopupPrefab, Vector3.zero, Quaternion.identity, CurrentCanvas.transform);
    }
}
