using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class EnglishWordProccessor : WordProcesscorBase
{
    public override GameLanguages GetWordProcessorLanguage()
    {
        return GameLanguages.English;
    }

    List<string> EndsWithChecks = new List<string>
        {
            "ING"
        };

    List<string> ContainsChecks = new List<string>
        {
            "Â",
            "Ê",
            "Î",
            "Î",
            "Ô",
            "Û",
            "-"
        };

    public override bool IsUsableWord(string word)
    {
        if (word.Any(ch => !Char.IsLetter(ch)))
            return false;

        for (int Index = 0; Index < EndsWithChecks.Count; Index++)
            if (word.EndsWith(EndsWithChecks[Index]))
                return false;

        for (int Index = 0; Index < ContainsChecks.Count; Index++)
            if (word.Contains(ContainsChecks[Index]))
                return false;

        if (word.Contains(" "))
            return false;

        return true;
    }

    public override List<WordData> LoadWordsFromPath(string DataPath)
    {
        List<WordData> LoadedWords = new List<WordData>();

        ListOfWords wordsList = new ListOfWords();

        using (StreamReader sr = new StreamReader(DataPath))
        {
            while (sr.Peek() >= 0)
            {
                string fileContents = sr.ReadLine();
                if (string.IsNullOrEmpty(fileContents))
                    continue;

                try
                {
                    WordAndMeaning tempData = JsonUtility.FromJson<WordAndMeaning>(fileContents);
                    wordsList.WordsList.Add(tempData);
                }
                catch
                {
                    Debug.LogError(fileContents);
                }

            }
        }

        for (int i = 0; i < wordsList.WordsList.Count; i++)
        {
            WordAndMeaning currentWord = wordsList.WordsList[i];

            WordData data = new WordData();
            data.Word = currentWord.Word;
            data.Word = data.Word.ToUpper(new System.Globalization.CultureInfo("en-US"));
            data.WordLength = data.Word.Length;
            data.Meaning = currentWord.Meaning;
            data.Letters = new char[data.WordLength];

            for (int charIndex = 0; charIndex < data.WordLength; charIndex++)
            {
                data.Letters[charIndex] = data.Word[charIndex];
            }

            LoadedWords.Add(data);
        }

        return LoadedWords;
    }

    public class ListOfWords
    {
        public List<WordAndMeaning> WordsList = new List<WordAndMeaning>();
    }

    public class WordAndMeaning
    {
        public string Word;
        public string Meaning;
    }
}
