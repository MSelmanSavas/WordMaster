using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface WordLoaderInterface
{
    public List<WordData> LoadWordsFromPath(string DataPath);
}
