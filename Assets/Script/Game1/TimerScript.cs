using TMPro;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

public class TimerScript : MonoBehaviour
{
    public static float TimeLeft;
    public static bool TimerOn = false;
    public TextMeshProUGUI TimerText;
    public GameObject gameOver;
    public GameObject pointsCanvas;
    public GameObject timerCanvas;
    public TextMeshProUGUI ScoreText;
    public TextMeshProUGUI RecordText;
    public GameObject NewRecordText;

    private void Start()
    {
        TimeLeft = 15;
    }

    // Update is called once per frame
    void Update()
    {
        if(TimerOn)
        {
            if(TimeLeft > 0)
            {
                TimeLeft -= Time.deltaTime;
                updateTimer(TimeLeft);
            }
            else
            {
                TimeLeft = 0;
                TimerText.text = string.Format("{00}", TimeLeft);
                TimerOn = false;
                onGameOver();
            }
        }
    }

    void updateTimer(float currentTime)
    {
        currentTime++;
        float seconds = Mathf.FloorToInt(currentTime);

        TimerText.text = string.Format("{00}", seconds);
    }

    void onGameOver()
    {
        Game1Controller.isGameOver = true;
        if (Game1Controller.score > ChoseFlower.record)
        {
            if(ChoseFlower.FlowerChosen == 0)
            {
                Game1Controller.record0 = Game1Controller.score;
                PlayerPrefs.SetInt("RecordGame1.0", Game1Controller.record0);
                RecordText.text = Game1Controller.record0.ToString();
            } else
            {
                Game1Controller.record1 = Game1Controller.score;
                PlayerPrefs.SetInt("RecordGame1.1", Game1Controller.record1);
                RecordText.text = Game1Controller.record1.ToString();
            }
            RecordText.color = Color.yellow;
            NewRecordText.SetActive(true);

        }
        else
        {
            RecordText.text = ChoseFlower.FlowerChosen == 0 ? Game1Controller.record0.ToString() : Game1Controller.record1.ToString();
        }
        ScoreText.text = Game1Controller.score.ToString();
        gameOver.SetActive(true);
        pointsCanvas.SetActive(false);
        timerCanvas.SetActive(false);
    }
}
