using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Text.RegularExpressions;
using UnityEngine.SceneManagement;
public class Cutscene : MonoBehaviour
{
    // Cutscene world 1
    public TextAsset cutsceneTest;
    public static TextAsset currentCutscene;

    public static int[] cutSceneData;
    Text dialogueName;
    Text dialogueText;
    string dialogueTextData;
    [TextArea]
    string[] dialogueList;
    int currentDialogue;

    public AnimationClip loadTimeReference;

    private void Start()
    {
        if (currentCutscene == null)
            currentCutscene = cutsceneTest;

        StartCoroutine(WaitAndLoadCutscene());
    }

    private IEnumerator WaitAndLoadCutscene()
    {
        yield return new WaitForSeconds(loadTimeReference.length);
        LoadCutscene();
    }

    protected internal void LoadCutscene()
    {
        dialogueName = GameObject.FindGameObjectWithTag("DialogueName").GetComponent<Text>();
        dialogueText = GameObject.FindGameObjectWithTag("DialogueText").GetComponent<Text>();

        dialogueTextData = currentCutscene.text;
        dialogueTextData = Regex.Replace(dialogueTextData, @"\t|\n|\r", "");

        dialogueList = dialogueTextData.Split(new char[] { ';' }, StringSplitOptions.None);

        currentDialogue = 0;
        StartCoroutine(ShowCutscene(currentDialogue));
    }

    string[] dialogueData;
    float speed = 0.01f;
    bool stop;
    private IEnumerator ShowCutscene(int DialogueIndex)
    {
        if (currentDialogue >= dialogueList.Length)
        {
            SceneManager.LoadScene(3);
        }
        else
        {
            dialogueData = dialogueList[DialogueIndex].Split(new char[] { '|' }, StringSplitOptions.None);
            // [0] = Name
            // [1] = Dialogue

            dialogueName.text = dialogueData[0];

            foreach (char dialogueChar in dialogueData[1])
            {
                dialogueText.text = dialogueText.text += dialogueChar;
                yield return new WaitForSeconds(speed);

                if (stop)
                    break;
            }
        }
    }

    int time;
    public void Next()
    {
        if (dialogueText.text == dialogueData[1])
        {
            stop = false;
            currentDialogue++;
            dialogueName.text = "";
            dialogueText.text = "";
            StartCoroutine(ShowCutscene(currentDialogue));
        }
        else
        {
            stop = true;
            dialogueText.text = dialogueData[1];
        }
    }
}
