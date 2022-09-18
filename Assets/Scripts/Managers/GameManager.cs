using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [SerializeField]
    WordDictionaryScriptableObject CurrentDictionarySO;

    [SerializeField]
    LevelData CurrentLevelData;

    [SerializeField]
    LevelInitializationValues CurrentLevelInitValues;

    [SerializeField]
    GameObject NotValidWordPrefab;

    [SerializeField]
    GameObject LetterBoxGroupPrefab;

    [SerializeField]
    GameObject LetterBoxPrefab;

    [SerializeField]
    public Color DefaultColor;

    [SerializeField]
    public Color FoundButWrongIndexColor;

    [SerializeField]
    public Color FoundAndCorrectIndexColor;

    [SerializeField]
    public Color NotCorrectColor;

    [SerializeField]
    WordData CurrentWordData;

    [SerializeField]
    GamefieldData CurrentGamefieldData;

    [SerializeField]
    KeyboardManager CurrentKeyboard;

    [SerializeField]
    GameObject LettersVerticalGrouper;

    [SerializeField]
    float WaitTimeBetweenLetterAnim = 0.1f;


    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(this);
            return;
        }

        Instance = this;

        CurrentKeyboard = GameObject.FindObjectOfType<KeyboardManager>();
    }

    public void RemoveLayoutGroupsFromGamefieldForAnimations()
    {
        UnityEngine.UI.LayoutGroup[] groups = LettersVerticalGrouper.GetComponentsInChildren<UnityEngine.UI.LayoutGroup>();

        for (int i = 0; i < groups.Length; i++)
        {
            Destroy(groups[i]);
        }

        Destroy(LettersVerticalGrouper.GetComponent<UnityEngine.UI.LayoutGroup>());
    }

    [Sirenix.OdinInspector.Button]
    public void LoadLevelData()
    {
        CurrentDictionarySO = LevelDataCarrier.Instance.CurrentLanguageScriptableData;
        CurrentLevelData = LevelDataCarrier.Instance.GetCurrentLevelData();
        CurrentLevelInitValues = LevelDataCarrier.Instance.GetInitValuesForLength(CurrentLevelData.Type, CurrentLevelData.WordLength);

        CurrentGamefieldData.WordLength = CurrentLevelInitValues.WordLength;
        CurrentGamefieldData.MaxTryAmount = CurrentLevelInitValues.TryAmount;
        CurrentGamefieldData.CurrentlyGuessedCharacters = new char[CurrentGamefieldData.WordLength];
        CurrentGamefieldData.CurrentWordLetterIndex = 0;
        CurrentGamefieldData.CurrentTryGroup = 0;

        CurrentWordData = DictionaryReader.Instance.GetWordByLevelTypeAndLenght(CurrentLevelData.Type, CurrentLevelData.WordLength);
    }

    public WordData GetCurrentWord() => CurrentWordData;
    public GamefieldData GetCurrentGamefieldData() => CurrentGamefieldData;

    [Sirenix.OdinInspector.Button]
    public void StartGamefieldConstruction()
    {
        int CurrentLength = CurrentGamefieldData.WordLength;
        int MaxTryAmount = CurrentGamefieldData.MaxTryAmount;

        for (int i = 0; i < MaxTryAmount; i++)
        {
            GameObject CurrentRow = Instantiate(LetterBoxGroupPrefab, transform.position, Quaternion.identity, LettersVerticalGrouper.transform);
            LetterBoxGroupData CurrentRowData = CurrentRow.GetComponent<LetterBoxGroupData>();
            CurrentGamefieldData.Groups.Add(CurrentRowData);

            for (int j = 0; j < CurrentLength; j++)
            {
                GameObject CurrentLetterBox = Instantiate(LetterBoxPrefab, transform.position, Quaternion.identity, CurrentRowData.transform);
                LetterBoxData CurrentLetterBoxData = CurrentLetterBox.GetComponent<LetterBoxData>();
                CurrentRowData.GroupData.Add(CurrentLetterBoxData);
            }
        }
    }

    public void OnValidInput(char PressedChar)
    {
        int CurrentGroupIndex = CurrentGamefieldData.CurrentTryGroup;
        int CurrentLetterIndex = CurrentGamefieldData.CurrentWordLetterIndex;

        if (CurrentLetterIndex < 0)
        {
            CurrentGamefieldData.CurrentWordLetterIndex = 0;
            CurrentLetterIndex = CurrentGamefieldData.CurrentWordLetterIndex;
        }

        if (CurrentLetterIndex >= CurrentGamefieldData.Groups[CurrentGroupIndex].GroupData.Count)
        {
            return;
        }

        CurrentGamefieldData.CurrentlyGuessedCharacters[CurrentLetterIndex] = PressedChar;
        CurrentGamefieldData.Groups[CurrentGroupIndex].GroupData[CurrentLetterIndex].Text.text = PressedChar.ToString();
        CurrentGamefieldData.CurrentWordLetterIndex++;
    }

    public void EraseLastEnteredCharacter()
    {
        int CurrentGroupIndex = CurrentGamefieldData.CurrentTryGroup;
        int CurrentLetterIndex = CurrentGamefieldData.CurrentWordLetterIndex - 1;

        if (CurrentLetterIndex < 0)
        {
            return;
        }

        CurrentGamefieldData.CurrentlyGuessedCharacters[CurrentLetterIndex] = '\0';
        CurrentGamefieldData.Groups[CurrentGroupIndex].GroupData[CurrentLetterIndex].Text.text = "";
        CurrentGamefieldData.CurrentWordLetterIndex--;
    }

    public void StartMainLoop()
    {
        StartCoroutine(MoveToNextGroup());
    }

    public IEnumerator MoveToNextGroup()
    {

        int CurrentGroupIndex = CurrentGamefieldData.CurrentTryGroup;
        int CurrentLetterIndex = CurrentGamefieldData.CurrentWordLetterIndex;
        int MaxGroupAmount = CurrentGamefieldData.Groups.Count;

        if (CurrentLetterIndex < CurrentGamefieldData.Groups[CurrentGroupIndex].GroupData.Count)
            yield break;

        if (CurrentGroupIndex >= MaxGroupAmount)
            yield break;

        string GuessedWord = new string(CurrentGamefieldData.CurrentlyGuessedCharacters);

        if (!CheckIfGivenGuessIsValid(GuessedWord))
        {
            Vector2 SpawnPosition = CurrentGamefieldData.Groups[CurrentGamefieldData.CurrentTryGroup].transform.position;
            Instantiate(NotValidWordPrefab, SpawnPosition, Quaternion.identity, null);
            yield break;
        }

        GameStateManager.Instance.CurrentGamestate = GameState.Pause;

        yield return CheckForGuessedLetters();

        if (CurrentGamefieldData.CurrentTryGroup + 1 >= MaxGroupAmount)
        {
            if (GameStateManager.Instance.CurrentGamestate != GameState.Win)
            {
                GameStateManager.Instance.CurrentGamestate = GameState.Lose;
            }
        }
        else if (GameStateManager.Instance.CurrentGamestate == GameState.Win)
        {
            yield break;
        }
        else
        {
            CurrentGamefieldData.CurrentTryGroup++;
            CurrentGamefieldData.CurrentWordLetterIndex = 0;
            CurrentGamefieldData.CurrentlyGuessedCharacters = new char[CurrentGamefieldData.WordLength];

            GameStateManager.Instance.CurrentGamestate = GameState.Play;
        }
    }

    private bool CheckIfGivenGuessIsValid(string GuessedWord)
    {
        return DictionaryReader.Instance.CheckIfGivenWordExists(GuessedWord);
    }

    private IEnumerator CheckForGuessedLetters()
    {
        //Seçilmiş kelime kendi harfinin yerini önce kendi yerinde arıycak, sonra diğerkilerine bakıcak.
        char[] CurrentlyGuessedChars = CurrentGamefieldData.CurrentlyGuessedCharacters;
        char[] SelectedWordChars = CurrentWordData.Letters;
        bool[] isCharacterFound = new bool[SelectedWordChars.Length];
        bool[] isCharacterFoundFullyTrue = new bool[SelectedWordChars.Length];
        LetterState[] LetterStatesForEachLetter = new LetterState[SelectedWordChars.Length];

        for (int i = 0; i < SelectedWordChars.Length; i++)
        {
            char CorrectChar = SelectedWordChars[i];

            if (CorrectChar == CurrentlyGuessedChars[i])
            {
                isCharacterFound[i] = true;
                isCharacterFoundFullyTrue[i] = true;
                LetterStatesForEachLetter[i] = LetterState.FoundAndCorrectIndex;
                continue;
            }

            for (int j = 0; j < CurrentlyGuessedChars.Length; j++)
            {
                if (j == i)
                    continue;

                char GuessedChar = CurrentlyGuessedChars[j];

                if (GuessedChar == CorrectChar)
                {
                    if (isCharacterFound[j])
                        continue;

                    isCharacterFound[j] = true;
                    LetterStatesForEachLetter[j] = LetterState.FoundButWrongIndex;
                    break;
                }
            }
        }

        for (int i = 0; i < CurrentlyGuessedChars.Length; i++)
        {
            if (isCharacterFound[i])
                continue;

            LetterStatesForEachLetter[i] = LetterState.WrongLetter;
        }

        for (int i = 0; i < CurrentlyGuessedChars.Length; i++)
        {
            CurrentGamefieldData.Groups[CurrentGamefieldData.CurrentTryGroup].GroupData[i].SetLetterState(LetterStatesForEachLetter[i]);
            CurrentKeyboard.SetKeyColor(CurrentlyGuessedChars[i], LetterStatesForEachLetter[i]);
            yield return new WaitForSeconds(WaitTimeBetweenLetterAnim);
        }

        yield return new WaitForSeconds(0.3f);

        bool isAllCharactersFoundTrue = true;

        for (int i = 0; i < isCharacterFoundFullyTrue.Length; i++)
        {
            if (isCharacterFoundFullyTrue[i] != true)
            {
                isAllCharactersFoundTrue = false;
                break;
            }
        }

        if (isAllCharactersFoundTrue)
        {
            GameStateManager.Instance.CurrentGamestate = GameState.Win;
        }

        yield break;
    }
}

