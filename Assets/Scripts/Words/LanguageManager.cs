using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LanguageManager : MonoBehaviour
{
    public static LanguageManager Instance;

    [SerializeField]
    GameLanguages CurrentLanguage;

    [SerializeField]
    public class DictionariesByLanguageDictionary : UnitySerializedDictionary<GameLanguages, WordDictionaryScriptableObject> { };

    public DictionariesByLanguageDictionary DictionariesByLanguage;

    public System.Action<GameLanguages> OnLanguageChanged;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public GameLanguages GetCurrentLanguage() => CurrentLanguage;

    public void ChangeLanguage(GameLanguages language)
    {
        CurrentLanguage = language;
        OnLanguageChanged?.Invoke(CurrentLanguage);
    }
}


