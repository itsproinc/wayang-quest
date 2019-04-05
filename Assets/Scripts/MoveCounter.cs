using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MoveCounter : MonoBehaviour
{
    public int moveLeft;
    public Text moveLeftText;
    int maxMove = 12;
    BattleManager battleManager;

    private void Start()
    {
        battleManager = GetComponent<BattleManager>();

        moveLeftText = GameObject.FindGameObjectWithTag("MoveCounterText").GetComponent<Text>();
        ResetMove();
    }

    protected internal void MinusMove()
    {
        moveLeft--;
        moveLeftText.text = moveLeft.ToString();

        if (moveLeft <= 0)
            battleManager.GameOver();
    }

    protected internal void ResetMove()
    {
        moveLeft = maxMove;
        moveLeftText.text = "Move left: " + moveLeft.ToString();
    }


}
