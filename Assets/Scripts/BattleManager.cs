using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using UnityEngine.UI;
using System.Text.RegularExpressions;

public class BattleManager : MonoBehaviour
{
    public TextAsset questionnaireTest;
    public AnimationClip loadTimeReference;
    protected internal static TextAsset currentQuestionnaire;

    private void OnEnable()
    {
        if (currentQuestionnaire == null)
            currentQuestionnaire = questionnaireTest;

        choicesButton.AddRange(GameObject.FindGameObjectsWithTag("Choices").OrderBy(cB => cB.name));
        questionText = GameObject.FindGameObjectWithTag("QuestionText").GetComponent<Text>();
    }

    private void Start()
    {
        StartCoroutine(WaitAndLoadCutscene());
    }

    private IEnumerator WaitAndLoadCutscene()
    {
        yield return new WaitForSeconds(loadTimeReference.length);
        LoadBattle();
    }

    List<string> questionDataList = new List<string>();
    private void LoadBattle()
    {
        questionDataList = currentQuestionnaire.text.Split(new char[] { ';' }, StringSplitOptions.None).ToList();
        ShowQuestion();
    }

    Text questionText;
    int answerIndex;
    public List<GameObject> choicesButton = new List<GameObject>();
    public List<string> currentData = new List<string>();
    char[] alphabet = new char[] { 'A', 'B', 'C', 'D' };
    private void ShowQuestion()
    {
        int randomDataIndex = UnityEngine.Random.Range(0, questionDataList.Count);

        string rawCurrentData = questionDataList[randomDataIndex];
        rawCurrentData = Regex.Replace(rawCurrentData, @"\t|\n|\r", "");
        currentData = rawCurrentData.Split(new char[] { '|' }, StringSplitOptions.None).ToList();
        questionText.text = currentData[0];
        currentData.RemoveAt(0);

        answerIndex = int.Parse(currentData[currentData.Count - 1]);
        currentData.RemoveAt(currentData.Count - 1);

        List<string> choiceReference = new List<string>(currentData);
        int i = 0;
        foreach (GameObject choiceButton in choicesButton)
        {
            Button cButon = choiceButton.GetComponent<Button>();
            cButon.onClick.RemoveAllListeners();
            int randomChoiceIndex = UnityEngine.Random.Range(0, currentData.Count);

            Text choiceText = choiceButton.transform.GetChild(0).GetComponent<Text>();
            choiceText.text = alphabet[i++] + ". " + currentData[randomChoiceIndex];

            int realIndex = choiceReference.IndexOf(currentData[randomChoiceIndex]);
            cButon.onClick.AddListener(() => ChooseChoice(realIndex));

            currentData.RemoveAt(randomChoiceIndex);
        }
    }

    public void ChooseChoice(int index)
    {
        if (index == answerIndex)
            Debug.Log("Answer correct");
        else
            Debug.Log("Answer wrong!");
    }

    private void Update()
    {
        // Healthbar animation
         
    }
}
