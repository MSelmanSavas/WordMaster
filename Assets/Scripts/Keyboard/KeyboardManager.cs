using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyboardManager : MonoBehaviour
{
    [Sirenix.OdinInspector.ShowInInspector]
    Dictionary<char, KeyboardButton> Keys;

    private void Awake()
    {
        Keys = new Dictionary<char, KeyboardButton>();
    }

    private void Start()
    {
        PopulateKeysDictionary();
    }

    private void PopulateKeysDictionary()
    {
        KeyboardButton[] buttons = GetComponentsInChildren<KeyboardButton>();

        for (int i = 0; i < buttons.Length; i++)
        {
            Keys.Add(buttons[i].GetKeyCharacter(), buttons[i]);
            buttons[i].SetKeyState(LetterState.Default);
        }
    }

    public void SetKeyColor(char KeyCharacter, LetterState State)
    {
        if (Keys.ContainsKey(KeyCharacter))
        {
            Keys[KeyCharacter].SetKeyState(State);
        }
    }
}
