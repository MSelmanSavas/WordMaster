using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GameStateChangeActionsBase : MonoBehaviour
{
    [SerializeField]
    protected GameState StateToActivate;

    private void OnEnable()
    {
        GameStateManager.Instance.OnGameStateChange += OnStateChanged;
    }

    private void OnDisable()
    {
        GameStateManager.Instance.OnGameStateChange -= OnStateChanged;
    }

    protected abstract void OnStateChanged(GameState state);
}
