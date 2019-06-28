using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO;

public class WorldMapManager : MonoBehaviour
{
  public TextAsset WordBank1_1;

  public List<TextAsset> level1WordBank = new List<TextAsset>();
  public List<List<TextAsset>> wordBank = new List<List<TextAsset>>();

  LoadingManager loadingManager;
  protected internal static int currentWorld = 0;

  private void Reset()
  {
    levelInfoUI = GameObject.FindGameObjectWithTag("LevelInfo");
  }

  private void Start()
  {
    level1WordBank.Add(WordBank1_1);
    wordBank.Add(level1WordBank);

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
      SaveData(World, Level);
      return null;
    }
    else
    {
      LevelData currentData = JsonUtility.FromJson<LevelData>(File.ReadAllText(fileLink));
      return currentData;
    }
  }

  LevelData recentlySavedData;
  private void SaveData(int World, int Level)
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
    levelDataClass.timeTaken = 0;

    StreamWriter sw = File.CreateText(fileLink);
    sw.Close();

    string json = JsonUtility.ToJson(levelDataClass, true);
    File.WriteAllText(fileLink, json);

    recentlySavedData = levelDataClass;
  }

  public GameObject levelInfoUI;
  private void OpenLevelInfo(LevelData levelInfo)
  {
    levelInfoUI.SetActive(true);

    // Level text
    levelInfoUI.transform.GetChild(1).GetComponent<Text>().text = "Level " + levelInfo.currentWorld + "-" + levelInfo.currentLevel;

    // Star
    for (int i = 0; i < levelInfo.star; i++)
    {
      levelInfoUI.transform.GetChild(2 + i).gameObject.SetActive(true);
    }

    // Timer
    int timeTaken = levelInfo.timeTaken;
    levelInfoUI.transform.GetChild(5).GetComponent<Text>().text = timeTaken.ToString("N2");

    BattleManager.currentWord = wordBank[levelInfo.currentWorld - 1][levelInfo.currentLevel - 1];

    // Button
    Button button = levelInfoUI.transform.GetChild(6).GetComponent<Button>();
    button.onClick.AddListener(() => loadingManager.LoadTransition(0));
    button.onClick.AddListener(() => loadingManager.NextScene("Game"));
  }

  public void CloseLevelInfo()
  {
    levelInfoUI.SetActive(false);
  }

  [Serializable]
  public class LevelData
  {
    public int currentWorld;
    public int currentLevel;
    public int star;
    public int timeTaken;
  }
}
