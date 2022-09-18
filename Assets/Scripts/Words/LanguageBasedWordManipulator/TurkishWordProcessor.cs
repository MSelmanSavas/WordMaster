using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class TurkishWordProcessor : WordProcesscorBase
{
    private ListOfRoot RootList = new ListOfRoot();
    Dictionary<string, Root> RootWords;

    public override GameLanguages GetWordProcessorLanguage()
    {
        return GameLanguages.Turkish;
    }

    public override bool IsUsableWord(string word)
    {
        if (!RootWords.ContainsKey(word))
            return false;

        Root root = RootWords[word];

        bool IsUsableWord = true;

        List<string> EndsWithChecks = new List<string>
        {
            "CILIK",
            "CİLİK",
            "ÇILIK",
            "ÇİLİK",
            "LANMA",
            "LENME",
            "LİNME",
            "BİLME",
            "BİLMEK",
            "VERME",
            "VERMEK"
        };

        List<string> ContainsChecks = new List<string>
        {
            "Â",
            "Ê",
            "Î",
            "Î",
            "Ô",
            "Û",
        };

        if (word.Any(ch => !Char.IsLetter(ch)))
        {
            IsUsableWord = false;
        }

        if (root.anlamlarListe.Count == 0
           || int.Parse(root.anlamlarListe[0].fiil) != 0)
        {
            IsUsableWord = false;
        }

        if (root.taki != null || root.on_taki != null)
        {
            IsUsableWord = false;
        }

        for (int Index = 0; Index < EndsWithChecks.Count; Index++)
        {
            if (word.EndsWith(EndsWithChecks[Index]))
            {
                IsUsableWord = false;
            }
        }

        for (int Index = 0; Index < ContainsChecks.Count; Index++)
        {
            if (word.Contains(ContainsChecks[Index]))
            {
                IsUsableWord = false;
            }
        }

        return IsUsableWord;
    }

    public override List<WordData> LoadWordsFromPath(string DataPath)
    {
        RootList = new ListOfRoot();
        RootList.List = new List<Root>();

        RootWords = new Dictionary<string, Root>();

        using (StreamReader sr = new StreamReader(DataPath))
        {
            while (sr.Peek() >= 0)
            {
                string fileContents = sr.ReadLine();
                Root tempData = JsonUtility.FromJson<Root>(fileContents);
                RootList.List.Add(tempData);
            }
        }

        List<WordData> AllWordsList = new List<WordData>();

        for (int i = 0; i < RootList.List.Count; i++)
        {
            Root root = RootList.List[i];

            string meaning;

            try
            {
                meaning = root.anlamlarListe[0].anlam;
            }
            catch
            {
                continue;
            }

            WordData data = new WordData();
            data.Word = root.madde;
            data.Word = data.Word.ToUpper(new System.Globalization.CultureInfo("tr-TR"));
            data.WordLength = data.Word.Length;
            data.Meaning = meaning;
            data.Letters = new char[data.WordLength];

            for (int charIndex = 0; charIndex < data.WordLength; charIndex++)
            {
                data.Letters[charIndex] = data.Word[charIndex];
            }

            if (!RootWords.ContainsKey(data.Word))
            {
                RootWords.Add(data.Word, root);
            }

            AllWordsList.Add(data);
        }

        return AllWordsList;
    }

    [System.Serializable]
    public class OzelliklerListe
    {
        public string ozellik_id;
        public string tur;
        public string tam_adi;
        public string kisa_adi;
        public string ekno;
    }

    [System.Serializable]
    public class AnlamlarListe
    {
        public string anlam_id;
        public string madde_id;
        public string anlam_sira;
        public string fiil;
        public string tipkes;
        public string anlam;
        public string gos;
        public List<OzelliklerListe> ozelliklerListe;
    }

    [System.Serializable]
    public class ListOfRoot
    {
        public List<Root> List = new List<Root>();
    }

    [System.Serializable]
    public class Root
    {
        public int _id;
        public string madde_id;
        public string kac;
        public string kelime_no;
        public string cesit;
        public string anlam_gor;
        public object on_taki;
        public string madde;
        public string cesit_say;
        public string anlam_say;
        public object taki;
        public string cogul_mu;
        public string ozel_mi;
        public string lisan_kodu;
        public string lisan;
        public object telaffuz;
        public object birlesikler;
        public object font;
        public string madde_duz;
        public object gosterim_tarihi;

        [Sirenix.OdinInspector.ShowInInspector]
        public List<AnlamlarListe> anlamlarListe;
    }

}

