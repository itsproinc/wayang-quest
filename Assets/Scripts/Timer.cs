using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    public int currentTimer;
    public Text timerText;
    int maxTimer = 20;
    TextInterpreter questionInterpreter;

    private void OnEnable()
    {
        timerText = GameObject.FindGameObjectWithTag("TimerText").GetComponent<Text>();
        questionInterpreter = GetComponent<TextInterpreter>();
    }

    protected internal void Stop()
    {
        StopAllCoroutines();
        currentTimer = 0;
    }

    protected internal void StartTimer()
    {
        currentTimer = maxTimer;
        timerText.text = currentTimer.ToString();
        StopAllCoroutines();
        StartCoroutine(RestartTimer());
    }

    private IEnumerator RestartTimer()
    {
        yield return new WaitForSeconds(1);
        currentTimer--;
        timerText.text = currentTimer.ToString();

        if (currentTimer > -1)
            StartCoroutine(RestartTimer());
        else
            questionInterpreter.CheckChoice(-1);
    }
}
