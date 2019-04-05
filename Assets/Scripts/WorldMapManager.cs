using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO;

public class WorldMapManager : MonoBehaviour
{
    TextManager textManager;
    LoadingManager loadingManager;
    public GameObject[] world;
    public GameObject[] levelList;
    public static int currentWorld = 0;

    private void Reset()
    {
        world = GameObject.FindGameObjectsWithTag("World");
        levelList = GameObject.FindGameObjectsWithTag("LevelList");
    }

    private void Start()
    {
        loadingManager = GameObject.FindGameObjectWithTag("Loading").GetComponent<LoadingManager>();
    }

    int levelAmount;
    private void OnEnable()
    {
        textManager = GetComponent<TextManager>();

        switch (currentWorld)
        {
            case 0:
                levelAmount = textManager.w1_questionnaire.Length;
                break;
        }

        LoadWorldMap();
    }

    public GameObject level;
    Transform curLevelList;
    List<Transform> levelUI = new List<Transform>();
    public void LoadWorldMap()
    {
        levelUI.Clear();

        curLevelList = GameObject.FindGameObjectWithTag("LevelList").GetComponent<Transform>();
        for (int i = 0; i < levelAmount; i++)
        {
            levelUI.Add(Instantiate(level).GetComponent<Transform>());
            levelUI[i].transform.SetParent(curLevelList);

            // Set position
            levelUI[i].localPosition = new Vector3(200 + (i * 400), curLevelList.position.y, curLevelList.position.z);
            levelUI[i].localRotation = curLevelList.localRotation;
            levelUI[i].localScale = new Vector3(1, 1, 1);
            levelUI[i].GetChild(0).GetComponent<Text>().text = (i + 1).ToString();
            levelUI[i].name = "Level " + (i + 1);
        }

        int levelCounter = 0;
        foreach (Transform levelUIButton in levelUI)
        {
            // Add button listener
            Button button = levelUIButton.GetComponent<Button>();
            string rawLevelData = currentWorld + "/" + levelCounter;
            button.onClick.AddListener(() => textManager.LoadLevel(rawLevelData));
            button.onClick.AddListener(() => loadingManager.LoadTransition(0));
            button.onClick.AddListener(() => loadingManager.NextScene("Cutscene"));

            // Load
            LevelData levelData = LoadData(currentWorld, levelCounter);
            if (levelData != null)
            {
                if (levelData.star >= 1)
                {
                    // 1 star
                    levelUIButton.GetChild(1).GetChild(0).gameObject.SetActive(true);

                    if (levelData.star >= 2)
                    {
                        // 2 stars
                        levelUIButton.GetChild(3).GetChild(0).gameObject.SetActive(true);

                        if (levelData.star >= 3)
                        {
                            // 3 stars
                            levelUIButton.GetChild(2).GetChild(0).gameObject.SetActive(true);
                        }
                    }
                }
            }

            levelCounter++;
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
            return JsonUtility.FromJson<LevelData>(File.ReadAllText(fileLink));
    }

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
        levelDataClass.star = 0;
        levelDataClass.timeTaken_minute = 0;
        levelDataClass.timeTaken_second = 0;

        StreamWriter sw = File.CreateText(fileLink);
        sw.Close();

        string json = JsonUtility.ToJson(levelDataClass, true);
        File.WriteAllText(fileLink, json);
    }

    [Serializable]
    private class LevelData
    {
        public int star;
        public int timeTaken_minute;
        public int timeTaken_second;
    }
}
