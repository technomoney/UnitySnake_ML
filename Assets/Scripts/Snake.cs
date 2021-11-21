using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

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
        
        AddBodyChunk();
        AddBodyChunk();
        AddBodyChunk();

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
        
        //plop the new part right behind the head, we use the -2 here since we don't need to count the head/tail
        chunk.transform.localPosition = new Vector3((m_list_bodyParts.Count - 2) * -1, 0, 0);
        
        //let's keep the tail at the back of the list
        MoveTailToEndOfList();
        
        //we'll need to move the tail to stay behind everything else
        var tail = m_list_bodyParts.Last();
        tail.transform.localPosition = new Vector3((m_list_bodyParts.Count - 1) * -1, 0, 0);

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
        //get the position we want to move to next
        //we'll have to track this per body chunk to get the classic snake-game movement style, so for now
        //just get the target for the head and we'll loop through the rest
        var targetPositions = new List<Vector3>
        {
            new Vector3(
                m_list_bodyParts[0].localPosition.x + Mathf.Cos(m_movementBearing), 0,
                m_list_bodyParts[0].localPosition.z + Mathf.Sin(m_movementBearing))
        };

        for (int x = 1; x < m_list_bodyParts.Count; x++)
        {
            //the movement target for a given body part (besides the head) is the position of the part just in front of it
            //in the loop (keeping the loop in order i.e. swapping the tail is key here)
            targetPositions.Add(m_list_bodyParts[x-1].localPosition);
        }
        
        //set our initial positions for the lerp
        var initialPositions = new List<Vector3>();
        //we can just do a foreach here since the process is the same for all parts including the head here
        foreach (var part in m_list_bodyParts) initialPositions.Add(part.localPosition);
        //lerp progress
        var progress = 0f;

        while (progress < 1)
        {
            //adjust the positions of the parts
            for (int x = 0; x < m_list_bodyParts.Count; x++)
                m_list_bodyParts[x].localPosition = Vector3.Lerp(initialPositions[x], targetPositions[x], progress);
            
            //increment the progress
            progress += Time.deltaTime;
            //return from the coroutine until we're finished moving
            yield return 0;
        }
        
        //done moving
    }
}
