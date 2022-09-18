using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameStateManager : MonoBehaviour
{
    public static GameStateManager Instance;

    GameState _currentGameState;

    [Sirenix.OdinInspector.ShowInInspector]
    public GameState CurrentGamestate
    {
        get
        {
            return _currentGameState;
        }
        set
        {
            if (_currentGameState == value)
            {
                return;
            }

            _currentGameState = value;
            OnGameStateChange?.Invoke(_currentGameState);
        }
    }

    public UnityAction<GameState> OnGameStateChange;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(this);
            return;
        }

        Instance = this;
    }

    private void Start()
    {

        StartCoroutine(StartActionsCor());
    }

    IEnumerator StartActionsCor()
    {
        CurrentGamestate = GameState.Loading;

        GameManager.Instance.LoadLevelData();
        GameManager.Instance.StartGamefieldConstruction();

        yield return new WaitForEndOfFrame();

        GameManager.Instance.RemoveLayoutGroupsFromGamefieldForAnimations();

        CurrentGamestate = GameState.Play;

        yield break;
    }
}

public enum GameState
{
    Loading,
    Pause,
    Play,
    Win,
    Lose
}
