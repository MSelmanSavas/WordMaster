using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LevelData
{
    public LevelType Type;
    public int WordLength;
}

[System.Serializable]
public class LevelInitializationData
{
    public LevelType Type;
    public List<LevelInitializationValues> Values;
}

[System.Serializable]
public class LevelInitializationValues
{
    public int WordLength = 0;
    public int TryAmount = 1;
    public int TimeAmountSeconds = 10;
}

public enum LevelType
{
    Normal,
    Daily,
    Timed
}
