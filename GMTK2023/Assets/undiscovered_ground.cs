using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class undiscovered_ground : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.CompareTag("Vision"))
        {
            Destroy(gameObject);
        }
    }
}
