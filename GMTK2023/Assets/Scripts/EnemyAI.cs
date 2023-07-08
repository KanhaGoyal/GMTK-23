using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class EnemyAI : MonoBehaviour
{
    public Transform targetPosition;    
    private AIPath aiPath;

    private void Awake(){
        aiPath = GetComponent<AIPath>();
    }

    private void Start() {
        aiPath.destination = targetPosition.position;
    }
}
