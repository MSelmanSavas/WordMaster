using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector.Editor;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using System.Linq;

public class WordSelectorEditor : OdinEditorWindow
{
    [MenuItem("Game Editors/Open Word Selector Editor")]
    public static void OpenWindow()
    {
        GetWindow(typeof(WordSelectorEditor));
    }

    WordDictionaryScriptableObject _DictionaryToModify;

    [PropertyOrder(-1)]
    [ShowInInspector]
    WordDictionaryScriptableObject DictionaryToModify
    {
        get
        {
            return _DictionaryToModify;
        }
        set
        {
            _DictionaryToModify = value;
            GetAllNotSelectedWordsFromDictionary();
        }
    }

    int _wordLength = 5;

    [PropertyOrder(-1)]
    [ShowInInspector]
    int WordLength
    {
        get
        {
            return _wordLength;
        }
        set
        {
            if (_DictionaryToModify == null)
                return;

            if (DictionaryToModify.AllFilteredWordsList.Where(x => x.WordsLength == value).FirstOrDefault() == null)
                return;

            _wordLength = value;
            GetAllNotSelectedWordsFromDictionary();
        }
    }

    [SerializeField]
    WordData CurrentWordData;

    int RealLeftOverWordsAmount = 0;

    List<WordData> NotSelectedWords = new List<WordData>();

    //[SerializeField]
    List<WordData> SelectedDailyWordsGroup = new List<WordData>();

    //[SerializeField]
    List<WordData> SelectedUnlimitedWordsGroup = new List<WordData>();

    // [PropertyOrder(-2)]
    // [Button(ButtonSizes.Large)]
    private void GetAllNotSelectedWordsFromDictionary()
    {
        SelectedDailyWordsGroup = DictionaryToModify.DailyWordsByLength.Where(x => x.WordsLength == WordLength).FirstOrDefault().WordList;
        SelectedUnlimitedWordsGroup = DictionaryToModify.NormalWordsByLength.Where(x => x.WordsLength == WordLength).FirstOrDefault().WordList;

        RemoveDuplicatesFromList(SelectedDailyWordsGroup);
        RemoveDuplicatesFromList(SelectedUnlimitedWordsGroup);

        RemoveDuplicatesFromOneListToAnother(ref SelectedDailyWordsGroup, ref SelectedUnlimitedWordsGroup);

        NotSelectedWords = new List<WordData>(DictionaryToModify.AllFilteredWordsList.Where(x => x.WordsLength == WordLength).FirstOrDefault().WordList);


        for (int i = 0; i < SelectedDailyWordsGroup.Count; i++)
        {
            if (CheckIfWordExistsInList(SelectedDailyWordsGroup[i].Word, NotSelectedWords))
                NotSelectedWords.RemoveAt(GetWordIndexFromList(SelectedDailyWordsGroup[i].Word, NotSelectedWords));
        }

        for (int i = 0; i < SelectedUnlimitedWordsGroup.Count; i++)
        {
            if (CheckIfWordExistsInList(SelectedUnlimitedWordsGroup[i].Word, NotSelectedWords))
                NotSelectedWords.RemoveAt(GetWordIndexFromList(SelectedUnlimitedWordsGroup[i].Word, NotSelectedWords));
        }

        RealLeftOverWordsAmount = NotSelectedWords.Count;

        CurrentWordData = GetRandomWordFromNotSelectedWords();
    }

    private WordData GetRandomWordFromNotSelectedWords()
    {
        return NotSelectedWords[Random.Range(0, NotSelectedWords.Count)];
    }

    [HorizontalGroup("Buttons")]
    [Button(ButtonSizes.Gigantic)]
    [GUIColor(72f / 256f, 207f / 256f, 173f / 256f)]
    void SelectDailyWord()
    {
        if (CurrentWordData == null)
            return;

        TryRemoveWordFromGivenList(CurrentWordData.Word, NotSelectedWords);

        if (!CheckIfWordExistsInList(CurrentWordData.Word, SelectedDailyWordsGroup))
            SelectedDailyWordsGroup.Add(CurrentWordData);

        EditorUtility.SetDirty(DictionaryToModify);
        RealLeftOverWordsAmount--;
        CurrentWordData = GetRandomWordFromNotSelectedWords();
    }

    [HorizontalGroup("Buttons")]
    [Button(ButtonSizes.Gigantic)]
    [GUIColor(160f / 256f, 212f / 256f, 104f / 256f)]
    void SelectUnlimitedWord()
    {
        if (CurrentWordData == null)
            return;

        TryRemoveWordFromGivenList(CurrentWordData.Word, NotSelectedWords);

        if (!CheckIfWordExistsInList(CurrentWordData.Word, SelectedUnlimitedWordsGroup))
            SelectedUnlimitedWordsGroup.Add(CurrentWordData);

        EditorUtility.SetDirty(DictionaryToModify);
        RealLeftOverWordsAmount--;
        CurrentWordData = GetRandomWordFromNotSelectedWords();
    }

    [Button(ButtonSizes.Gigantic)]
    [GUIColor(216f / 256f, 51f / 256f, 74f / 256f)]
    void DiscardWord()
    {
        if (CurrentWordData == null)
            return;

        TryRemoveWordFromGivenList(CurrentWordData.Word, NotSelectedWords);
        CurrentWordData = GetRandomWordFromNotSelectedWords();
    }

    public bool CheckIfWordExistsInList(string word, List<WordData> GivenList)
    {
        return GivenList.Where(x => x.Word == word).FirstOrDefault() != null;
    }

    public bool TryRemoveWordFromGivenList(string word, List<WordData> GivenList)
    {
        if (CheckIfWordExistsInList(word, GivenList))
        {
            WordData DuplicatedData = GivenList.Where(x => x.Word == word).FirstOrDefault();
            GivenList.Remove(DuplicatedData);
            return true;
        }

        return false;
    }

    public int GetWordIndexFromList(string word, List<WordData> GivenList)
    {
        return GivenList.IndexOf(GivenList.Where(x => x.Word == word).FirstOrDefault());
    }

    public void RemoveDuplicatesFromList(List<WordData> GivenList)
    {
        List<WordData> DuplicateDatas = new List<WordData>();

        for (int i = 0; i < GivenList.Count; i++)
        {
            WordData CurrentCheck = GivenList[i];

            if (GivenList.Where(x => x.Word == CurrentCheck.Word).Count() > 1)
            {
                if (DuplicateDatas.Where(y => y.Word == CurrentCheck.Word).FirstOrDefault() == null)
                {
                    DuplicateDatas.Add(CurrentCheck);
                }
            }
        }

        for (int i = 0; i < DuplicateDatas.Count; i++)
        {
            WordData DuplicatedData = GivenList.Where(x => x.Word == DuplicateDatas[i].Word).FirstOrDefault();
            GivenList.Remove(DuplicatedData);
            Debug.LogError("Duplicate Found! : " + DuplicateDatas[i].Word + "... Removing...");
        }
    }

    private void RemoveDuplicatesFromOneListToAnother(ref List<WordData> FirstList, ref List<WordData> PotentialDuplicatesList)
    {
        List<WordData> DuplicateDatas = new List<WordData>();

        for (int i = 0; i < FirstList.Count; i++)
        {
            WordData CurrentCheck = FirstList[i];

            if (PotentialDuplicatesList.Where(x => x.Word == CurrentCheck.Word).Count() > 0)
            {
                if (DuplicateDatas.Where(y => y.Word == CurrentCheck.Word).FirstOrDefault() == null)
                {
                    DuplicateDatas.Add(CurrentCheck);
                }
            }
        }

        for (int i = 0; i < DuplicateDatas.Count; i++)
        {
            WordData DuplicatedData = PotentialDuplicatesList.Where(x => x.Word == DuplicateDatas[i].Word).FirstOrDefault();
            PotentialDuplicatesList.Remove(DuplicatedData);
            Debug.LogError("Duplicate Found! : " + DuplicateDatas[i].Word + "... Removing...");
        }
    }



    private new void OnGUI()
    {
        base.OnGUI();

        EditorGUILayout.BeginVertical();

        EditorGUILayout.LabelField($"Word Length : {WordLength}");
        EditorGUILayout.LabelField($"Amount of Daily Words : {SelectedDailyWordsGroup.Count}");
        EditorGUILayout.LabelField($"Amount of Unlimited Words : {SelectedUnlimitedWordsGroup.Count}");
        EditorGUILayout.LabelField($"Amount of Current Session Leftover Words : {NotSelectedWords.Count}");
        EditorGUILayout.LabelField($"Amount of Real Leftover Words : {RealLeftOverWordsAmount}");


        EditorGUILayout.EndVertical();
    }
}

public enum WordSelectionType
{
    Daily,
    Unlimited
}
