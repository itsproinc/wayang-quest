using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QnA : MonoBehaviour
{
    Text questionDisplay;
    List<Text> choicesDisplay = new List<Text> ();
    QuestionInterpreter questionInterpreter;
    GameObject [] choicesDisplayGameObject;
    private void Start ()
    {
        questionDisplay = GameObject.FindGameObjectWithTag ("QuestionDisplay").GetComponent<Text> ();
        questionInterpreter = GetComponent<QuestionInterpreter> ();

        choicesDisplayGameObject = GameObject.FindGameObjectsWithTag ("ChoicesDisplay");

        foreach (GameObject ChoicesDisplayGameObject in choicesDisplayGameObject)
            choicesDisplay.Add (ChoicesDisplayGameObject.transform.GetChild (0).GetComponent<Text> ());

        ShowQnA ();
    }

    [TextArea]
    int [] availableQuestionData;
    string rawQuestionData;
    string [] questionData;
    List<String> allChoices = new List<String> ();
    int corretChoice;
    private void ShowQnA ()
    {
        rawQuestionData = questionInterpreter.GetQuestionData (0, 0, 0);
        questionData = rawQuestionData.Split (new char [] { '|' }, StringSplitOptions.None);

        allChoices.Clear ();
        for (int i = 0; i < (questionData.Length - 2); i++)
        {
            allChoices.Add (questionData [i + 1]);
            availableChoices.Add (i);
        }

        StartCoroutine (ShowQuestion (questionData [0]));
        corretChoice = int.Parse (questionData [questionData.Length - 1]);
    }

    // Typewritter effect for question
    public float TypewritterSpeedDelay = 0.05f;
    bool doneShowQuestion;
    private IEnumerator ShowQuestion (String Question)
    {
        foreach (char QuestionLetters in Question)
        {
            questionDisplay.text = questionDisplay.text + QuestionLetters.ToString ();
            yield return new WaitForSeconds (TypewritterSpeedDelay);
        }

        ShowChoices ();
    }

    List<int> availableChoices = new List<int> ();
    int [] choicesPlacement;
    private void ShowChoices ()
    {
        choicesPlacement = new int [allChoices.Count];
        for (int i = 0; i < (questionData.Length - 2); i++)
        {
            // Pick random
            int random = UnityEngine.Random.Range (0, availableChoices.Count);
            choicesDisplay [i].text = allChoices [availableChoices [random]];
            choicesPlacement [i] = availableChoices [random];
            availableChoices.Remove (availableChoices [random]);
        }
    }
}