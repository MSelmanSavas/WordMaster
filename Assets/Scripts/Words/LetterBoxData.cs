using UnityEngine;

[System.Serializable]
public class LetterBoxData : MonoBehaviour
{
    public GameObject LetterBox;
    public TMPro.TextMeshProUGUI Text;
    public UnityEngine.UI.Image BoxBackground;
    public LetterState CurrentState;
    public Animator Animator;

    public void SetLetterState(LetterState state)
    {
        CurrentState = state;

        Animator.SetTrigger(CurrentState.ToString());

        switch (CurrentState)
        {
            case LetterState.WrongLetter:
                {
                    AudioManager.Instance.PlayBlackLetterSound();
                    break;
                }
            case LetterState.FoundButWrongIndex:
                {
                    AudioManager.Instance.PlayYellowLetterSound();
                    break;
                }
            case LetterState.FoundAndCorrectIndex:
                {
                    AudioManager.Instance.PlayGreenLetterSound();
                    break;
                }
        }
    }
}
