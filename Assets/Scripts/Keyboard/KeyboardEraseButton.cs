using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyboardEraseButton : MonoBehaviour
{
    [SerializeField]
    bool canPress = true;

    [SerializeField]
    UnityEngine.UI.Button Button;

    private void Start()
    {
        GameStateManager.Instance.OnGameStateChange += SetCanPressBasedOnGameState;
        Button.onClick.AddListener(() => PressButton());
    }

    private void PressButton()
    {
        if (canPress)
        {
            AudioManager.Instance.PlayKeyboardClickSound();
            GameManager.Instance.EraseLastEnteredCharacter();
        }
    }

    private void SetCanPressBasedOnGameState(GameState state)
    {
        canPress = state == GameState.Play;
        Button.interactable = state == GameState.Play || state == GameState.Pause;
    }

}
