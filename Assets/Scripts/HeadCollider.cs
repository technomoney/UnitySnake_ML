using System;
using System.Linq;
using UnityEngine;

public class HeadCollider : MonoBehaviour
{
    //we need a ref to the snake so we can make him grow big and strong, assigned in inspector
    public Snake m_snake;
    //we'll need to move the food after we eat it...
    public Food m_food;
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals("Food"))
        {
            m_snake.m_growBiggerAfterMovement = true;
            m_food.Respawn();
            return;
        }

        if (other.tag.Equals("Body"))
        {
            Debug.Log("gg");
            GameManager.Inst.ShowGameOver();
        }
        
    }
}
