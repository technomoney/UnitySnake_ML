using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Snake : MonoBehaviour
{
    /// <summary>
    /// Prefab to make new parts of the snake body, assign in inspector
    /// </summary>
    public Transform pfb_bodyChunk;


    /// <summary>
    /// the current direction the snake is moving, in rads
    /// </summary>
    private float m_movementBearing = 0;

    /// <summary>
    /// List to hold all the parts of the snake
    /// </summary>
    private List<Transform> m_list_bodyParts;

    void Start()
    {
        //add the head and the tail initially to the body parts list
        m_list_bodyParts = new List<Transform> { transform.Find("Snake_Head"), transform.Find("Snake_Tail") };

    }

    void Update()
    {
        //testing
        if (Input.GetKeyDown(KeyCode.Space))
            AddBodyChunk();
        if(Input.GetKeyDown(KeyCode.R))
            StartMoving();
    }

    /// <summary>
    /// should only need to be called once upon game run to get the snake moving
    /// </summary>
    private void StartMoving()
    {
        StartCoroutine(HandleMovement());
    }

    /// <summary>
    /// add a chunk to the snake
    /// </summary>
    private void AddBodyChunk()
    {
        //make the cube and parent it under the main snake object
        var chunk = Instantiate(pfb_bodyChunk, transform);
        m_list_bodyParts.Add(chunk);
        
        //plop the new part right behind the head
        chunk.transform.localPosition = new Vector3(0, 0, (m_list_bodyParts.Count - 1) * -1);
        
        //let's keep the tail at the back of the list
        MoveTailToEndOfList();
        
        //we'll need to move the tail to stay behind everything else
        var tail = m_list_bodyParts.Last();
        tail.transform.localPosition = new Vector3(0, 0, (m_list_bodyParts.Count) * -1);

    }

    /// <summary>
    /// Swaps the last and second to last positions in the body parts list, should be called right after adding a new
    /// body part, depends on the tail being second to last...
    /// </summary>
    private void MoveTailToEndOfList()
    {
        var newBodyChunk = m_list_bodyParts.Last();
        var listSize = m_list_bodyParts.Count;
        m_list_bodyParts[listSize - 1] = m_list_bodyParts[listSize - 2];
        m_list_bodyParts[listSize - 2] = newBodyChunk;
    }

    /// <summary>
    /// co routine to handle moving the head and then getting every body part to follow
    /// </summary>
    IEnumerator HandleMovement()
    {
        var targetPosition = new Vector3(m_list_bodyParts[0].localPosition.x + Mathf.Cos(m_movementBearing), 0,
            m_list_bodyParts[0].localPosition.z + Mathf.Sin(m_movementBearing));
        var initialPosition = m_list_bodyParts[0].localPosition;
        var progress = 0f;

        while (progress < 1)
        {
            m_list_bodyParts[0].localPosition = Vector3.Lerp(initialPosition, targetPosition, progress);
            progress += Time.deltaTime;
            yield return 0;
        }
        
        //done moving
    }
}
