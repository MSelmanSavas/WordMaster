using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public abstract class WordProcesscorBase : MonoBehaviour, WordFilterInterface, WordLoaderInterface
{
    public abstract GameLanguages GetWordProcessorLanguage();
    public abstract bool IsUsableWord(string word);
    public abstract List<WordData> LoadWordsFromPath(string DataPath);
}
