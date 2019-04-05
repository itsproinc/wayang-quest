using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TextManager : MonoBehaviour
{
    public TextAsset[] w1_cutscene;
    public TextAsset[] w1_questionnaire;

    public void LoadLevel(string RawLevelData)
    {
        string[] levelData = RawLevelData.Split(new char[] { '/' }, StringSplitOptions.None);
        int world = int.Parse(levelData[0]);
        int level = int.Parse(levelData[1]);

        switch (world)
        {
            case 0:
                Cutscene.currentCutscene = w1_cutscene[level];
                TextInterpreter.questionnaire = w1_questionnaire[level];
                break;
        }
    }
}
