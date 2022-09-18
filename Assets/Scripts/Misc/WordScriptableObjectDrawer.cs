#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using System.IO;
using System.Text;

//[CustomEditor(typeof(WordDictionaryScriptableObject))]
public class WordDictionaryScriptableObjectDrawer : Editor
{
    WordDictionaryScriptableObject currentTarget;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (currentTarget == null)
        {
            currentTarget = (WordDictionaryScriptableObject)serializedObject.targetObject;
        }
       

      

        if (GUILayout.Button("Save All Words"))
        {
            string outputPath = EditorUtility.OpenFolderPanel("Select Normal Daily Words Datas", Application.dataPath + "/Data/WordDatas/", "");

            string outputFile;

            SimpleWordDataList simpleWordList = new SimpleWordDataList();

            #region 
            for (int i = 0; i < currentTarget.AllFilteredWordsList.Count; i++)
            {
                for (int j = 0; j < currentTarget.AllFilteredWordsList[i].WordList.Count; j++)
                {
                    simpleWordList.WordList.Add(currentTarget.AllFilteredWordsList[i].WordList[j]);
                }
            }

            outputFile = outputPath + $"/{currentTarget.Language}AllFilteredWords.txt";

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
            for (int i = 0; i < currentTarget.DailyWordsByLength.Count; i++)
            {
                for (int j = 0; j < currentTarget.DailyWordsByLength[i].WordList.Count; j++)
                {
                    simpleWordList.WordList.Add(currentTarget.DailyWordsByLength[i].WordList[j]);
                }
            }

            outputFile = outputPath + $"/{currentTarget.Language}SelectedDailyWords.txt";

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
            for (int i = 0; i < currentTarget.NormalWordsByLength.Count; i++)
            {
                for (int j = 0; j < currentTarget.NormalWordsByLength[i].WordList.Count; j++)
                {
                    simpleWordList.WordList.Add(currentTarget.NormalWordsByLength[i].WordList[j]);
                }
            }

            outputFile = outputPath + $"/{currentTarget.Language}SelectedNormalWords.txt";

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
}
#endif