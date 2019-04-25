using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using UnityEngine.UI;
using System.Text.RegularExpressions;

public class BattleManager : MonoBehaviour
{
    public TextAsset wordTest;
    public AnimationClip loadTimeReference;
    protected internal static TextAsset currentWord;

    private void OnEnable()
    {
        if (currentWord == null)
            currentWord = wordTest;

        timerText = GameObject.FindGameObjectWithTag("TimerText").GetComponent<Text>();
        wordText = GameObject.FindGameObjectWithTag("QuestionText").GetComponent<Text>();
        playerHPBar = GameObject.FindGameObjectWithTag("PlayerHP").GetComponent<Transform>();
        enemyHPBar = GameObject.FindGameObjectWithTag("EnemyHP").GetComponent<Transform>();
        moveCounterText = GameObject.FindGameObjectWithTag("MoveCounterText").GetComponent<Text>();
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

    public List<string> wordDataList = new List<string>();
    private void LoadBattle()
    {
        wordDataList = currentWord.text.Split(new char[] { ';' }, StringSplitOptions.None).ToList();
        CalculateDamage();
        ShowQuestion();
    }

    Text wordText;
    int answerIndex;
    List<string> currentData = new List<string>();
    char[] alphabet = new char[] { 'A', 'B', 'C', 'D' };
    Text moveCounterText;
    private void ShowQuestion()
    {
        int randomDataIndex = UnityEngine.Random.Range(0, wordDataList.Count);
        string rawCurrentData = wordDataList[randomDataIndex];
        wordDataList.Remove(rawCurrentData);
        moveCounterText.text = "Move left: " + wordDataList.Count;

        rawCurrentData = Regex.Replace(rawCurrentData, @"\t|\n|\r", "");
        currentData = rawCurrentData.Split(new char[] { '|' }, StringSplitOptions.None).ToList();
        wordText.text = currentData[0];
        currentData.RemoveAt(0);

        answerIndex = int.Parse(currentData[currentData.Count - 1]);
        currentData.RemoveAt(currentData.Count - 1);



        StopAllCoroutines();
        timerText.text = (curTimer = maxTimer).ToString();
        StartCoroutine(timer());
    }

    public void ChooseChoice(int index)
    {
        if (!hpBarAnimation)
        {
            if (index == answerIndex)
            {
                Debug.Log("Correct");
                if (currentTurn == 0)
                    DoDamage();
                else
                {
                    currentTurn = (currentTurn == 0) ? 1 : 0;
                    ShowQuestion();
                }
            }
            else
            {
                Debug.Log("Wrong");
                if (currentTurn == 0)
                {
                    currentTurn = (currentTurn == 0) ? 1 : 0;
                    ShowQuestion();
                }
                else
                    DoDamage();
            }
        }
    }

    // HPBar
    Transform playerHPBar;
    Transform enemyHPBar;
    int maxTimer = 20;
    int curTimer;
    Text timerText;
    bool hpBarAnimation;
    float targetHP;
    // Timer
    private void Update()
    {
        // Healthbar animation
        if (hpBarAnimation)
        {
            if (currentTurn == 0)
            {
                if (currentEnemyHP > targetHP)
                {
                    float enemyHP = currentEnemyHP -= Time.deltaTime;
                    enemyHPBar.localScale = new Vector3(enemyHP, enemyHPBar.localScale.y, enemyHPBar.localScale.z);
                    currentEnemyHP = enemyHP;
                }
                else
                {
                    hpBarAnimation = false;
                    currentTurn = (currentTurn == 0) ? 1 : 0;
                    ShowQuestion();
                }
            }
            else
            {
                if (currentPlayerHP > targetHP)
                {
                    float playerHP = currentPlayerHP -= Time.deltaTime;
                    playerHPBar.localScale = new Vector3(playerHP, enemyHPBar.localScale.y, enemyHPBar.localScale.z);
                    currentPlayerHP = playerHP;
                }
                else
                {
                    hpBarAnimation = false;
                    currentTurn = (currentTurn == 0) ? 1 : 0;
                    ShowQuestion();
                }
            }
        }
    }

    IEnumerator timer()
    {
        yield return new WaitForSeconds(1);
        timerText.text = (--curTimer).ToString();
        if (curTimer > 0)
            StartCoroutine(timer());
        else
            DoDamage();
    }

    public int currentTurn = 0; // 0 - Player, 1 - Enemy    
    float currentPlayerHP;
    public float currentEnemyHP;
    public void DoDamage()
    {
        hpBarAnimation = true;
        if (currentTurn == 0) // Deduct enemy HP (player attack)
            targetHP = currentEnemyHP - realPlayerDamage;
        else
            targetHP = currentPlayerHP - realEnemyDamage;
    }

    int playerMaxHP = 3;
    float playerDamage = 1;
    public float realPlayerDamage;
    int enemyMaxHP = 3;
    float enemyDamage = 1;
    float realEnemyDamage;
    private void CalculateDamage()
    {
        currentPlayerHP = 1;
        currentEnemyHP = 1;

        realPlayerDamage = playerDamage / enemyMaxHP;
        realEnemyDamage = enemyDamage / playerMaxHP;
    }
}
