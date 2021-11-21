using UnityEngine;

/// <summary>
/// The collider for the head is on a chile object of the main 'head' object in the inspector, so we'll just give it a
/// little helper class here to deal with the collisions
/// </summary>
public class HeadCollider : MonoBehaviour
{
    //we need a ref to the snake so we can make him grow big and strong, assigned in inspector
    public Snake m_snake;
    //we'll need to move the food after we eat it...
    public Food m_food;
    
    /// <summary>
    /// we use trigger here instead of actual collision because we're moving the snake manually, not under the physics engine 
    /// further, it's easier to use trigger since we're just plopping each body part where it's supposed to go.
    /// Only the head and the food have rigidbodies and they're both kinetic, which is all we need to 
    /// get the triggers to go off
    /// </summary>
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
            GameManager.Inst.ShowGameOver();
        }
        
    }
}
