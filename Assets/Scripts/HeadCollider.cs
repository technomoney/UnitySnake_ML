using System;
using System.Linq;
using UnityEngine;

public class HeadCollider : MonoBehaviour
{
    //we need a ref to the snake so we can make him grow big and strong, assigned in inspector
    public Snake m_snake;
    private void OnTriggerEnter(Collider other)
    {
        if (!other.tag.Equals("Food")) return;

        m_snake.m_growBiggerAfterMovement = true;
    }
}
