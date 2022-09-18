using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class WordData
{
    public int WordLength;

    [TextArea(1, 5)]
    public string Word;

    [TextArea(1, 5)]
    public string Meaning;
    public char[] Letters;
}
