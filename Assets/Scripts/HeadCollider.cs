using System;
using UnityEngine;

public class HeadCollider : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Collided with: " + other.tag);
    }
}
