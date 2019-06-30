using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO;

public class WorldMapManager : MonoBehaviour
{
  public TextAsset WordBank1;
  public TextAsset WordBank2;
  public TextAsset WordBank3;
  public TextAsset WordBank4;
  public TextAsset WordBank5;

  public List<TextAsset> wordBank = new List<TextAsset>();

  LoadingManager loadingManager;
  protected internal static int currentWorld = 0;

  private void Reset()
  {
    levelInfoUI = GameObject.FindGameObjectWithTag("LevelInfo");
  }

  private void Start()
  {
    wordBank.Add(WordBank1);
    wordBank.Add(WordBank2);
    wordBank.Add(WordBank3);
    wordBank.Add(WordBank4);
    wordBank.Add(WordBank5);

    loadingManager = GameObject.FindGameObjectWithTag("Loading").GetComponent<LoadingManager>();

    LoadWorldMap();
  }

  int levelAmount = 5;

  public GameObject level;
  Transform curLevelList;
  List<Transform> levelUI = new List<Transform>();
  public List<LevelData> levelDataPool = new List<LevelData>();
  public void LoadWorldMap()
  {
    levelUI.Clear();

    curLevelList = GameObject.FindGameObjectWithTag("LevelList").GetComponent<Transform>();
    for (int i = 0; i < levelAmount; i++)
    {
      recentlySavedData = null;
      levelUI.Add(Instantiate(level).GetComponent<Transform>());
      levelUI[i].transform.SetParent(curLevelList);

      // Set position
      levelUI[i].localPosition = new Vector3(200 + (i * 400), curLevelList.position.y, curLevelList.position.z);
      levelUI[i].localRotation = curLevelList.localRotation;
      levelUI[i].localScale = new Vector3(1, 1, 1);
      levelUI[i].GetChild(0).GetComponentInChildren<Text>().text = (i + 1).ToString();
      levelUI[i].name = i.ToString();

      // Star
      LevelData currentLevelData = LoadData(currentWorld, i);

      if (currentLevelData == null)
      {
        currentLevelData = recentlySavedData;
        levelDataPool.Add(recentlySavedData);
      }
      else
      {
        levelDataPool.Add(currentLevelData);
      }


      int starAmount = currentLevelData.star;

      for (int j = 0; j < starAmount; j++)
      {
        levelUI[i].GetChild(1 + j).GetChild(0).gameObject.SetActive(true);
      }

      if (!currentLevelData.unlock)
      {
        levelUI[i].GetChild(4).gameObject.SetActive(true);
      }
    }

    for (int i = 0; i < levelUI.Count; i++)
    {
      int levelCount = i;
      // Add button listener
      Button button = levelUI[i].GetComponent<Button>();
      int cLevel = int.Parse(levelUI[i].name);

      // LoadWordBank(cLevel)]
      button.onClick.AddListener(() => OpenLevelInfo(levelDataPool[levelCount]));
    }
  }

  private LevelData LoadData(int World, int Level)
  {
    string fileLink = Application.persistentDataPath + "/Save/World" + World + "/Level" + Level + ".json";
    if (!File.Exists(fileLink))
    {
      NewData(World, Level);
      return null;
    }
    else
    {
      LevelData currentData = JsonUtility.FromJson<LevelData>(File.ReadAllText(fileLink));
      return currentData;
    }
  }

  LevelData recentlySavedData;
  private void NewData(int World, int Level)
  {
    string fileLink = Application.persistentDataPath + "/Save/World" + World + "/Level" + Level + ".json";
    // Check folder
    if (!Directory.Exists(Application.persistentDataPath + "/Save"))
      Directory.CreateDirectory(Application.persistentDataPath + "/Save");

    if (!Directory.Exists(Application.persistentDataPath + "/Save/World" + World))
      Directory.CreateDirectory(Application.persistentDataPath + "/Save/World" + World);

    // Create file
    LevelData levelDataClass = new LevelData();
    levelDataClass.currentWorld = currentWorld + 1;
    levelDataClass.currentLevel = Level + 1;
    levelDataClass.star = 0;
    levelDataClass.minuteTaken = "00";
    levelDataClass.secondTaken = "00";
    if (Level == 0)
      levelDataClass.unlock = true;
    else
      levelDataClass.unlock = false;

    StreamWriter sw = File.CreateText(fileLink);
    sw.Close();

    string json = JsonUtility.ToJson(levelDataClass, true);
    File.WriteAllText(fileLink, json);

    recentlySavedData = levelDataClass;
  }

  protected internal static void SaveData(int World, int Level, int star, String minuteTaken, String secondTaken)
  {
    int curWorld = World - 1;
    int curLevel = Level - 1;

    string fileLink = Application.persistentDataPath + "/Save/World" + curWorld + "/Level" + curLevel + ".json";
    File.Delete(fileLink);
    // Check folder
    if (!Directory.Exists(Application.persistentDataPath + "/Save"))
      Directory.CreateDirectory(Application.persistentDataPath + "/Save");

    if (!Directory.Exists(Application.persistentDataPath + "/Save/World" + curWorld))
      Directory.CreateDirectory(Application.persistentDataPath + "/Save/World" + curWorld);

    // Create file
    LevelData levelDataClass = new LevelData();
    levelDataClass.currentWorld = curWorld + 1;
    levelDataClass.currentLevel = curLevel + 1;
    levelDataClass.star = star;
    levelDataClass.minuteTaken = minuteTaken;
    levelDataClass.secondTaken = secondTaken;
    levelDataClass.unlock = true;

    StreamWriter sw = File.CreateText(fileLink);
    sw.Close();

    string json = JsonUtility.ToJson(levelDataClass, true);
    File.WriteAllText(fileLink, json);

    int NextLevel = curLevel + 1;
    UnlockNextLevel(curWorld, NextLevel);
  }

  static LevelData currentNextData;
  static void UnlockNextLevel(int World, int Level)
  {
    int curLevel = Level - 1;

    if (curLevel != 5)
    {
      string nextFileLink = Application.persistentDataPath + "/Save/World" + World + "/Level" + Level + ".json";
      Debug.Log(nextFileLink);
      if (File.Exists(nextFileLink))
      {
        Debug.Log("exists");
        currentNextData = JsonUtility.FromJson<LevelData>(File.ReadAllText(nextFileLink));
      }
    }

    string fileLink = Application.persistentDataPath + "/Save/World" + World + "/Level" + Level + ".json";
    File.Delete(fileLink);

    // Check folder
    if (!Directory.Exists(Application.persistentDataPath + "/Save"))
      Directory.CreateDirectory(Application.persistentDataPath + "/Save");

    if (!Directory.Exists(Application.persistentDataPath + "/Save/World" + World))
      Directory.CreateDirectory(Application.persistentDataPath + "/Save/World" + World);

    // Create file
    LevelData levelDataClass = new LevelData();
    levelDataClass.currentWorld = (currentNextData == null) ? currentWorld + 1 : currentNextData.currentWorld;
    levelDataClass.currentLevel = (currentNextData == null) ? Level + 1 : currentNextData.currentLevel;
    levelDataClass.star = (currentNextData == null) ? 0 : currentNextData.star;
    levelDataClass.minuteTaken = (currentNextData == null) ? "00" : currentNextData.minuteTaken;
    levelDataClass.secondTaken = (currentNextData == null) ? "00" : currentNextData.secondTaken;
    levelDataClass.unlock = true;

    StreamWriter sw = File.CreateText(fileLink);
    sw.Close();

    string json = JsonUtility.ToJson(levelDataClass, true);
    File.WriteAllText(fileLink, json);
  }

  public GameObject levelInfoUI;
  private void OpenLevelInfo(LevelData levelInfo)
  {
    if (levelInfo.unlock)
    {
      // Reset
      for (int i = 0; i < 3; i++)
      {
        levelInfoUI.transform.GetChild(2 + i).GetChild(0).gameObject.SetActive(false);
      }
      levelInfoUI.transform.GetChild(5).GetComponent<Text>().text = "00:00";


      levelInfoUI.SetActive(true);

      // Level text
      levelInfoUI.transform.GetChild(1).GetComponent<Text>().text = "Level " + levelInfo.currentWorld + "-" + levelInfo.currentLevel;

      // Star
      for (int i = 0; i < levelInfo.star; i++)
      {
        levelInfoUI.transform.GetChild(2 + i).GetChild(0).gameObject.SetActive(true);
      }

      // Timer
      levelInfoUI.transform.GetChild(5).GetComponent<Text>().text = levelInfo.minuteTaken + ":" + levelInfo.secondTaken;

      BattleManager.currentWord = wordBank[levelInfo.currentLevel - 1];
      BattleManager.currentWorld = levelInfo.currentWorld;
      BattleManager.currentLevel = levelInfo.currentLevel;
      BattleManager.prevStar = levelInfo.star;

      // Button
      Button button = levelInfoUI.transform.GetChild(6).GetComponent<Button>();
      button.onClick.AddListener(() => loadingManager.LoadTransition(0));
      button.onClick.AddListener(() => loadingManager.NextScene("Game"));
    }
  }

  public void CloseLevelInfo()
  {
    levelInfoUI.SetActive(false);
  }

  public void QuitGame()
  {
    Application.Quit();
  }

  [Serializable]
  public class LevelData
  {
    public int currentWorld;
    public int currentLevel;
    public int star;
    public string secondTaken;
    public string minuteTaken;
    public bool unlock;
  }
}
