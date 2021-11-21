using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Easy helper class for the UI and score
/// </summary>
public class GameManager : MonoBehaviour
{
    public Snake snake;
    public int Score;
    public static GameManager Inst;

    public TextMeshProUGUI m_text_score, m_text_banner;

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

        //we could lock this behind the gameover screen, but this is handy for testing...
        if (Input.GetKeyDown(KeyCode.P))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    public void ShowGameOver()
    {
        m_text_banner.text = " Game Over! \nP to restart";
        snake.m_isStopped = true;
    }
}
