using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleSoundEffects : MonoBehaviour
{
    [SerializeField]
    GameObject objToToggle;

    private void Start()
    {
        objToToggle.SetActive(!SoundManager.Instance.IsEffectsVolumeOn);
    }

    public void ToggleSoundEffectsVolume()
    {
        SoundManager.Instance.ToggleSoundEffectsVolume(objToToggle);
    }
}
