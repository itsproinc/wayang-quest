using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using System.Text.RegularExpressions;
using System;
using UnityEngine.SceneManagement;

public class BattleManager : MonoBehaviour
{
  public TextAsset wordTest;
  public AnimationClip loadTimeReference;
  protected internal static TextAsset currentWord;
  public int turnLeft = 10;
  protected internal static int currentWorld = 1;
  protected internal static int currentLevel = 1;
  protected internal static int prevStar = 0;

  Animator battleCanvas;
  LoadingManager loadingManager;

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

    partofSpeechText = GameObject.FindGameObjectWithTag("PartofSpeechText").GetComponent<Text>();
    hintText = GameObject.FindGameObjectWithTag("HintText").GetComponent<Text>();

    wordTilesPool_Animator = wordTilesPool.GetComponent<Animator>();
    letterTilesPool_Animator = letterTilesPool.GetComponent<Animator>();

    gameAudio = GetComponent<AudioSource>();
  }

  private void Start()
  {
    maxTimer *= currentLevel;
    if (maxTimer > 100)
      maxTimer = 100;

    battleCanvas = GetComponent<Animator>();
    loadingManager = GameObject.FindGameObjectWithTag("Loading").GetComponent<LoadingManager>();

    CalculateHealth();
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
    wordDataList = currentWord.text.Split(new char[] { '^' }, StringSplitOptions.None).ToList();
    CheckUsedWord();

    ShowWord();
  }

  public List<int> usedWord = new List<int>();
  private void CheckUsedWord()
  {
    for (int i = 0; i < wordDataList.Count; i++)
    {
      int checkIndex = int.Parse(wordDataList[i].Split(new char[] { '|' }, StringSplitOptions.None)[0]);

      if (usedWord.Contains(checkIndex))
        wordDataList.RemoveAt(i);
    }
  }

  public Button letterButton;
  Transform wordTilesPool;
  Animator wordTilesPool_Animator;
  Transform letterTilesPool;
  Animator letterTilesPool_Animator;

  List<string> currentData = new List<string>();
  string rawCurrentData;
  Text moveCounterText;
  Text partofSpeechText;
  Text hintText;

  int current_index;
  string current_partofSpeech;
  public string current_word;
  int current_wordSize;
  string current_hint;
  string current_letters;
  public List<string> anagram_word = new List<string>();
  public List<string> anagram_partOfSpeech = new List<string>();
  public List<string> anagram_definition = new List<string>();

  List<Transform> wordTiles = new List<Transform>();
  List<Transform> letterTiles = new List<Transform>();

  Text currentLetterText;

  public Transform playerHalo;
  public Transform enemyHalo;
  private void ShowWord()
  {
    turnLeft--;

    // Halo
    if (currentTurn == 0)
    {
      playerHalo.gameObject.SetActive(true);
      enemyHalo.gameObject.SetActive(false);
    }
    else
    {
      playerHalo.gameObject.SetActive(false);
      enemyHalo.gameObject.SetActive(true);
    }

    // Remove tiles
    if (wordTiles.Count > 0)
    {
      foreach (Transform wTiles in wordTiles)
        Destroy(wTiles.gameObject);
    }

    wordTiles.Clear();
    if (letterTiles.Count > 0)
    {
      foreach (Transform lTiles in letterTiles)
        Destroy(lTiles.gameObject);
    }

    letterTiles.Clear();

    // Get random words then iterate it's data
    int randomDataIndex = UnityEngine.Random.Range(0, wordDataList.Count);

    rawCurrentData = wordDataList[randomDataIndex];
    moveCounterText.text = "Move left: " + turnLeft;
    rawCurrentData = Regex.Replace(rawCurrentData, @"\t|\n|\r", String.Empty);
    currentData = rawCurrentData.Split(new char[] { '|' }, StringSplitOptions.None).ToList();

    // Place each iterated data into variable
    current_index = int.Parse(currentData[0]);
    usedWord.Add(current_index);

    String[] rawWord = currentData[1].Split(new char[] { ':' }, StringSplitOptions.None);

    current_partofSpeech = rawWord[0];
    partofSpeechText.text = current_partofSpeech;

    String[] wordAndMeaning = rawWord[1].Split(new char[] { ',' }, StringSplitOptions.None);
    current_word = wordAndMeaning[0];
    current_hint = wordAndMeaning[1].Trim();
    hintText.text = current_hint;

    current_letters = currentData[2];
    current_wordSize = current_word.Length;
    lettersInput = new String[current_wordSize];
    lettersInputTiles = new Button[current_wordSize];

    anagram_word.Clear();
    anagram_partOfSpeech.Clear();
    anagram_definition.Clear();
    usedAnagram.Clear();
    // Find and store anagram
    if (currentData[3] != "")
    {
      String[] rawAnagram = currentData[3].Split(new char[] { '/' }, StringSplitOptions.None);

      foreach (String anagram in rawAnagram)
      {
        String[] rawAnagramWord = anagram.Split(new char[] { ':' }, StringSplitOptions.None);

        anagram_partOfSpeech.Add(rawAnagramWord[0]);
        String[] anagramWordAndMeaning = rawAnagramWord[1].Split(new char[] { ',' }, StringSplitOptions.None);
        anagram_word.Add(anagramWordAndMeaning[0]);
        anagram_definition.Add(anagramWordAndMeaning[1].Trim());
      }
    }

    // End of iteration
    wordDataList.RemoveAt(randomDataIndex);
    currentData.RemoveAt(0);

    // Show word tiles
    int letterIndex = 0;
    foreach (char characters in current_word)
    {
      // Instantiate tiles
      Transform currentWordTile = Instantiate(letterButton).GetComponent<Transform>();
      currentWordTile.SetParent(wordTilesPool);
      currentWordTile.localScale = new Vector3(1, 1, 1);
      currentWordTile.name = letterIndex++.ToString();

      wordTiles.Add(currentWordTile);

      // Add functionality to button
      Button letterTilerButton = currentWordTile.GetComponent<Button>();
      letterTilerButton.onClick.AddListener(() => RemoveLetter(int.Parse(currentWordTile.name)));
    }

    ShuffleLetter();

    StopAllCoroutines();
    timerText.text = (curTimer = maxTimer).ToString();
    StartCoroutine(timer());

    LetterPlacementCheck();
    anagramModifier = 0;
  }

  public String shuffledLetter;
  public List<Char> shuffledCharacters = new List<Char>();
  private void ShuffleLetter()
  {
    disablePress = true;

    shuffledLetter = String.Empty;
    shuffledCharacters.Clear();

    foreach (Transform currentTile in letterTiles)
    {
      Destroy(currentTile.gameObject);
    }
    letterTiles.Clear();

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
      Transform currentLetterTile = Instantiate(letterButton).GetComponent<Transform>();
      currentLetterTile.SetParent(letterTilesPool);
      currentLetterTile.localScale = new Vector3(1, 1, 1);

      // Text
      shuffledCharacters.Add(letters[randomIndex]);
      shuffledLetter += letters[randomIndex];

      letterTiles.Add(currentLetterTile);
      letters.RemoveAt(randomIndex);

      // Add functionality to button
      Button currentLetterButton = currentLetterTile.GetComponent<Button>();
      currentLetterButton.onClick.AddListener(() => LetterPress(currentLetterButton));
    }

    // Text
    if (!shuffledLetter.Equals(current_word))
    {
      // Check if current word (2)
      if (!shuffledLetter.Contains(current_word))
      {
        // Check if anagram
        if (anagram_word.Count > 0)
        {
          for (int i = 0; i < anagram_word.Count; i++)
          {
            // Check if anagram
            if (!shuffledLetter.Equals(anagram_word[i]))
            {
              // Check if anagram (broad)
              if (!shuffledLetter.Contains(anagram_word[i]))
                ShuffleSuccess();
              else
              {
                ShuffleLetter();
                break;
              }
            }
            else
            {
              ShuffleLetter();
              break;
            }
          }
        }
        else
          ShuffleSuccess();
      }
      else
        ShuffleLetter();
    }
    else
      ShuffleLetter();
  }

  void ShuffleSuccess()
  {
    for (int j = 0; j < letterTiles.Count; j++)
    {
      currentLetterText = letterTiles[j].GetComponentInChildren<Text>();
      currentLetterText.text = shuffledCharacters[j].ToString();

      letterTiles[j].name = currentLetterText.text;
      prevShuffledLetter = shuffledLetter;
    }

    disablePress = false;
  }

  public void ReshuffleButton()
  {
    if (!disablePress)
      ReshuffleLetter();
  }

  public Transform[] toShuffle;
  public int[] shuffledIndex;
  List<int> shufflePosition = new List<int>();
  public String prevShuffledLetter;
  void ReshuffleLetter()
  {
    disablePress = true;

    shuffledLetter = String.Empty;
    shuffledCharacters.Clear();
    shufflePosition.Clear();

    for (int i = 0; i < letterTiles.Count; i++)
    {
      shufflePosition.Add(i);
    }

    toShuffle = new Transform[letterTiles.Count];
    for (int i = 0; i < letterTiles.Count; i++)
    {
      toShuffle[i] = letterTiles[i];

      currentLetterText = letterTiles[i].GetComponentInChildren<Text>();
      currentLetterText.text = String.Empty;
    }

    // Shuffle
    shuffledIndex = new int[toShuffle.Length];
    for (int i = 0; i < toShuffle.Length; i++)
    {
      int randomIndex = UnityEngine.Random.Range(0, shufflePosition.Count);
      int randomShuffleIndex = shufflePosition[randomIndex];
      shuffledIndex[i] = randomShuffleIndex;

      // Get button name
      String currentShuffleButton = toShuffle[randomShuffleIndex].name;
      shuffledCharacters.Add(Char.Parse(toShuffle[randomShuffleIndex].name));
      shuffledLetter += toShuffle[randomShuffleIndex].name;

      shufflePosition.RemoveAt(randomIndex);
    }

    // Text
    if (!shuffledLetter.Equals(prevShuffledLetter))
    {
      if (!shuffledLetter.Equals(current_word))
      {
        // Check if current word (2)
        if (!shuffledLetter.Contains(current_word))
        {
          if (anagram_word.Count > 0)
          {
            // Check if anagram
            for (int i = 0; i < anagram_word.Count; i++)
            {
              // Check if anagram
              if (!shuffledLetter.Equals(anagram_word[i]))
              {
                // Check if anagram (broad)
                if (!shuffledLetter.Contains(anagram_word[i]))
                  ReshuffleSuccess();
                else
                {
                  ReshuffleLetter();
                  break;
                }
              }
              else
              {
                ReshuffleLetter();
                break;
              }
            }
          }
          else
            ReshuffleSuccess();
        }
        else
          ReshuffleLetter();
      }
      else
        ReshuffleLetter();
    }
    else
      ReshuffleLetter();
  }

  void ReshuffleSuccess()
  {
    for (int j = 0; j < letterTiles.Count; j++)
    {
      toShuffle[j].SetSiblingIndex(shuffledIndex[j]);

      if (toShuffle[j].GetComponent<Button>().interactable)
      {
        currentLetterText = toShuffle[j].GetComponentInChildren<Text>();
        currentLetterText.text = toShuffle[j].name;
        prevShuffledLetter = shuffledLetter;
      }
    }

    disablePress = false;
  }

  public void RemoveLetter(int index)
  {
    if (!disablePress)
    {
      gameAudio.PlayOneShot(letterPressed);

      if (lettersInputTiles[index] != null)
      {
        // Show hidden letter tile
        lettersInputTiles[index].GetComponent<Image>().color = new Color32(255, 255, 255, 255);
        lettersInputTiles[index].interactable = true;

        // Text (letter tile)
        currentLetterText = lettersInputTiles[index].GetComponentInChildren<Text>();
        currentLetterText.text = lettersInputTiles[index].name;

        // Text (word tile)
        currentLetterText = wordTiles[index].GetComponentInChildren<Text>();
        currentLetterText.text = String.Empty;

        // Remove from array
        lettersInputTiles[index] = null;
        lettersInput[index] = String.Empty;

        LetterPlacementCheck();
      }
    }
  }

  public AudioClip letterPressed;
  AudioSource gameAudio;
  public AudioSource tickingAudio;
  public String currentPressedWord;

  public AudioClip wrongWord;
  public AudioClip correctWord;
  public AudioClip anagramWord;

  public AnimationClip shake;
  public bool disablePress;
  public List<String> usedAnagram = new List<string>();
  bool isAnagram;
  public int anagramModifier;
  public void LetterPress(Button letterButton)
  {
    if (!disablePress)
    {
      gameAudio.PlayOneShot(letterPressed);

      // Hide
      letterButton.GetComponent<Image>().color = Color.clear;
      letterButton.interactable = false;

      // Text
      currentLetterText = letterButton.GetComponentInChildren<Text>();
      currentLetterText.text = String.Empty;

      // Letter place
      Text currentWordText = wordTiles[currentLetterFill].GetComponentInChildren<Text>();
      currentWordText.text = letterButton.name;

      lettersInput[currentLetterFill] = letterButton.name;

      lettersInputTiles[currentLetterFill] = letterButton;
      LetterPlacementCheck();

      currentPressedWord = String.Empty;
      foreach (String letter in lettersInput)
      {
        currentPressedWord += letter;
      }

      // Check if all filled
      if (currentPressedWord.Length == current_wordSize)
      {
        if (currentLetterTileCadet != null)
          Destroy(currentLetterTileCadet.gameObject);

        // Check correct word
        if (currentPressedWord.Equals(current_word))
        {
          tickingAudio.Stop();
          gameAudio.PlayOneShot(correctWord);
          if (currentTurn == 0)
          {
            DoDamage(1);
          }
          else
          {
            battleCanvas.SetTrigger("Shield");
            DoDamage(0);
          }
        }
        else
        {
          // Check anagram
          if (anagram_word.Count > 0)
          {
            if (anagram_word.Contains(currentPressedWord))
            {
              if (!usedAnagram.Contains(currentPressedWord))
              {
                // Accepted anagram (no duplicate)
                gameAudio.PlayOneShot(anagramWord);
                disablePress = true;
                usedAnagram.Add(currentPressedWord);

                for (int j = 0; j < wordTiles.Count; j++)
                {
                  // Color yellow tiles
                  wordTiles[j].GetComponent<Image>().color = new Color32(255, 255, 0, 255);
                }

                noAnimationReset = true;
                StartCoroutine(ResetWordTiles(0.5f));
                if (currentTurn == 0)
                {
                  battleCanvas.SetTrigger("SmallAttack");
                  isAnagram = true;
                  DoDamage(0.3f);
                }
                else
                {
                  anagramModifier++;
                  battleCanvas.SetTrigger("UpShield");
                }
              }
              else
              {
                for (int j = 0; j < wordTiles.Count; j++)
                {
                  // Color blue tiles
                  wordTiles[j].GetComponent<Image>().color = new Color32(255, 0, 255, 255);
                }
                WrongWord();
                tickingAudio.Stop();
              }
            }
            else
            {
              for (int k = 0; k < wordTiles.Count; k++)
              {
                // Color red tiles
                wordTiles[k].GetComponent<Image>().color = new Color32(255, 0, 0, 255);
              }
              WrongWord();
              tickingAudio.Stop();
              if (currentTurn == 1)
              {
                DoDamage(1);
              }
              else
              {
                StartCoroutine(WrongReset());
              }
            }
          }
          else
          {
            for (int i = 0; i < wordTiles.Count; i++)
            {
              // Color red tiles
              wordTiles[i].GetComponent<Image>().color = new Color32(255, 0, 0, 255);
            }

            WrongWord();
            if (currentTurn == 1)
            {
              DoDamage(1);
            }
            else
            {

              StartCoroutine(WrongReset());
            }
          }
        }
      }
    }
  }

  IEnumerator WrongReset()
  {
    yield return new WaitForSeconds(shake.length);
    DoDamage(0);
  }

  void WrongWord()
  {
    gameAudio.PlayOneShot(wrongWord);
    wordTilesPool_Animator.SetBool("reset", false);
    wordTilesPool_Animator.SetBool("wrong", true);

    disablePress = true;
    StartCoroutine(ResetWordTiles(shake.length));
  }

  public void RemoveWordTiles()
  {
    if (!disablePress)
    {
      StartCoroutine(ResetWordTiles(0));
    }
  }

  bool noAnimationReset;
  IEnumerator ResetWordTiles(float delay)
  {
    yield return new WaitForSeconds(delay);
    if (!noAnimationReset)
    {
      wordTilesPool_Animator.SetBool("reset", true);
      wordTilesPool_Animator.SetBool("wrong", false);
    }
    noAnimationReset = false;

    // Reset color
    for (int i = 0; i < wordTiles.Count; i++)
    {
      // Show hidden letter tile
      wordTiles[i].GetComponent<Image>().color = new Color32(255, 255, 255, 255);
    }

    // Reset tiles
    for (int i = 0; i < lettersInputTiles.Length; i++)
    {
      if (lettersInputTiles[i] != null)
      {
        // Show hidden letter tile
        lettersInputTiles[i].GetComponent<Image>().color = new Color32(255, 255, 255, 255);
        lettersInputTiles[i].interactable = true;

        // Text (letter tile)
        currentLetterText = lettersInputTiles[i].GetComponentInChildren<Text>();
        currentLetterText.text = lettersInputTiles[i].name;

        // Text (word tile)
        currentLetterText = wordTiles[i].GetComponentInChildren<Text>();
        currentLetterText.text = String.Empty;

        // Remove from array
        lettersInputTiles[i] = null;
        lettersInput[i] = String.Empty;
      }
    }

    LetterPlacementCheck();
    disablePress = false;
  }

  public String[] lettersInput;
  public Button[] lettersInputTiles;
  public Transform cadet;
  public int currentLetterFill;
  Transform currentLetterTileCadet;
  public void LetterPlacementCheck()
  {
    // Check empty
    for (int i = 0; i < lettersInput.Length; i++)
    {
      if (lettersInput[i] == null || lettersInput[i] == String.Empty)
      {
        if (currentLetterTileCadet != null)
          Destroy(currentLetterTileCadet.gameObject);

        currentLetterTileCadet = Instantiate(cadet).GetComponent<Transform>();
        currentLetterFill = i;
        currentLetterTileCadet.SetParent(wordTiles[i]);
        currentLetterTileCadet.localPosition = new Vector3(0, 0, 0);
        currentLetterTileCadet.localScale = new Vector3(1, 1, 1);
        break;
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
          enemyHPBar.localScale = new Vector3(enemyHPBar.localScale.x, enemyHP, enemyHPBar.localScale.z);
          currentEnemyHP = enemyHP;
        }
        else
        {
          hpBarAnimation = false;
          if (!isAnagram)
          {
            currentTurn = (currentTurn == 0) ? 1 : 0;
            ShowWord();
          }
          isAnagram = false;
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
    else
    {
      playerHPText = currentPlayerHP * playerMaxHP;
      playerHPBar.parent.GetComponentInChildren<Text>().text = playerHPText.ToString("N2");
      enemyHPText = currentEnemyHP * enemyMaxHP;
      enemyHPBar.parent.GetComponentInChildren<Text>().text = enemyHPText.ToString("N2");

      if (turnLeft < 0 || currentPlayerHP <= 0 || currentEnemyHP <= 0)
      {
        StopAllCoroutines();

        timerText.text = "0";
        moveCounterText.text = "Move Left: 0";

        // Lose
        if (currentTurn < 0 || currentPlayerHP <= 0)
          winLose = false;

        // Win
        if (currentEnemyHP <= 0)
          winLose = true;

        if (currentEnemyHP < 0)
        {
          currentEnemyHP = 0;
          enemyHPText = currentEnemyHP * enemyMaxHP;
          enemyHPBar.parent.GetComponentInChildren<Text>().text = enemyHPText.ToString("N2");
        }

        if (currentPlayerHP < 0)
        {
          currentPlayerHP = 0;
          playerHPText = currentPlayerHP * playerMaxHP;
          playerHPBar.parent.GetComponentInChildren<Text>().text = playerHPText.ToString("N2");
        }

        GameOver();
      }
    }
  }

  public int currentTurn = 0; // 0 - Player, 1 - Enemy    
  float currentPlayerHP;
  public float currentEnemyHP;
  public void DoDamage(float modifier)
  {
    // Animation (normal attack)
    if (modifier == 1)
    {
      if (currentTurn == 0)
      {
        battleCanvas.SetTrigger("BigAttack");
      }
    }

    if (modifier > 0 && currentTurn == 1)
    {
      battleCanvas.SetTrigger("EnemyAttack");
    }

    float anagramDefence = playerDamage - (anagramModifier * 0.1f);

    realPlayerDamage = (playerDamage * modifier) / enemyMaxHP;
    realEnemyDamage = ((enemyDamage * modifier) / playerMaxHP) * anagramDefence;

    hpBarAnimation = true;
    if (currentTurn == 0) // Deduct enemy HP (player attack)
    {
      targetHP = currentEnemyHP - realPlayerDamage;
    }
    else
    {
      targetHP = currentPlayerHP - realEnemyDamage;
    }
  }

  float playerMaxHP = 3;
  float playerDamage = 1;
  public float realPlayerDamage;
  float enemyMaxHP = 3;
  float enemyDamage = 1;
  float realEnemyDamage;

  float playerHPText;
  float enemyHPText;
  private void CalculateHealth()
  {
    enemyMaxHP += ((currentWorld * currentLevel) * 0.5f);

    currentPlayerHP = 1;
    currentEnemyHP = 1;

    playerHPText = currentPlayerHP * playerMaxHP;
    playerHPBar.parent.GetComponentInChildren<Text>().text = playerHPText.ToString("N2");
    enemyHPText = currentEnemyHP * enemyMaxHP;
    enemyHPBar.parent.GetComponentInChildren<Text>().text = enemyHPText.ToString("N2");
  }

  public Transform levelUI;
  bool winLose;

  public AudioClip oneStar;
  public AudioClip twoStar;
  public AudioClip threeStar;

  public AudioClip winSFX;
  public AudioClip loseSFX;

  public AudioSource bgSoundSource;
  bool loseOnce;
  public int star = 1;

  void GameOver()
  {
    if (!loseOnce)
    {
      loseOnce = true;
      levelUI.gameObject.SetActive(true);
      bgSoundSource.Stop();
      tickingAudio.Stop();

      Text winLoseText = levelUI.GetChild(2).GetComponent<Text>();
      if (!winLose)
      {
        winLoseText.text = "LOSE";
        gameAudio.PlayOneShot(loseSFX);

      }
      else
      {
        winLoseText.text = "WIN";
        gameAudio.PlayOneShot(winSFX);
      }

      Text timer = levelUI.GetChild(6).GetComponent<Text>();
      String minuteTimer = (totalTimer / 60).ToString();
      String secondTimer = (totalTimer % 60).ToString();

      // Calculate star   
      if ((currentPlayerHP * playerMaxHP) >= (playerMaxHP / 2))
      {
        star++;
      }

      if ((currentPlayerHP * playerMaxHP) == playerMaxHP)
      {
        star++;
      }

      if (winLose)
      {
        if (currentStar < star)
        {
          InvokeRepeating("StarResult", 1.5f, 1f);
        }
      }

      if (minuteTimer.Length == 1)
      {
        minuteTimer = "0" + minuteTimer;
      }

      if (secondTimer.Length == 1)
      {
        secondTimer = "0" + secondTimer;
      }

      timer.text = minuteTimer + ":" + secondTimer;

      // Save
      if (winLose && star > prevStar)
      {
        WorldMapManager.SaveData(currentWorld, currentLevel, star, minuteTimer, secondTimer);
      }
    }
  }

  int currentStar;
  void StarResult()
  {
    currentStar++;

    levelUI.GetChild(2 + currentStar).GetChild(0).gameObject.SetActive(true);
    if (currentStar == 1)
      gameAudio.PlayOneShot(oneStar);
    else if (currentStar == 2)
      gameAudio.PlayOneShot(twoStar);
    else if (currentStar == 3)
      gameAudio.PlayOneShot(threeStar);

    if (currentStar >= star)
      CancelInvoke("StarResult");
  }

  public void Replay()
  {
    loadingManager.LoadTransition(0);
    loadingManager.NextScene(SceneManager.GetActiveScene().name);
  }

  public void QuitToMenu()
  {
    loadingManager.LoadTransition(0);
    loadingManager.NextScene("WorldMap");
  }

  int totalTimer;
  public AudioClip tickingSound;
  public AudioClip timesUp;
  IEnumerator timer()
  {
    yield return new WaitForSeconds(1);
    timerText.text = (--curTimer).ToString();
    totalTimer++;
    if (curTimer == 5)
    {
      tickingAudio.PlayOneShot(tickingSound);
    }

    if (curTimer > 0)
      StartCoroutine(timer());
    else
    {
      if (currentTurn != 0)
      {
        DoDamage(1);
      }
      else
      {
        gameAudio.PlayOneShot(timesUp);
        currentTurn = (currentTurn == 0) ? 1 : 0;
        turnLeft--;
        ShowWord();
      }
    }
  }


  public AudioClip punch;
  public AudioClip enemyAttack;
  public AudioClip playerShield;
  public AudioClip playerShieldUp;
  public void BattleSound(int audioIndex)
  {
    switch (audioIndex)
    {
      case 0: gameAudio.PlayOneShot(punch); break;
      case 1: gameAudio.PlayOneShot(enemyAttack); break;
      case 2: gameAudio.PlayOneShot(playerShield); break;
      case 3: gameAudio.PlayOneShot(playerShieldUp); break;
    }
  }

  public GameObject pauseUI;
  public void Pause()
  {
    pauseUI.SetActive(true);
    Time.timeScale = 0;

    gameAudio.Pause();
    tickingAudio.Pause();
    bgSoundSource.Pause();
  }

  public void UnPause()
  {
    pauseUI.SetActive(false);
    Time.timeScale = 1;

    gameAudio.UnPause();
    tickingAudio.UnPause();
    bgSoundSource.UnPause();

  }
}
