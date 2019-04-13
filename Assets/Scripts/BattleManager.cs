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
        timerText = GameObject.FindGameObjectWithTag("TimerText").GetComponent<Text>();
        questionText = GameObject.FindGameObjectWithTag("QuestionText").GetComponent<Text>();
        playerHPBar = GameObject.FindGameObjectWithTag("PlayerHP").GetComponent<Transform>();
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
        currentTurn = (currentTurn == 0) ? 1 : 0;
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

        StopAllCoroutines();
        timerText.text = (curTimer = maxTimer).ToString();
        StartCoroutine(timer());
    }

    public void ChooseChoice(int index)
    {
        if (index == answerIndex)
            ShowQuestion();
        else
            Debug.Log("Answer wrong!");
    }

    // HPBar
    public int playerHealth;
    Transform playerHPBar;
    int maxTimer = 20;
    int curTimer;
    Text timerText;
    bool hpBarAnimation;
    // Timer
    private void Update()
    {
        // Healthbar animation
        if (hpBarAnimation)
        {

        }
    }

    IEnumerator timer()
    {
        yield return new WaitForSeconds(1);
        timerText.text = (--curTimer).ToString();
        if (curTimer > 0)
            StartCoroutine(timer());
        else
        {
            foreach (GameObject choiceButton in choicesButton)
            {
                Text choiceText = choiceButton.transform.GetChild(0).GetComponent<Text>();
                choiceText.text = "";
                questionText.text = "";
            }

            DoDamage(currentTurn);
        }
    }

    int currentTurn = 1; // 0 - Player, 1 - Enemy
    public void DoDamage(int turn)
    {

    }
}
