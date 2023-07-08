using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum LevelState
{
    BeingHunter,
    BeingHunted
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

        foreach (EnemyAI enemy in FindObjectsOfType<EnemyAI>())
        {
            Enemies.Add(enemy);
        }
    }

    public void TurnTheCards(LevelState state)
    {
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
                break;

            case LevelState.BeingHunted:
                //Add the code here!
                break;
        }
    }
}
