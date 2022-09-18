using System.Collections.Generic;

[System.Serializable]
public class WordDataList
{
    public int WordsLength;
    public List<WordData> WordList = new List<WordData>();
    public WordDataList() { }
    public WordDataList(int Length)
    {
        WordsLength = Length;
    }
}
