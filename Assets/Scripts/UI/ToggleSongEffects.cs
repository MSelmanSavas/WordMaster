using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleSongEffects : MonoBehaviour
{
    [SerializeField]
    GameObject objToToggle;

    private void Start()
    {
        objToToggle.SetActive(!SoundManager.Instance.IsSongVolumeOn);
    }

    public void ToggleSongVolume()
    {
        SoundManager.Instance.ToggleSongVolume(objToToggle);
    }
}
