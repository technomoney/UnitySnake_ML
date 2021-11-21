using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public Snake snake;
    public int Score;
    public static GameManager Inst;

    public TextMeshProUGUI m_text_score, m_text_banner;
    // Start is called before the first frame update
    void Start()
    {
        Inst = this;
        m_text_score.text = string.Empty;
        Score = 0;
    }

    // Update is called once per frame
    void Update()
    {
        m_text_score.text = "Score: " + Score;
    }

    public void ShowGameOver()
    {
        m_text_banner.text = " Game Over! \nR to restart";
        snake.m_isStopped = true;
    }
}
