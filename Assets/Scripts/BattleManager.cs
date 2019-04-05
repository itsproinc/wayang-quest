using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class BattleManager : MonoBehaviour
{
    TextInterpreter questionInterpreter;
    public float targetHP;
    public float enemyHealth;
    public float enemyHealthLoss;
    public float playerHealth;
    public float playerHealthLoss;
    public Transform playerHPBar;
    public Transform enemyHPBar;
    float maxEnemyHP = 3f;
    float maxPlayerHP = 3f;
    public AudioSource sfx;
    public AudioClip punch;
    public AudioClip slashPrajurit;
    private void Start()
    {
        questionInterpreter = GetComponent<TextInterpreter>();

        playerHPBar = GameObject.FindGameObjectWithTag("PlayerHP").GetComponent<Transform>();
        enemyHPBar = GameObject.FindGameObjectWithTag("EnemyHP").GetComponent<Transform>();
        sfx = GameObject.FindGameObjectWithTag("SFX").GetComponent<AudioSource>();

        ResetHP();
    }

    protected internal void ResetHP()
    {
        playerHealth = 1;
        enemyHealth = 1;
        enemyHealthLoss = 0;
    }

    protected internal void Battle(int side)
    {
        switch (side)
        {
            case 0: // Deduct player
                updatePlayerHP = true;
                playerHealthLoss++;
                sfx.PlayOneShot(slashPrajurit);
                break;

            case 1: // Deduct enemy
                updateEnemyHP = true;
                enemyHealthLoss++;
                sfx.PlayOneShot(punch);
                break;
        }

        // Detect if player health is 0
        if (playerHealth <= 0)
            GameOver();

        // Detect if enemy health is 0
        if (enemyHealth <= 0)
            Win();
    }

    bool updateEnemyHP;
    bool updatePlayerHP;
    private void Update()
    {
        if (updateEnemyHP)
        {
            float divider = 1f / maxEnemyHP;
            targetHP = 1f - (enemyHealthLoss * divider);
            if (enemyHealth > targetHP)
            {
                enemyHealth -= Time.deltaTime;
                enemyHPBar.localScale = new Vector3(enemyHealth, enemyHPBar.localScale.y, enemyHPBar.localScale.z);
            }
            else
            {
                updateEnemyHP = false;
            }
        }
        else
        {
            if (enemyHealth < 0)
            {
                enemyHealth = 0;
                enemyHPBar.localScale = new Vector3(enemyHealth, enemyHPBar.localScale.y, enemyHPBar.localScale.z);
            }
        }

        if (updatePlayerHP)
        {
            float divider = 1f / maxPlayerHP;
            targetHP = 1f - (playerHealthLoss * divider);
            if (playerHealth > targetHP)
            {
                playerHealth -= Time.deltaTime;
                playerHPBar.localScale = new Vector3(playerHealth, playerHPBar.localScale.y, playerHPBar.localScale.z);
            }
            else
            {
                updatePlayerHP = false;
            }
        }
        else
        {
            if (playerHealth < 0)
            {
                playerHealth = 0;
                playerHPBar.localScale = new Vector3(playerHealth, enemyHPBar.localScale.y, enemyHPBar.localScale.z);
            }
        }
    }

    protected internal void GameOver()
    {
        Debug.Log("GameOver");
    }

    protected internal void Win()
    {

    }
}
