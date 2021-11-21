using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        
    }
}
