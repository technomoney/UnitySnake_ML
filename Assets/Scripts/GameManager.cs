using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public Snake snake;
    public static int Score;

    public TextMeshProUGUI m_text;
    // Start is called before the first frame update
    void Start()
    {
        m_text.text = string.Empty;
        Score = 0;
    }

    // Update is called once per frame
    void Update()
    {
        //update our testing info

        m_text.text = "Bearing: " + snake.m_movementBearing + "("+snake.m_movementBearing* Mathf.Rad2Deg+")"+"\n" +
                      "Head: " + snake.m_list_bodyParts[0].localRotation.eulerAngles.y+"\n"+
                      "Score: " + Score;
    }
}
