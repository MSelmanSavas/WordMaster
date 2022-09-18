using System.Collections;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Experimental.SceneManagement;
#endif

using UnityEngine;

public class KeyboardButton : MonoBehaviour
{
    [SerializeField]
    char KeyCharacter;

    [SerializeField]
    LetterState CurrentState;

    [SerializeField]
    bool canPress = true;

    [SerializeField]
    TMPro.TextMeshProUGUI Text;

    [SerializeField]
    UnityEngine.UI.Button Button;

    [SerializeField]
    UnityEngine.UI.Image BackgroundImage;

    [SerializeField]
    Animator KeyboardButtonAnimator;

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (PrefabStageUtility.GetCurrentPrefabStage() != null)
        {
            bool isPrefabInstance = PrefabUtility.IsPartOfPrefabInstance(gameObject);
        }
        if (PrefabStageUtility.GetCurrentPrefabStage() == null)
            gameObject.name = "Key" + KeyCharacter;

        Text.text = KeyCharacter.ToString();
    }
#endif

    private void Start()
    {
        GameStateManager.Instance.OnGameStateChange += SetCanPressBasedOnGameState;
        Button.onClick.AddListener(() => PressButton());
    }

    public char GetKeyCharacter()
    {
        return KeyCharacter;
    }

    private void PressButton()
    {
        if (canPress)
        {
            GameManager.Instance.OnValidInput(KeyCharacter);
            AudioManager.Instance.PlayKeyboardClickSound();
        }
    }

    private void SetCanPressBasedOnGameState(GameState state)
    {
        canPress = state == GameState.Play;
        Button.interactable = state == GameState.Play || state == GameState.Pause;
    }

    public void SetKeyState(LetterState state)
    {
        if (CurrentState == state)
            return;
        switch (state)
        {
            case LetterState.Default:
                {
                    if (CurrentState != LetterState.FoundAndCorrectIndex
                       && CurrentState != LetterState.FoundButWrongIndex
                       && CurrentState != LetterState.WrongLetter)
                    {
                        CurrentState = state;
                        KeyboardButtonAnimator.SetTrigger(state.ToString());
                    }
                    //BackgroundImage.color = GameManager.Instance.DefaultColor;
                    break;
                }
            case LetterState.FoundAndCorrectIndex:
                {
                    CurrentState = state;
                    KeyboardButtonAnimator.SetTrigger(state.ToString());
                    //BackgroundImage.color = GameManager.Instance.FoundAndCorrectIndexColor;
                    break;
                }
            case LetterState.FoundButWrongIndex:
                {
                    if (CurrentState != LetterState.FoundAndCorrectIndex)
                    {
                        CurrentState = state;
                        KeyboardButtonAnimator.SetTrigger(state.ToString());
                        //BackgroundImage.color = GameManager.Instance.FoundButWrongIndexColor;
                    }

                    break;
                }
            case LetterState.WrongLetter:
                {
                    if (CurrentState != LetterState.FoundAndCorrectIndex
                    && CurrentState != LetterState.FoundButWrongIndex)
                    {
                        CurrentState = state;
                        KeyboardButtonAnimator.SetTrigger(state.ToString());
                        //BackgroundImage.color = GameManager.Instance.NotCorrectColor;
                    }
                    break;
                }
        }
    }
}
