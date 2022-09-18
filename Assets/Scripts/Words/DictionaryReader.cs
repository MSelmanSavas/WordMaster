using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class DictionaryReader : MonoBehaviour
{
    public static DictionaryReader Instance;

    [SerializeField]
    WordDictionaryScriptableObject CurrentDictionary;

    [Sirenix.OdinInspector.ShowInInspector]
    Dictionary<int, Dictionary<string, WordData>> AllWordsCacheByLengths;

    [Sirenix.OdinInspector.ShowInInspector]
    Dictionary<int, List<WordData>> DailyWordsCacheByLengths;

    [Sirenix.OdinInspector.ShowInInspector]
    Dictionary<int, Dictionary<string, WordData>> NormalWordsCacheByLengths;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(this);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        Initialize();
    }

    void Initialize()
    {
        AllWordsCacheByLengths = new Dictionary<int, Dictionary<string, WordData>>();
        DailyWordsCacheByLengths = new Dictionary<int, List<WordData>>();
        NormalWordsCacheByLengths = new Dictionary<int, Dictionary<string, WordData>>();
    }

    public WordDictionaryScriptableObject GetCurrentDictionary() => CurrentDictionary;

    bool TryLoadAllWordsToCacheByLength(int length)
    {
        List<WordData> WordListForLength;

        WordListForLength = CurrentDictionary.AllFilteredWordsList.Where(x => x.WordsLength == length).FirstOrDefault().WordList;

        if (WordListForLength == null || WordListForLength.Count == 0)
            return false;

        AllWordsCacheByLengths.Add(length, new Dictionary<string, WordData>());
        Dictionary<string, WordData> WordsDictionary = AllWordsCacheByLengths[length];

        for (int i = 0; i < WordListForLength.Count; i++)
        {
            WordData CurrentWord = WordListForLength[i];
            WordsDictionary.Add(CurrentWord.Word, CurrentWord);
        }

        return true;
    }

    //TODO : Should filter out users previous daily guesses words
    bool TryLoadDailyWordsToCacheByLength(int length)
    {
        List<WordData> WordListForLength;

        WordListForLength = CurrentDictionary.DailyWordsByLength.Where(x => x.WordsLength == length).FirstOrDefault().WordList;

        if (WordListForLength == null || WordListForLength.Count == 0)
            return false;

        DailyWordsCacheByLengths.Add(length, new List<WordData>());
        List<WordData> WordsDictionary = DailyWordsCacheByLengths[length];

        UserDataPerLength DataForGivenLength = UserDataManager.Instance.GetUserDataEntryForLength(length);

        for (int i = 0; i < WordListForLength.Count; i++)
        {
            WordData CurrentWord = WordListForLength[i];

            // if (DataForGivenLength.DailyInfo.PlayedWords.Where(x => x.Word == CurrentWord.Word).FirstOrDefault() != null)
            // {
            //     continue;
            // }

            WordsDictionary.Add(CurrentWord);
        }

        return true;
    }

    //TODO : Should filter users previous guessed datas
    bool TryLoadNormalWordsToCacheByLength(int length)
    {
        List<WordData> WordListForLength;

        WordListForLength = CurrentDictionary.NormalWordsByLength.Where(x => x.WordsLength == length).FirstOrDefault().WordList;

        if (WordListForLength == null || WordListForLength.Count == 0)
            return false;

        NormalWordsCacheByLengths.Add(length, new Dictionary<string, WordData>());
        Dictionary<string, WordData> WordsDictionary = NormalWordsCacheByLengths[length];

        UserDataPerLength DataForGivenLength = UserDataManager.Instance.GetUserDataEntryForLength(length);

        for (int i = 0; i < WordListForLength.Count; i++)
        {
            WordData CurrentWord = WordListForLength[i];

            if (DataForGivenLength.NormalInfo.PlayedWords.Where(x => x.Word == CurrentWord.Word).FirstOrDefault() != null)
            {
                continue;
            }

            WordsDictionary.Add(CurrentWord.Word, CurrentWord);
        }

        return true;
    }

    //This function should return ordered daily word where it iterates through the daily word list.
    WordData GetDailyWordFromLength(int WordLength)
    {
        if (!DailyWordsCacheByLengths.ContainsKey(WordLength))
        {
            if (!TryLoadDailyWordsToCacheByLength(WordLength))
            {
                if (!AllWordsCacheByLengths.ContainsKey(WordLength))
                    if (!TryLoadAllWordsToCacheByLength(WordLength))
                        return null;

                string RandomAllDaily = AllWordsCacheByLengths[WordLength].Keys.ElementAt(Random.Range(0, AllWordsCacheByLengths[WordLength].Keys.Count));
                return AllWordsCacheByLengths[WordLength][RandomAllDaily];
            }
        }

        //string RandomDaily = DailyWordsCacheByLengths[WordLength].ElementAt(DailyWordsManager.Instance.GetTodaysDailyWordIndex());
        return DailyWordsCacheByLengths[WordLength][DailyWordsManager.Instance.GetTodaysDailyWordIndex()];
    }

    //This function should return random unlimited word.
    WordData GetNormalWordFromLength(int WordLength)
    {
        if (!NormalWordsCacheByLengths.ContainsKey(WordLength))
        {
            if (!TryLoadNormalWordsToCacheByLength(WordLength))
            {
                if (!AllWordsCacheByLengths.ContainsKey(WordLength))
                    if (!TryLoadAllWordsToCacheByLength(WordLength))
                        return null;

                string RandomAllUnlimited = AllWordsCacheByLengths[WordLength].Keys.ElementAt(Random.Range(0, AllWordsCacheByLengths[WordLength].Keys.Count));
                return AllWordsCacheByLengths[WordLength][RandomAllUnlimited];
            }
        }

        string RandomUnlimited = NormalWordsCacheByLengths[WordLength].Keys.ElementAt(Random.Range(0, NormalWordsCacheByLengths[WordLength].Keys.Count));
        return NormalWordsCacheByLengths[WordLength][RandomUnlimited];
    }

    public WordData GetWordByLevelTypeAndLenght(LevelType type, int WordLength)
    {
        switch (type)
        {
            case LevelType.Normal:
                {
                    return GetNormalWordFromLength(WordLength);
                }
            case LevelType.Daily:
                {
                    return GetDailyWordFromLength(WordLength);
                }
            default:
                {
                    throw new System.NotImplementedException($"LevelType : {type} is not implemented for DictionaryReader...");
                }
        }
    }

    public void RemoveDailyGivenWordFromCache(WordData wordData)
    {
        if (!DailyWordsCacheByLengths.ContainsKey(wordData.WordLength))
            if (!TryLoadDailyWordsToCacheByLength(wordData.WordLength))
                return;

        DailyWordsCacheByLengths[wordData.WordLength].Remove(DailyWordsCacheByLengths[wordData.WordLength].Where(x => x.Word == wordData.Word).First());
    }

    public void RemoveNormalGivenWordFromCache(WordData wordData)
    {
        if (!NormalWordsCacheByLengths.ContainsKey(wordData.WordLength))
            if (!TryLoadNormalWordsToCacheByLength(wordData.WordLength))
                return;

        NormalWordsCacheByLengths[wordData.WordLength].Remove(wordData.Word);
    }

    public bool CheckIfGivenWordExists(WordData wordData)
    {
        int checkCacheLength = wordData.WordLength;

        if (!AllWordsCacheByLengths.ContainsKey(checkCacheLength))
        {
            if (!TryLoadAllWordsToCacheByLength(checkCacheLength))
                return false;
        }

        //AllWordsCacheByLengths does contains the cache for given length.
        //Check if word exists.

        if (!AllWordsCacheByLengths[checkCacheLength].ContainsKey(wordData.Word))
            return false;


        return true;
    }

    public bool CheckIfGivenWordExists(string wordData)
    {
        int checkCacheLength = wordData.Length;

        if (!AllWordsCacheByLengths.ContainsKey(checkCacheLength))
        {
            if (!TryLoadAllWordsToCacheByLength(checkCacheLength))
                return false;
        }

        //AllWordsCacheByLengths does contains the cache for given length.
        //Check if word exists.

        if (!AllWordsCacheByLengths[checkCacheLength].ContainsKey(wordData))
            return false;


        return true;
    }
}
