using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;
using System.Text.RegularExpressions;

public class TextInterpreter : MonoBehaviour
{
    // World 1 questionnaire
    public static TextAsset questionnaire;


    private void LoadQuestionData(int world, int scene)
    {
        string rawQuestionData = questionnaire.text;
        rawQuestionData = Regex.Replace(rawQuestionData, @"\t|\n|\r", "");
        allStageQuestionData = rawQuestionData.Split(new char[] { ';' }, StringSplitOptions.None).ToList();
    }


    [TextArea]
    public List<string> allStageQuestionData = new List<string>();
    String[] currentStageData;
    Timer timer;
    int randomStage;
    List<string> rawChoicePlacement = new List<string>();
    private void ShowQuestionAndChoices()
    {
        timer.StartTimer();

        // Get random question
        randomStage = UnityEngine.Random.Range(0, allStageQuestionData.Count);
        String currentStageRawQuestionData = allStageQuestionData[randomStage];

        // Intepret question data into perline where it can be shown to the question text and choices text
        currentStageData = currentStageRawQuestionData.Split(new char[] { '|' }, StringSplitOptions.None);
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

        rawChoicePlacement.Clear();
        List<string> unpairedChoice = new List<string>();
        for (int i = 0; i < choicesButtonText.Length; i++)
        {
            unpairedChoice.Add(currentStageData[1 + i]);
            rawChoicePlacement.Add(currentStageData[1 + i]);
        }

        choicePlacement = new Dictionary<int, string>();
        Char[] choiceAlphabat = new char[4] { 'A', 'B', 'C', 'D' };
        for (int i = 0; i < choicesButton.Length; i++)
        {
            int random = UnityEngine.Random.Range(0, unpairedChoice.Count);
            choicePlacement.Add(i, unpairedChoice[random]);

            choicesButtonText[i].text = choiceAlphabat[i] + ". " + unpairedChoice[random];
            unpairedChoice.RemoveAt(random);
        }

        Debug.Log("Jawaban benar: " + rawChoicePlacement[int.Parse(currentStageData[5])]);
    }

    public Dictionary<int, string> choicePlacement;
    BattleManager battleManager;
    MoveCounter moveCounter;
    int sideTurn = 0; // 0 - Player, 1 - Enemy
    public void CheckChoice(int index)
    {
        Debug.Log("Jawaban player: " + choicePlacement[index]);
        if (index >= 0)
        {
            if (choicePlacement[index] == rawChoicePlacement[int.Parse(currentStageData[5])])
            {
                if (sideTurn == 0)
                    battleManager.Battle(1);

            }
            else
            {
                if (sideTurn == 1)
                    battleManager.Battle(0);
            }
        }
        else
        {
            if (sideTurn == 1)
                battleManager.Battle(0);
        }

        allStageQuestionData.RemoveAt(randomStage);
        moveCounter.MinusMove();

        if (moveCounter.moveLeft <= 0)
            battleManager.GameOver();
        else
        {
            sideTurn = (sideTurn == 0) ? 1 : 0;
            Debug.Log("Giliran: " + sideTurn.ToString());
            ShowQuestionAndChoices();
        }

    }

    private void Start()
    {
        timer = GetComponent<Timer>();
        moveCounter = GetComponent<MoveCounter>();
        battleManager = GetComponent<BattleManager>();

        LoadQuestionData(0, 0);
        ShowQuestionAndChoices();
    }
}

