using UnityEngine;
using Random = UnityEngine.Random;

public class Food : MonoBehaviour
{
    //we need our material so we change change colors..
    private Material m_material;
    void Start()
    {
        m_material = GetComponent<Renderer>().materials[0];
        //initially we'll be red
        m_material.color = Color.red;
        //well leave our initial position up to wherever we are in the scene..
    }

    public void Respawn()
    {
        //could theoretically be a deadlock, though not very likely... could easily have a 
        //run check and just bail out if we do it too many times, or do a recursive call,
        //but this should be ok for this purpose
        while (true)
        {
            //whenever we get eaten we need to move somewhere else and change color
            m_material.color = new Color(Random.value, Random.value, Random.value, 1);

            //let's move +/-30 spaces away from the head
            //we'll start at 1 and not 0 so we don't pop right on top of the head again
            var newPos = new Vector3(Random.Range(1, 31), 0, Random.Range(1, 31));

            //we have to check if we're off the edge of the world
            
            if (newPos.x <= -39 || newPos.x >= 39 || newPos.z <= -39 || newPos.z >= 30) continue;
            
            transform.localPosition = newPos;
            break;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.tag.Equals("Body")) return;
        
        //presumably the only way this happens is if we spawn on top of it so..... let's just respawn again
        Respawn();
    }
}
