using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnableButtonOnlyOnGivenGameState : MonoBehaviour
{
    [SerializeField]
    GameState stateToActive;

    [SerializeField]
    UnityEngine.UI.Button ButtonToActivate;

    private void Start()
    {
        OnGameStateChange(GameStateManager.Instance.CurrentGamestate);
    }

    private void OnEnable()
    {
        GameStateManager.Instance.OnGameStateChange += OnGameStateChange;
    }

    private void OnDisable()
    {
        GameStateManager.Instance.OnGameStateChange -= OnGameStateChange;
    }

    void OnGameStateChange(GameState state)
    {
        ButtonToActivate.interactable = state == stateToActive;
    }
}
