using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayKeyClickSound : MonoBehaviour
{
    public void PlaySound()
    {
        AudioManager.Instance.PlayKeyboardClickSound();
    }
}
