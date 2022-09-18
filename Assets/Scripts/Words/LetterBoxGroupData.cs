using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LetterBoxGroupData : MonoBehaviour
{
    public GameObject GroupHolder;
    public List<LetterBoxData> GroupData = new List<LetterBoxData>();
}

