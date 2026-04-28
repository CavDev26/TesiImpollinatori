using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class Game1Controller : MonoBehaviour
{
    public GameObject insettoL;
    public GameObject insettoR;
    float spawnTimerL;
    float spawnRateL;
    float spawnTimerR;
    float spawnRateR;

    public static bool isGameOver = true;

    public TextMeshProUGUI ScoreText;
    public static int score;
    public static int record0;
    public static int record1;

    // Start is called before the first frame update
    void Start()
    {
        spawnTimerL += Time.deltaTime;
        spawnInsettoL();
        spawnRateL = 1.6f;

        spawnTimerR += Time.deltaTime;
        spawnInsettoR();
        spawnRateR = 1.3f;

        score = 0;
        record0 = PlayerPrefs.GetInt("RecordGame1.0");
        record1 = PlayerPrefs.GetInt("RecordGame1.1");
    }

    // Update is called once per frame
    void Update()
    {
        UpdateScore();
        if(!isGameOver)
        {
            spawnTimerL += Time.deltaTime;
            spawnTimerR += Time.deltaTime;
            if (spawnTimerL >= spawnRateL)
            {
                spawnInsettoL();
            }
            if (spawnTimerR >= spawnRateR)
            {
                spawnInsettoR();
            }
        }
    }

    void spawnInsettoL()
    {
        spawnTimerL -= spawnRateL;
        Vector2 spawnPos = new Vector2(3.3f, Random.Range(-2f, 2.3f));
        Instantiate(insettoL, spawnPos, Quaternion.identity);
    }

    void spawnInsettoR()
    {
        spawnTimerR -= spawnRateR;
        Vector2 spawnPos = new Vector2(-3.3f, Random.Range(-2f, 2.3f));
        Instantiate(insettoR, spawnPos, Quaternion.identity);
    }

    private void UpdateScore()
    {
        ScoreText.text = score.ToString();
    }
}
