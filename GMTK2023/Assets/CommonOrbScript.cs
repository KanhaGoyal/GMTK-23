using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommonOrbScript : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.TryGetComponent<Player>(out Player player) && player.FollowingGhosts.Count < player.maxFollowerAmount){
            player.SpawnFollowerGhost();
            Destroy(gameObject);
        }

        if(collision.TryGetComponent<EnemyAI>(out EnemyAI enemy)){
            LevelManager.Instance.TurnTheCards(LevelState.BeingHunted);
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
