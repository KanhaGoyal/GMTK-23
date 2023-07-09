using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum LevelState
{
    BeingHunter,
    BeingHunted,
    GameOver
}

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;
    public LevelState currentState = LevelState.BeingHunter;
    public List<GhostAI> AllAnts = new List<GhostAI>();
    public List<EnemyAI> Enemies = new List<EnemyAI>();
    public Player player;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }else{
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        player = FindObjectOfType<Player>();
    }

    public void TurnTheCards(LevelState state)
    {
        if(currentState == state) return;
        currentState = state;

        player.SwitchMode(state);
        foreach (GhostAI ant in AllAnts)
        {
            ant.SwitchMode(state);
        }
        

        switch (state)
        {
            case LevelState.BeingHunter:
                //Add the code here
                foreach (EnemyAI enemy in Enemies)
                {
                    enemy.OnStateChange(EnemyState.Patrol);
                }
                break;

            case LevelState.BeingHunted:
                //Add the code here!
                foreach (EnemyAI enemy in Enemies)
                {
                    enemy.OnStateChange(EnemyState.Hunter);
                }
                break;
            
            case LevelState.GameOver:
                Debug.Log("YOU DIED!");
                break;
        }
    }

    public void CheckForWin(EnemyAI enemyDestroyed){
        Enemies.Remove(enemyDestroyed);
        if(Enemies.Count <= 0){
            Debug.Log("Win!");
        }
    }
}
