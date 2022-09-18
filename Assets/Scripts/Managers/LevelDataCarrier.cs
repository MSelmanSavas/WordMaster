using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class LevelDataCarrier : MonoBehaviour
{
    public static LevelDataCarrier Instance;

    [SerializeField]
    List<LevelInitializationData> InitializationValues = new List<LevelInitializationData>();

    [SerializeField]
    LevelData CurrentLevelData;

    public WordDictionaryScriptableObject CurrentLanguageScriptableData;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(this);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(this);
    }

    public LevelInitializationValues GetInitValuesForLength(LevelType type, int Length)
    {
        LevelInitializationData datas = InitializationValues.Where(x => x.Type == type).FirstOrDefault();
        return datas.Values.Where(x => x.WordLength == Length).FirstOrDefault();
    }

    public void SetCurrentLevelData(LevelData data) => CurrentLevelData = data;
    public LevelData GetCurrentLevelData() => CurrentLevelData;
}
