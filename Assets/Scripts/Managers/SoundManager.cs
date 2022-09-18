using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    public bool IsEffectsVolumeOn;

    public bool IsSongVolumeOn;

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
        IsEffectsVolumeOn = PlayerPrefs.GetInt(nameof(IsEffectsVolumeOn), 1) > 0;
        IsSongVolumeOn = PlayerPrefs.GetInt(nameof(IsSongVolumeOn), 1) > 0;
        
        AudioManager.Instance.SetMuteOnSoundEffects(IsEffectsVolumeOn);
        AudioManager.Instance.SetMuteOnMusic(IsSongVolumeOn);
    }

    public void ToggleSoundEffectsVolume(GameObject objToToggle = null)
    {
        IsEffectsVolumeOn = !IsEffectsVolumeOn;
        PlayerPrefs.SetInt(nameof(IsEffectsVolumeOn), IsEffectsVolumeOn ? 1 : 0);
        AudioManager.Instance.SetMuteOnSoundEffects(IsEffectsVolumeOn);

        if (objToToggle)
            objToToggle.SetActive(!IsEffectsVolumeOn);
    }

    public void ToggleSongVolume(GameObject objToToggle = null)
    {
        IsSongVolumeOn = !IsSongVolumeOn;
        PlayerPrefs.SetInt(nameof(IsSongVolumeOn), IsSongVolumeOn ? 1 : 0);
        AudioManager.Instance.SetMuteOnMusic(IsSongVolumeOn);

        if (objToToggle)
            objToToggle.SetActive(!IsSongVolumeOn);
    }
}
