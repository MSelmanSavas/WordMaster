using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(TMPro.TextMeshProUGUI))]
public class CurrentWordMeaningDisplay : MonoBehaviour
{
    [SerializeField]
    TMPro.TextMeshProUGUI Text;


    private void OnEnable()
    {
        if (Text == null)
            GetComponent<TMPro.TextMeshProUGUI>();

        Text.text = GameManager.Instance.GetCurrentWord().Meaning;
    }
}
