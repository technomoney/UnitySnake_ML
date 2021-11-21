using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
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
    public float m_movementBearing = 0;

    /// <summary>
    /// List to hold all the parts of the snake
    /// </summary>
    private List<Transform> m_list_bodyParts;

    /// <summary>
    /// we can use a flag here to tell the snake to grow after we finish our current movement lerp
    /// this will stop the snake from growing during a lerp and getting an index out of range error
    /// </summary>
    public bool m_growBiggerAfterMovement = false;

    /// <summary>
    /// control how fast the snake moves
    /// </summary>
    private float m_moveSpeed = 10f;

    /// <summary>
    /// we can pause slightly between each movement
    /// </summary>
    private bool m_isPaused = false;
    private float m_pauseTime = .07f;
    private float m_pauseProgress = 0;
    
    void Start()
    {
        //add the head and the tail initially to the body parts list
        m_list_bodyParts = new List<Transform> { transform.Find("Snake_Head"), transform.Find("Snake_Tail") };
        
        AddBodyChunk();
        AddBodyChunk();
        AddBodyChunk();
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
            ContinueMoving();
        
        
        //snake rotation
        if (Input.GetKeyDown(KeyCode.E))
            m_movementBearing -= 1.57f;
        if (Input.GetKeyDown(KeyCode.Q))
            m_movementBearing += 1.57f;
        
        //rotate the camera to match the movement direction
        //let's just rotate the head, which is a sphere anyway, where the camera is a child
        //we use -deg2Rad here since we want it to face the opposite direction we're actually moving
        //todo if we have time this should be a nice lerp instead of the quick snap of the camera
        m_list_bodyParts[0].transform.rotation = Quaternion.Euler(0, m_movementBearing*-Mathf.Rad2Deg, 0);
        
        HandlePause();
    }

    /// <summary>
    /// keep our bearing to nice 0-360 numbers
    /// </summary>
    private void ClampMovementBearing()
    {
        if (m_movementBearing < -0) m_movementBearing += 6.28f;
        if (m_movementBearing > 6.28f) m_movementBearing -= 6.28f;

        //this might be more superstition than anything, but it makes me feel better
        if (m_movementBearing == 6.28f) m_movementBearing = 0;

    }

    /// <summary>
    /// should only need to be called once upon game run to get the snake moving
    /// </summary>
    private void ContinueMoving()
    {
        StartCoroutine(HandleMovement());
    }

    /// <summary>
    /// add a chunk to the snake
    /// </summary>
    public void AddBodyChunk()
    {
        //make the cube and parent it under the main snake object
        var chunk = Instantiate(pfb_bodyChunk, transform);
        m_list_bodyParts.Add(chunk);
        
        //let's keep the tail at the back of the list
        MoveTailToEndOfList();
        
        //put the new chunk right where the tail currently is, then we'll bump the tail accordingly
        //chunk.transform.localPosition = new Vector3((m_list_bodyParts.Count - 2) * -1, 0, 0);
        chunk.transform.localPosition = m_list_bodyParts.Last().localPosition;
        
        //we'll need to move the tail to stay behind everything else
        var tail = m_list_bodyParts.Last();
        //his new position should depend on the direction he's currently facing...
        var newPos = new Vector3(tail.localPosition.x - Mathf.Cos(m_movementBearing), 0,
            tail.localPosition.z - Mathf.Sin(m_movementBearing));
        tail.transform.localPosition = newPos;
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

    private void HandlePause()
    {
        if (!m_isPaused) return;

        m_pauseProgress += Time.deltaTime;
        if (m_pauseProgress < m_pauseTime) return;

        m_isPaused = false;
        m_pauseProgress = 0;

        ContinueMoving();
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
            progress += Time.deltaTime*m_moveSpeed;
            //return from the coroutine until we're finished moving
            yield return 0;
        }
        
        //done moving
        //when we're actually done moving start a pause
        m_isPaused = true;
        
        //see if we ate something and need to grow
        if (m_growBiggerAfterMovement)
        {
            AddBodyChunk();
            m_growBiggerAfterMovement = false; //reset the flag
        }
        
        //we should also rectify the positions of the parts by manually setting them to their targets
        //this will remove andy weird rounding/lerp errors
        //we use the conditional check in the count to check against the list sizes of body parts and target positions
        //these can be different if we added a chunk, it's not an issue since we'll have already corrected their positions
        //in AddChunk(), but we'll still do the check here so we aren't getting out of range errors
        for (int x = 0; x < m_list_bodyParts.Count - (targetPositions.Count == m_list_bodyParts.Count ? 0 : 1); x++)
        {
            m_list_bodyParts[x].localPosition = targetPositions[x];
            
            //lets also just check if we've moved to the other end of the world
            //todo this is a little ugly, fix this if we have time
            var part = m_list_bodyParts[x];
            if (part.localPosition.x < -40) part.localPosition = new Vector3(40, 0, 0);
            if (part.localPosition.x > 40) part.localPosition = new Vector3(-40, 0, 0);
            if (part.localPosition.z < -40) part.localPosition = new Vector3(0, 0, 40);
            if (part.localPosition.z > 40) part.localPosition = new Vector3(0,0, -40);

        }


        //lets also fix the rotation of the tail
        m_list_bodyParts.Last().transform.rotation = Quaternion.Euler(0, m_movementBearing * -Mathf.Rad2Deg, 0);
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("collided with " + other.tag);
    }
}
