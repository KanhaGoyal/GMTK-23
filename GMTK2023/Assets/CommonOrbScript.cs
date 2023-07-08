using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommonOrbScript : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            AddGhost();
        }

        if(collision.CompareTag("Enemy"))
        {
            KillPlayerTimer();
        }
    }

    private void AddGhost()
    {

    }

    private void KillPlayerTimer()
    {

    }
}
