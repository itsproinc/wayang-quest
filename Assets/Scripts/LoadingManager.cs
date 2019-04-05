using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingManager : MonoBehaviour
{
    Animator animator;
    Canvas loadingCanvas;
    public AnimationClip loadingAnimation;
    public bool fadeoutAtStart; // Check to show tree fadeout effect

    private void OnEnable()
    {
        animator = GetComponent<Animator>();

        loadingCanvas = GameObject.FindGameObjectWithTag("LoadingCanvas").GetComponent<Canvas>();
        loadingCanvas.sortingOrder = -1;

        if (fadeoutAtStart)
        {
            LoadTransition(1);
        }
    }

    public void LoadTransition(int Type) // 0 - Fadein, 1 - Fadeout
    {
        loadingCanvas.sortingOrder = 99;

        if (Type == 0)
            animator.SetBool("Fadein", true);
        else
        {
            animator.SetBool("Fadeout", true);
            StartCoroutine(ResetTransition());
        }
    }

    public void NextScene(string SceneIndex)
    {
        StartCoroutine(LoadScene(SceneIndex));
    }

    private IEnumerator LoadScene(string SceneIndex)
    {
        yield return new WaitForSeconds(loadingAnimation.length);
        SceneManager.LoadScene(SceneIndex);
        StartCoroutine(ResetTransition());
    }

    private IEnumerator ResetTransition()
    {
        yield return new WaitForSeconds(loadingAnimation.length);
        animator.SetBool("Fadein", false);
        animator.SetBool("Fadeout", false);

        loadingCanvas.sortingOrder = -1;
        animator.SetBool("Reset", true);
        yield return new WaitForSeconds(0.1f);
        animator.SetBool("Reset", false);
    }
}
