using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;
using System.Text.RegularExpressions;

public class QuestionInterpreter : MonoBehaviour
{
    // World 1 questionnaire
    public TextAsset[] w1_questionnaire;
    public TextAsset[] questionnaire;

    private string GetLevelQuestionData(int world, int scene)
    {
        // Pick world
        switch (world)
        {
            case 0:
                questionnaire = w1_questionnaire;
                break;
        }

        // Pick scene
        TextAsset sceneQuestionnaire;
        sceneQuestionnaire = questionnaire[scene];

        return sceneQuestionnaire.text;
    }

    private void LoadQuestionData(int world, int scene)
    {
        string rawQuestionData = GetLevelQuestionData(world, scene);
        rawQuestionData = Regex.Replace(rawQuestionData, @"\t|\n|\r", "");
        allStageQuestionData = rawQuestionData.Split(new char[] { ';' }, StringSplitOptions.None).ToList();
    }


    [TextArea]
    public List<string> allStageQuestionData = new List<string>();
    private void ShowQuestionAndChoices()
    {
        // Get random question
        int randomStage = UnityEngine.Random.Range(0, allStageQuestionData.Count);
        String currentStageRawQuestionData = allStageQuestionData[randomStage];

        // Intepret question data into perline where it can be shown to the question text and choices text
        String[] currentStageData = currentStageRawQuestionData.Split(new char[] { '|' }, StringSplitOptions.None);
        /*  currentStageData[0] = Question
            currentStageData[1] = Choice 1
            currentStageData[2] = Choice 2
            currentStageData[3] = Choice 3
            currentStageData[4] = Choice 4
            currentStageData[5] = Correct answer
        */

        // Show question data to choices text
        Text questionText = GameObject.FindGameObjectWithTag("QuestionText").GetComponent<Text>();
        questionText.text = currentStageData[0];

        // Randomize choice placement & show it
        GameObject[] choicesButton = GameObject.FindGameObjectsWithTag("ChoiceText").OrderBy(choiceButton => choiceButton.name).ToArray();
        Text[] choicesButtonText = new Text[choicesButton.Length];
        for (int i = 0; i < choicesButton.Length; i++)
            choicesButtonText[i] = choicesButton[i].GetComponent<Text>();

        List<string> unpairedChoice = new List<string>();
        for (int i = 0; i < choicesButtonText.Length; i++)
            unpairedChoice.Add(currentStageData[1 + i]);

        choicePlacement = new Hashtable();
        for (int i = 1; i <= choicesButton.Length; i++)
        {
            int random = UnityEngine.Random.Range(0, unpairedChoice.Count);
            choicePlacement.Add(i, unpairedChoice[random]);

            choicesButtonText[i - 1].text = unpairedChoice[random];
            unpairedChoice.RemoveAt(random);
        }
    }

    public Hashtable choicePlacement;
    public void CheckChoice(int index)
    {

    }

    private void Start()
    {
        LoadQuestionData(0, 0);
        ShowQuestionAndChoices();
    }
}

