using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector.Editor;
using Sirenix.OdinInspector;
using UnityEditor;
using System.Reflection;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using System.IO;
using System.Text;

public class LanguageEditor : OdinEditorWindow
{
    [MenuItem("Game Editors/Open Language Editor")]
    public static void OpenWindow()
    {
        GetWindow(typeof(LanguageEditor));
    }


    [Sirenix.OdinInspector.FilePath(ParentFolder = "Assets/Data", RequireExistingPath = true, AbsolutePath = true)]
    [SerializeField]
    string WordsPath;

    [SerializeField]
    WordDictionaryScriptableObject DictionaryToPopulate;

    [Sirenix.OdinInspector.ShowInInspector]
    [Sirenix.OdinInspector.ReadOnly]
    Dictionary<GameLanguages, Type> LanguageProcessors;

    [SerializeField]
    [ShowIf("isCurrentlyWorking")]
    [Sirenix.OdinInspector.ProgressBar(0, 1)]
    float Progress;

    Task CurrentWordProcessingTask;
    CancellationTokenSource CurrentWordProcessingTaskToken;

    [SerializeField]
    [ShowIf("isCurrentlyWorking")]
    string LastLoadedWord;

    [SerializeField]
    [ShowIf("isCurrentlyWorking")]
    string LastLoadedMeaning;

    GameObject referenceToProcessorObj;

    bool isCurrentlyWorking = false;

    [SerializeField]
    List<int> WordLengthsToLoad = new List<int> { 4, 5, 6, 7, 8 };

    private void Awake()
    {
        LoadWordProcessorsForLanguage();
    }

    private void OnValidate()
    {
        LoadWordProcessorsForLanguage();
    }

    private void OnDisable()
    {
        if (CurrentWordProcessingTaskToken != null)
            CurrentWordProcessingTaskToken.Cancel();
    }

    //[Sirenix.OdinInspector.Button(Sirenix.OdinInspector.ButtonSizes.Gigantic)]
    private void LoadWordProcessorsForLanguage()
    {
        LanguageProcessors = new Dictionary<GameLanguages, Type>();
        IEnumerable<Type> WordProcessorTypes = Assembly.GetAssembly(typeof(WordProcesscorBase)).GetTypes().Where(myType => myType.IsClass && !myType.IsAbstract && myType.IsSubclassOf(typeof(WordProcesscorBase)));

        WordProcesscorBase CurrentWordProcessor;

        foreach (System.Type type in WordProcessorTypes)
        {
            CurrentWordProcessor = (WordProcesscorBase)Activator.CreateInstance(type);
            LanguageProcessors.Add(CurrentWordProcessor.GetWordProcessorLanguage(), type);
        }
    }

    private IEnumerable<GameLanguages> GetGameLanguages()
    {
        return LanguageProcessors != null ? LanguageProcessors.Keys : null;
    }

    private WordProcesscorBase GetWordProcesscorFromLanguage(GameLanguages Language)
    {
        return (WordProcesscorBase)Activator.CreateInstance(LanguageProcessors[Language]);
    }

    [Sirenix.OdinInspector.Button(Sirenix.OdinInspector.ButtonSizes.Gigantic)]
    private async void StartWordProcessing()
    {
        try
        {
            CurrentWordProcessingTaskToken = new CancellationTokenSource();
            await Task.Run(() => ProcessorWordData(CurrentWordProcessingTaskToken.Token), CurrentWordProcessingTaskToken.Token);
        }
        catch (OperationCanceledException ex)
        {
            Debug.LogError("Word Processing Canceled...");
        }
        finally
        {
            DestroyImmediate(referenceToProcessorObj);
            CurrentWordProcessingTaskToken.Dispose();
            isCurrentlyWorking = false;
        }
    }

    [ShowIf("isCurrentlyWorking")]
    [Sirenix.OdinInspector.Button(Sirenix.OdinInspector.ButtonSizes.Gigantic)]
    private void StopWordProcessing()
    {
        CurrentWordProcessingTaskToken.Cancel();
    }

    private async Task ProcessorWordData(CancellationToken token)
    {
        isCurrentlyWorking = true;
        Debug.LogError("Starting word processing...");
        LastLoadedWord = "Loading...";
        LastLoadedMeaning = "Loading...";
        Progress = 0f;

        //Class shows as null when debugged, but all of the functions are running correctly.
        WordProcesscorBase CurrentWordProcessor = GetWordProcesscorFromLanguage(DictionaryToPopulate.Language);

        List<WordData> AllWordsForLanguage = CurrentWordProcessor.LoadWordsFromPath(WordsPath);

        DictionaryToPopulate.AllFilteredWordsList = new List<WordDataList>(); ;

        for (int i = 0; i < AllWordsForLanguage.Count; i++)
        {
            if (token.IsCancellationRequested)
            {
                token.ThrowIfCancellationRequested();
                break;
            }

            WordData CurrentWordData = AllWordsForLanguage[i];

            if (!WordLengthsToLoad.Contains(CurrentWordData.WordLength))
                continue;

            LastLoadedWord = CurrentWordData.Word;
            LastLoadedMeaning = CurrentWordData.Meaning;

            Progress = (float)i / (float)AllWordsForLanguage.Count;

            if (!CurrentWordProcessor.IsUsableWord(CurrentWordData.Word))
                continue;

            if (DictionaryToPopulate.AllFilteredWordsList.Where(x => x.WordsLength == CurrentWordData.WordLength).FirstOrDefault() == null)
                DictionaryToPopulate.AllFilteredWordsList.Add(new WordDataList(CurrentWordData.WordLength));

            List<WordData> CurrentList = DictionaryToPopulate.AllFilteredWordsList.Where(x => x.WordsLength == CurrentWordData.WordLength).FirstOrDefault().WordList;

            if (CurrentList.Where(x => x.Word == CurrentWordData.Word).FirstOrDefault() != null)
                continue;

            CurrentList.Add(CurrentWordData);
        }


        CheckForSelectedWordsExistance();

        isCurrentlyWorking = false;
        Progress = 1f;
    }

    [Sirenix.OdinInspector.PropertySpace(20)]
    [Button]
    private void ForceSaveDictionary()
    {
        EditorUtility.SetDirty(DictionaryToPopulate);
        AssetDatabase.SaveAssets();
    }

    [Sirenix.OdinInspector.PropertySpace(20)]
    [Button]
    private void CheckForSelectedWordsExistance()
    {
        if (DictionaryToPopulate.DailyWordsByLength == null || DictionaryToPopulate.DailyWordsByLength.Count == 0)
        {
            for (int i = 0; i < DictionaryToPopulate.AllFilteredWordsList.Count; i++)
            {
                if (DictionaryToPopulate.DailyWordsByLength.Where(x => x.WordsLength == DictionaryToPopulate.AllFilteredWordsList[i].WordsLength).FirstOrDefault() != null)
                    continue;

                WordDataList newList = new WordDataList();
                newList.WordsLength = DictionaryToPopulate.AllFilteredWordsList[i].WordsLength;
                newList.WordList = new List<WordData>();
                DictionaryToPopulate.DailyWordsByLength.Add(newList);
            }
        }

        if (DictionaryToPopulate.NormalWordsByLength == null || DictionaryToPopulate.NormalWordsByLength.Count == 0)
        {
            for (int i = 0; i < DictionaryToPopulate.AllFilteredWordsList.Count; i++)
            {
                if (DictionaryToPopulate.NormalWordsByLength.Where(x => x.WordsLength == DictionaryToPopulate.AllFilteredWordsList[i].WordsLength).FirstOrDefault() != null)
                    continue;

                WordDataList newList = new WordDataList();
                newList.WordsLength = DictionaryToPopulate.AllFilteredWordsList[i].WordsLength;
                newList.WordList = new List<WordData>();
                DictionaryToPopulate.NormalWordsByLength.Add(newList);
            }
        }

        DictionaryToPopulate.AllFilteredWordsList = DictionaryToPopulate.AllFilteredWordsList.OrderBy(x => x.WordsLength).ToList();
        DictionaryToPopulate.DailyWordsByLength = DictionaryToPopulate.DailyWordsByLength.OrderBy(x => x.WordsLength).ToList();
        DictionaryToPopulate.NormalWordsByLength = DictionaryToPopulate.NormalWordsByLength.OrderBy(x => x.WordsLength).ToList();

        try
        {
            for (int i = 0; i < DictionaryToPopulate.AllFilteredWordsList.Count; i++)
            {
                List<WordData> ExceptDaily = new List<WordData>();
                List<WordData> ExceptUnlimited = new List<WordData>();

                List<WordData> CurrentAllFilteredList = DictionaryToPopulate.AllFilteredWordsList[i].WordList;
                List<WordData> CurrentDailyList = DictionaryToPopulate.DailyWordsByLength[i].WordList;
                List<WordData> CurrentUnlimitedList = DictionaryToPopulate.NormalWordsByLength[i].WordList;

                for (int j = 0; j < CurrentDailyList.Count; j++)
                {
                    if (!DoesThisListContainsWord(CurrentDailyList[j].Word, ref CurrentAllFilteredList))
                    {
                        ExceptDaily.Add(CurrentDailyList[j]);
                    }
                }

                for (int j = 0; j < ExceptDaily.Count; j++)
                {
                    Debug.LogError($"{ExceptDaily[j].Word} is not in AllFilteredWords... Removing from DailyList...");
                    CurrentDailyList.Remove(ExceptDaily[j]);
                }

                for (int j = 0; j < CurrentUnlimitedList.Count; j++)
                {
                    if (!DoesThisListContainsWord(CurrentUnlimitedList[j].Word, ref CurrentAllFilteredList))
                    {
                        ExceptUnlimited.Add(CurrentUnlimitedList[j]);
                    }
                }

                for (int j = 0; j < ExceptUnlimited.Count; j++)
                {
                    Debug.LogError($"{ExceptUnlimited[j].Word} is not in AllFilteredWords... Removing from UnlimitedList...");
                    CurrentUnlimitedList.Remove(ExceptUnlimited[j]);
                }
            }

        }
        catch (System.Exception e)
        {
            Debug.LogError(e);
        }
    }

    private bool DoesThisListContainsWord(string word, ref List<WordData> data)
    {
        return data.Where(x => x.Word == word).FirstOrDefault() != null;
    }

    [Sirenix.OdinInspector.PropertySpace(20)]
    [Sirenix.OdinInspector.Button]
    public void LoadAllWordsDatas()
    {
        string DataPath = EditorUtility.OpenFilePanel("Select All Words Datas", Application.dataPath + "/Data/WordDatas/" + DictionaryToPopulate.Language.ToString(), "txt");

        if (System.IO.File.Exists(DataPath))
        {
            string data = System.IO.File.ReadAllText(DataPath);

            SimpleWordDataList WordList = JsonUtility.FromJson<SimpleWordDataList>(data);

            DictionaryToPopulate.AllFilteredWordsList = new List<WordDataList>();

            for (int i = 0; i < WordList.WordList.Count; i++)
            {
                WordData CurrentWordData = WordList.WordList[i];

                if (DictionaryToPopulate.AllFilteredWordsList.Where(x => x.WordsLength == CurrentWordData.WordLength).FirstOrDefault() == null)
                {
                    DictionaryToPopulate.AllFilteredWordsList.Add(new WordDataList(CurrentWordData.WordLength));
                }

                DictionaryToPopulate.AllFilteredWordsList.Where(x => x.WordsLength == CurrentWordData.WordLength).FirstOrDefault().WordList.Add(CurrentWordData);
            }

            EditorUtility.SetDirty(DictionaryToPopulate);
        }
    }

    [Sirenix.OdinInspector.PropertySpace(20)]
    [Sirenix.OdinInspector.Button]
    public void LoadSelectedDailyWordsDatas()
    {
        string DataPath = EditorUtility.OpenFilePanel("Select Selected Daily Words Datas", Application.dataPath + "/Data/WordDatas/" + DictionaryToPopulate.Language.ToString(), "txt");

        if (System.IO.File.Exists(DataPath))
        {
            string data = System.IO.File.ReadAllText(DataPath);

            SimpleWordDataList WordList = JsonUtility.FromJson<SimpleWordDataList>(data);

            DictionaryToPopulate.DailyWordsByLength = new List<WordDataList>();

            for (int i = 0; i < DictionaryToPopulate.AllFilteredWordsList.Count; i++)
            {
                DictionaryToPopulate.DailyWordsByLength.Add(new WordDataList(DictionaryToPopulate.AllFilteredWordsList[i].WordsLength));
            }

            for (int i = 0; i < WordList.WordList.Count; i++)
            {
                WordData CurrentWordData = WordList.WordList[i];

                if (DictionaryToPopulate.DailyWordsByLength.Where(x => x.WordsLength == CurrentWordData.WordLength).FirstOrDefault() == null)
                {
                    DictionaryToPopulate.DailyWordsByLength.Add(new WordDataList(CurrentWordData.WordLength));
                }

                DictionaryToPopulate.DailyWordsByLength.Where(x => x.WordsLength == CurrentWordData.WordLength).FirstOrDefault().WordList.Add(CurrentWordData);
            }
            EditorUtility.SetDirty(DictionaryToPopulate);
        }
    }

    [Sirenix.OdinInspector.PropertySpace(20)]
    [Sirenix.OdinInspector.Button]
    public void LoadSelectedNormalWordsDatas()
    {
        string DataPath = EditorUtility.OpenFilePanel("Select Normal Daily Words Datas", Application.dataPath + "/Data/WordDatas/" + DictionaryToPopulate.Language.ToString(), "txt");
        if (System.IO.File.Exists(DataPath))
        {
            string data = System.IO.File.ReadAllText(DataPath);

            SimpleWordDataList WordList = JsonUtility.FromJson<SimpleWordDataList>(data);

            DictionaryToPopulate.NormalWordsByLength = new List<WordDataList>();

            for (int i = 0; i < DictionaryToPopulate.AllFilteredWordsList.Count; i++)
            {
                DictionaryToPopulate.NormalWordsByLength.Add(new WordDataList(DictionaryToPopulate.AllFilteredWordsList[i].WordsLength));
            }

            for (int i = 0; i < WordList.WordList.Count; i++)
            {
                WordData CurrentWordData = WordList.WordList[i];

                if (DictionaryToPopulate.NormalWordsByLength.Where(x => x.WordsLength == CurrentWordData.WordLength).FirstOrDefault() == null)
                {
                    DictionaryToPopulate.NormalWordsByLength.Add(new WordDataList(CurrentWordData.WordLength));
                }

                DictionaryToPopulate.NormalWordsByLength.Where(x => x.WordsLength == CurrentWordData.WordLength).FirstOrDefault().WordList.Add(CurrentWordData);
            }

            EditorUtility.SetDirty(DictionaryToPopulate);
        }
    }

    [Sirenix.OdinInspector.PropertySpace(20)]
    [Sirenix.OdinInspector.Button]
    public void SaveAllWords()
    {
        string outputPath = EditorUtility.OpenFolderPanel("Select Normal Daily Words Datas", Application.dataPath + "/Data/WordDatas/", "");

        string outputFile;

        SimpleWordDataList simpleWordList = new SimpleWordDataList();

        #region 
        for (int i = 0; i < DictionaryToPopulate.AllFilteredWordsList.Count; i++)
        {
            for (int j = 0; j < DictionaryToPopulate.AllFilteredWordsList[i].WordList.Count; j++)
            {
                simpleWordList.WordList.Add(DictionaryToPopulate.AllFilteredWordsList[i].WordList[j]);
            }
        }

        outputFile = outputPath + $"/{DictionaryToPopulate.Language}AllFilteredWords.txt";

        if (File.Exists(outputFile))
        {
            File.Delete(outputFile);
        }

        using (FileStream fs = File.Open(outputFile, FileMode.OpenOrCreate))
        {
            string data = JsonUtility.ToJson(simpleWordList, prettyPrint: true);
            byte[] byteData = Encoding.UTF8.GetBytes(data);
            fs.Write(byteData, 0, byteData.Length);
        }
        #endregion

        simpleWordList.WordList.Clear();

        #region 
        for (int i = 0; i < DictionaryToPopulate.DailyWordsByLength.Count; i++)
        {
            for (int j = 0; j < DictionaryToPopulate.DailyWordsByLength[i].WordList.Count; j++)
            {
                simpleWordList.WordList.Add(DictionaryToPopulate.DailyWordsByLength[i].WordList[j]);
            }
        }

        outputFile = outputPath + $"/{DictionaryToPopulate.Language}SelectedDailyWords.txt";

        if (File.Exists(outputFile))
        {
            File.Delete(outputFile);
        }

        using (FileStream fs = File.Open(outputFile, FileMode.OpenOrCreate))
        {
            string data = JsonUtility.ToJson(simpleWordList, prettyPrint: true);
            byte[] byteData = Encoding.UTF8.GetBytes(data);
            fs.Write(byteData, 0, byteData.Length);
        }
        #endregion

        simpleWordList.WordList.Clear();

        #region 
        for (int i = 0; i < DictionaryToPopulate.NormalWordsByLength.Count; i++)
        {
            for (int j = 0; j < DictionaryToPopulate.NormalWordsByLength[i].WordList.Count; j++)
            {
                simpleWordList.WordList.Add(DictionaryToPopulate.NormalWordsByLength[i].WordList[j]);
            }
        }

        outputFile = outputPath + $"/{DictionaryToPopulate.Language}SelectedNormalWords.txt";

        if (File.Exists(outputFile))
        {
            File.Delete(outputFile);
        }

        using (FileStream fs = File.Open(outputFile, FileMode.OpenOrCreate))
        {
            string data = JsonUtility.ToJson(simpleWordList, prettyPrint: true);
            byte[] byteData = Encoding.UTF8.GetBytes(data);
            fs.Write(byteData, 0, byteData.Length);
        }
        #endregion
    }
}


