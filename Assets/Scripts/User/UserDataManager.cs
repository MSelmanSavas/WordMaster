using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using UnityEngine;
using System.Linq;

public class UserDataManager : MonoBehaviour
{
    public static UserDataManager Instance;

    [SerializeField]
    string CurrentSaveDataLocation;

    [SerializeField]
    UserData CurrentSaveData;

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
        UpdateUserDataFromLanguage(LanguageManager.Instance.GetCurrentLanguage());
    }

    private void OnEnable()
    {
        LanguageManager.Instance.OnLanguageChanged += UpdateUserDataFromLanguage;
    }

    // private void OnDisable()
    // {
    //     LanguageManagerInstance.OnLanguageChanged -= UpdateUserDataFromLanguage;
    // }

    void UpdateUserDataFromLanguage(GameLanguages language)
    {
        UpdateCurrentSaveDataLocationByLanguage(language);
        LoadSaveDataFromPath(CurrentSaveDataLocation);
    }

    [Sirenix.OdinInspector.Button]
    void UpdateCurrentSaveDataLocationByLanguage(GameLanguages Language)
    {
        CurrentSaveDataLocation = Application.persistentDataPath + $"/UserData{Language}.dat";
    }

    bool CheckSaveLocationExists(string path)
    {
        return System.IO.File.Exists(path);
    }

    void CreateDefaultSaveDataForLanguage(GameLanguages language)
    {
        WordDictionaryScriptableObject words = DictionaryReader.Instance.GetCurrentDictionary();

        CurrentSaveData = new UserData();
        CurrentSaveData.Language = language;
        CurrentSaveData.UserDataEntries = new List<UserDataPerLength>();

        for (int i = 0; i < words.AllFilteredWordsList.Count; i++)
        {
            int WordLength = words.AllFilteredWordsList[i].WordsLength;

            CurrentSaveData.UserDataEntries.Add(new UserDataPerLength());
            UserDataPerLength LastEntry = CurrentSaveData.UserDataEntries.Last();
            LastEntry.WordLength = words.AllFilteredWordsList[i].WordsLength;
            LastEntry.DailyInfo = new UserDataInfo();
            LastEntry.NormalInfo = new UserDataInfo();

            LastEntry.DailyInfo.GuessDistribution = new List<int>();
            LastEntry.NormalInfo.GuessDistribution = new List<int>();

            int tryAmount = LevelDataCarrier.Instance.GetInitValuesForLength(LevelType.Daily, WordLength).TryAmount;

            for (int j = 0; j < tryAmount; j++)
            {
                LastEntry.DailyInfo.GuessDistribution.Add(0);
                LastEntry.NormalInfo.GuessDistribution.Add(0);
            }
        }

        SaveDataToPath(CurrentSaveDataLocation);
    }

    public void UpdateUserDataForGivenLength(LevelType levelType, WordData wordData, GameState gameState, int GuessNumber)
    {
        UserDataPerLength EntryToUpdate = CurrentSaveData.UserDataEntries.Where(x => x.WordLength == wordData.WordLength).FirstOrDefault();
        UserDataInfo InfoToUpdate;

        try
        {
            switch (levelType)
            {
                case LevelType.Daily:
                    {
                        InfoToUpdate = EntryToUpdate.DailyInfo;
                        break;
                    }
                case LevelType.Normal:
                case LevelType.Timed:
                    {
                        InfoToUpdate = EntryToUpdate.NormalInfo;
                        break;
                    }
                default:
                    {
                        return;
                    }
            }

            if (InfoToUpdate.PlayedWords.Where(x => x.Word == wordData.Word).FirstOrDefault() != null)
                return;

            InfoToUpdate.PlayedWords.Add(wordData);

            InfoToUpdate.PlayedSessions++;

            switch (levelType)
            {
                case LevelType.Daily:
                    {
                        DictionaryReader.Instance.RemoveDailyGivenWordFromCache(wordData);
                        break;
                    }
                case LevelType.Normal:
                case LevelType.Timed:
                    {
                        DictionaryReader.Instance.RemoveNormalGivenWordFromCache(wordData);
                        break;
                    }
            }

            switch (gameState)
            {
                case GameState.Win:
                    {
                        InfoToUpdate.CurrentStreak++;

                        if (InfoToUpdate.BestStreak < InfoToUpdate.CurrentStreak)
                            InfoToUpdate.BestStreak = InfoToUpdate.CurrentStreak;

                        InfoToUpdate.IncreaseGuessDistributon(GuessNumber);
                        break;
                    }
                case GameState.Lose:
                    {
                        InfoToUpdate.FailedSessions++;
                        InfoToUpdate.CurrentStreak = 0;
                        break;
                    }
            }

            SaveDataToPath(CurrentSaveDataLocation);
        }
        catch (System.Exception e)
        {
            Debug.LogError(e);
        }

    }

    public UserDataPerLength GetUserDataEntryForLength(int length)
    {
        return CurrentSaveData.UserDataEntries.Where(x => x.WordLength == length).FirstOrDefault();
    }

    [Sirenix.OdinInspector.Button]
    bool LoadSaveDataFromPath(string path)
    {
        if (!CheckSaveLocationExists(path))
        {
            CreateDefaultSaveDataForLanguage(LanguageManager.Instance.GetCurrentLanguage());
        }

        byte[] byteData = File.ReadAllBytes(path);
        string EncryptedString = Encoding.ASCII.GetString(byteData);
        string GameData = EncryptionTool.DecryptString(EncryptedString);
        Debug.LogError(GameData);
        CurrentSaveData = JsonUtility.FromJson<UserData>(GameData);
        return true;
    }

    [Sirenix.OdinInspector.Button]
    bool SaveDataToPath(string path)
    {
        if (CheckSaveLocationExists(path))
        {
            File.Delete(path);
        }

        string GameData = JsonUtility.ToJson(CurrentSaveData);
        string EncryptedString = EncryptionTool.EncryptString(GameData);

        byte[] byteData = Encoding.ASCII.GetBytes(EncryptedString);

        File.WriteAllBytes(path, byteData);

        return true;
    }
}
