using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnapPosition : MonoBehaviour
{
    private bool snappedPlayer;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("SnapTo") && snappedPlayer == false)
        {
            snappedPlayer = true;
            collision.transform.parent.transform.position = transform.position;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("SnapTo"))
        {
            snappedPlayer = false;
        }
    }
}
