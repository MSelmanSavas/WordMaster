using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;
using System.Reflection;
using System.IO;
using System.Text;

[CreateAssetMenu(fileName = "NewWordDictionarySO", menuName = "Scriptables/WordDictionary", order = 1)]
public class WordDictionaryScriptableObject : ScriptableObject
{
    public GameLanguages Language;
    public List<WordDataList> AllFilteredWordsList = new List<WordDataList>();
    public List<WordDataList> DailyWordsByLength = new List<WordDataList>();
    public List<WordDataList> NormalWordsByLength = new List<WordDataList>();
}

[System.Serializable]
public class SimpleWordDataList
{
    [SerializeField]
    public List<WordData> WordList = new List<WordData>();
}