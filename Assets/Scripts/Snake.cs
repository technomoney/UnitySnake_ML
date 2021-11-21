using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

public class Snake : MonoBehaviour
{
    #region Variables
    
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
    public List<Transform> m_list_bodyParts;

    /// <summary>
    /// we can use a flag here to tell the snake to grow after we finish our current movement lerp
    /// this will stop the snake from growing during a lerp and getting an index out of range error
    /// </summary>
    public bool m_growBiggerAfterMovement = false;

    /// <summary>
    /// we can use this flag to halt the snake from ever getting out of pause, so he'll just stop
    /// </summary>
    public bool m_isStopped;

    /// <summary>
    /// How long do we pause between movement steps
    /// </summary>
    private const float PauseTime = .25f;
    
    #endregion
    
    void Start()
    {
        //add the head and the tail initially to the body parts list
        m_list_bodyParts = new List<Transform> { transform.Find("Snake_Head"), transform.Find("Snake_Tail") };
        
        //add the three starting body chunks
        AddBodyChunk();
        AddBodyChunk();
        AddBodyChunk();

        //do a single pause, which will get us moving once it completes
        StartCoroutine(HandlePause());
    }

    void Update()
    {
        //this is cheating, but is useful for testing...
        if (Input.GetKeyDown(KeyCode.Space))
            AddBodyChunk();

        //check for rotation of the movement direction
        if (Input.GetKeyDown(KeyCode.E))
        {
            m_movementBearing -= 1.57f;
            ClampMovementBearing();
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            m_movementBearing += 1.57f;
            ClampMovementBearing();
        }

        //rotate the camera to match the movement direction
        //let's just rotate the head, which is a sphere anyway, where the camera is a child
        //we use -deg2Rad here since we want it to face the opposite direction we're actually moving
        //todo if we have time this should be a nice lerp instead of the quick snap of the camera
        m_list_bodyParts[0].transform.rotation = Quaternion.Euler(0, m_movementBearing * -Mathf.Rad2Deg, 0);
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
    /// add a chunk to the snake
    /// </summary>
    private void AddBodyChunk()
    {
        //make the cube and parent it under the main snake object
        var chunk = Instantiate(pfb_bodyChunk, transform);
        m_list_bodyParts.Add(chunk);
        
        //let's keep the tail at the back of the list
        MoveTailToEndOfList();
        
        //put the new chunk right where the tail currently is, then we'll bump the tail accordingly
        chunk.transform.localPosition = m_list_bodyParts.Last().localPosition;
        
        //we'll need to move the tail to stay behind everything else
        var tail = m_list_bodyParts.Last();
        //his new position should depend on the direction he's currently facing...
        var newPos = new Vector3(tail.localPosition.x - Mathf.Cos(m_movementBearing), 0,
            tail.localPosition.z - Mathf.Sin(m_movementBearing));
        tail.transform.localPosition = newPos;
        
        //every time we add a chunk, increase our score
        GameManager.Inst.Score++;
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
    /// scoot each element of the snake one unit ahead in the direction we're facing
    /// </summary>
    private void HandleMovement()
    {
        //set the target positions of each body parts, we'll initialize the the list with the target
        //for the head which we can grab with the current bearing and simple trig
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
            targetPositions.Add(m_list_bodyParts[x - 1].localPosition);
        }
        
        
        //lets also fix the rotation of the tail
        //the rotation doesn't necessarily care about the current heading, more about the which direction we just
        //moved, so let's figure that out.  this is also a little tricky in that we lose our initial position after
        //we move, so we do this after we set targets, but before we actually move to them
        var initialPos = new Vector2(m_list_bodyParts.Last().localPosition.x, m_list_bodyParts.Last().localPosition.z);
        var targetPos = new Vector2(targetPositions.Last().x, targetPositions.Last().z);
        var diff = targetPos - initialPos;
        var ang = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;
        m_list_bodyParts.Last().transform.rotation = Quaternion.Euler(0, -ang, 0);

        
        //set the positions of the parts by manually setting them to their targets
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
            if (part.localPosition.z > 40) part.localPosition = new Vector3(0, 0, -40);
        }

        
        //done moving
        //when we're actually done moving start a pause
        StartCoroutine(HandlePause());

        //see if we ate something and need to grow
        if (m_growBiggerAfterMovement)
        {
            AddBodyChunk();
            m_growBiggerAfterMovement = false; //reset the flag
        }
    }

    /// <summary>
    /// simple way to delay the calling of the next handlemovement()
    /// Adjust the speed in the PauseTime const
    /// </summary>
    /// <returns></returns>
    IEnumerator HandlePause()
    {
        var progress = 0f;

        while (progress < PauseTime)
        {
            progress += Time.deltaTime;
            yield return 0;
        }

        //this will stop us from moving
        if (!m_isStopped)
            HandleMovement();
    }
}
