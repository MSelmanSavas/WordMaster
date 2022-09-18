using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GamefieldData
{
    public GameObject VerticalGrouper;
    public List<LetterBoxGroupData> Groups = new List<LetterBoxGroupData>();
    public int WordLength = 0;
    public int CurrentTryGroup = 0;
    public int MaxTryAmount = 0;
    public int CurrentWordLetterIndex = 0;
    public char[] CurrentlyGuessedCharacters = new char[0];
}