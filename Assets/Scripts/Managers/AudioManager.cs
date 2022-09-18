using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    public AudioSource MainThemeSource;
    public AudioSource OtherSoundsSource;
    public AudioClip BlackLetterSound;
    public AudioClip YellowLetterSound;
    public AudioClip GreenLetterSound;
    public AudioClip KeyboardClickSound;
    public AudioClip LoseTheme;
    public AudioClip WinTheme;
    public AudioClip MainTheme;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        PlayMainTheme();
    }

    public void PlayMainTheme()
    {
        MainThemeSource.Play();
    }

    public void SetMuteOnMusic(bool muteStatus)
    {
        MainThemeSource.mute = !muteStatus;
    }

    public void SetMuteOnSoundEffects(bool muteStatus)
    {
        OtherSoundsSource.mute = !muteStatus;
    }

    public void PlayKeyboardClickSound()
    {
        OtherSoundsSource.PlayOneShot(KeyboardClickSound);
    }

    public void PlayBlackLetterSound()
    {
        OtherSoundsSource.PlayOneShot(BlackLetterSound);
    }

    public void PlayYellowLetterSound()
    {
        OtherSoundsSource.PlayOneShot(YellowLetterSound);
    }

    public void PlayGreenLetterSound()
    {
        OtherSoundsSource.PlayOneShot(GreenLetterSound);
    }

    public void PlayLoseTheme()
    {
        OtherSoundsSource.PlayOneShot(LoseTheme);
    }

    public void PlayWinTheme()
    {
        OtherSoundsSource.PlayOneShot(WinTheme);
    }
}
