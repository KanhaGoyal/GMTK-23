using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommonOrbScript : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.TryGetComponent<Player>(out Player player)){
            player.SpawnFollowerGhost();
            Destroy(gameObject);
        }
    }

    /*
    private void EnemyOrb()
    {
        ghosts turn hollow;
        enemies turn solid;
        change music to Dark theme;

        if(enemy collides with any ghost)
        {
            Player Dies;
        }

        if(enemies are outside of vision bubble)
        {
            Dont render enemy;
        }
    }*/
}
