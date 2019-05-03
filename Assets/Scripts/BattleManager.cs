using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using System.Text.RegularExpressions;
using System;

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
        playerHPBar = GameObject.FindGameObjectWithTag("PlayerHP").GetComponent<Transform>();
        enemyHPBar = GameObject.FindGameObjectWithTag("EnemyHP").GetComponent<Transform>();
        moveCounterText = GameObject.FindGameObjectWithTag("MoveCounterText").GetComponent<Text>();

        wordTilesPool = GameObject.FindGameObjectWithTag("WordTiles").GetComponent<Transform>();
        letterTilesPool = GameObject.FindGameObjectWithTag("LetterTiles").GetComponent<Transform>();
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
        ShowWord();
    }

    public Transform letterPanel;
    Transform wordTilesPool;
    Transform letterTilesPool;

    public List<string> currentData = new List<string>();
    public string rawCurrentData;
    Text moveCounterText;

    string current_index;
    string current_word;
    string current_letters;
    string current_hint;
    public List<string> current_anagram = new List<string>();

    List<Transform> wordTiles = new List<Transform>();
    List<Transform> rawletterTiles = new List<Transform>();
    List<Transform> letterTiles = new List<Transform>();
    private void ShowWord()
    {
        // Get random words then iterate it's data
        int randomDataIndex = UnityEngine.Random.Range(0, wordDataList.Count);
        rawCurrentData = wordDataList[randomDataIndex];
        moveCounterText.text = "Move left: " + wordDataList.Count;
        rawCurrentData = Regex.Replace(rawCurrentData, @"\t|\n|\r", "");
        currentData = rawCurrentData.Split(new char[] { '|' }, StringSplitOptions.None).ToList();

        // Place each iterated data into variable
        current_index = currentData[0];
        current_word = currentData[1];
        current_letters = currentData[2];
        current_hint = currentData[3];
        current_anagram = currentData[4].Split(new char[] { ',' }, StringSplitOptions.None).ToList();

        // End of iteration
        wordDataList.Remove(rawCurrentData);
        currentData.RemoveAt(0);

        // Show word tiles
        foreach (char characters in current_word)
        {
            // Instantiate tiles
            Transform currentWordTile = Instantiate(letterPanel).GetComponent<Transform>();
            currentWordTile.SetParent(wordTilesPool);
            currentWordTile.localScale = new Vector3(1, 1, 1);

            wordTiles.Add(currentWordTile);
        }

        // Get all letters
        List<char> letters = new List<char>();
        foreach (char characters in current_letters)
        {
            letters.Add(characters);
        }

        // Show letter tiles
        int index = letters.Count;
        for (int i = 0; i < index; i++)
        {
            int randomIndex = UnityEngine.Random.Range(0, letters.Count);

            // Instantiate tiles
            Transform currentLetterTile = Instantiate(letterPanel).GetComponent<Transform>();
            currentLetterTile.SetParent(letterTilesPool);
            currentLetterTile.localScale = new Vector3(1, 1, 1);

            // Text
            Text currentLetterText = currentLetterTile.GetChild(0).GetComponent<Text>();
            currentLetterText.text = letters[randomIndex].ToString();

            rawletterTiles.Add(currentLetterTile);
            letters.RemoveAt(randomIndex);
        }

        StopAllCoroutines();
        timerText.text = (curTimer = maxTimer).ToString();
        StartCoroutine(timer());
    }

    public void ChooseChoice(int index)
    {
        if (!hpBarAnimation)
        {
            // if (index == answerIndex)
            // {
            //     Debug.Log("Correct");
            //     if (currentTurn == 0)
            //         DoDamage();
            //     else
            //     {
            //         currentTurn = (currentTurn == 0) ? 1 : 0;
            //         ShowWord();
            //     }
            // }
            // else
            // {
            //     Debug.Log("Wrong");
            //     if (currentTurn == 0)
            //     {
            //         currentTurn = (currentTurn == 0) ? 1 : 0;
            //         ShowWord();
            //     }
            //     else
            //         DoDamage();
            // }
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
                    enemyHPBar.localScale = new Vector3(enemyHPBar.localScale.x, enemyHP, enemyHPBar.localScale.z);
                    currentEnemyHP = enemyHP;
                }
                else
                {
                    hpBarAnimation = false;
                    currentTurn = (currentTurn == 0) ? 1 : 0;
                    ShowWord();
                }
            }
            else
            {
                if (currentPlayerHP > targetHP)
                {
                    float playerHP = currentPlayerHP -= Time.deltaTime;
                    playerHPBar.localScale = new Vector3(enemyHPBar.localScale.x, playerHP, enemyHPBar.localScale.z);
                    currentPlayerHP = playerHP;
                }
                else
                {
                    hpBarAnimation = false;
                    currentTurn = (currentTurn == 0) ? 1 : 0;
                    ShowWord();
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
