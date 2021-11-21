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
    
    //not a pure singleton, but a self referring static instance to the gamemanger,
    //even though we only call this once, I like this pattern for gm-type 
    //logic
    public static GameManager Inst;

    //text fields for our basic info
    public TextMeshProUGUI m_text_score, m_text_banner;

    void Start()
    {
        //set the reference 
        Inst = this;
        m_text_score.text = string.Empty;
        Score = 0;
    }
    
    void Update()
    {
        //probably don't really need to update every frame,... but no bog deal
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
