using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public enum GhostBehaviour{
    None,
    Follow,
    Camp,
    Reverse,
    MapRecon,
    AttackStraight
}

public class GhostAI : MonoBehaviour
{
    Seeker seeker;
    AIPath aIPath;

    public Transform target;
    public float movementSpeed;
    public GhostBehaviour myCurrentBehaviour = GhostBehaviour.None;

    private void Start() {
        seeker = GetComponent<Seeker>();
        aIPath = GetComponent<AIPath>();
    }

    private void LateUpdate() {
        if(myCurrentBehaviour == GhostBehaviour.Follow){
            transform.position = target.position;
        }
    }

    public void ChangeBehaviour(GhostBehaviour behaviour, Transform followTarget = null){
        myCurrentBehaviour = behaviour;

        switch (behaviour)
        {
            case GhostBehaviour.Follow:
                target = followTarget;
                break;
        }
    }
}
