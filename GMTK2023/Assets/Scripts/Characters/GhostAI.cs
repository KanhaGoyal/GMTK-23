using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using UnityEngine.UI;

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
    public LayerMask groundLayer;

    [Header("UI display")]
    public Image behaviourGizmo;
    public List<BehaviourGizmoData> gizmoDatas;

    [System.Serializable]
    public struct BehaviourGizmoData{
        public GhostBehaviour Behaviour;
        public Sprite Sprite;
    }

    Vector2 attackLastPoint;


    private void Start() {
        seeker = GetComponent<Seeker>();
        aIPath = GetComponent<AIPath>();

        Physics2D.IgnoreLayerCollision(7, 8); //Player and Follower Layer
        aIPath.maxSpeed = movementSpeed;
    }

    private void Update() {

    }

    private void LateUpdate() {
        if(myCurrentBehaviour == GhostBehaviour.Follow){
            transform.position = target.position;
        }

        if(myCurrentBehaviour == GhostBehaviour.AttackStraight){
            if(Vector2.Distance(transform.position, attackLastPoint) <= 1.2f){
                Destroy(gameObject);
            }
        }
    }

    public void ChangeBehaviour(GhostBehaviour behaviour, Transform followTarget = null, Vector2? campPosition = null, Vector2? attackDir = null){
        myCurrentBehaviour = behaviour;

        switch (behaviour)
        {
            case GhostBehaviour.Follow:
                target = followTarget;
                break;

            case GhostBehaviour.Camp:
                //aIPath.destination = campPosition.Value;
                GetComponent<Collider2D>().isTrigger = false;
                StartCoroutine(MovePlayer(campPosition.Value));
                behaviourGizmo.sprite = GetBehaviourGizmo(behaviour);
                behaviourGizmo.gameObject.SetActive(true);
                break;

            case GhostBehaviour.AttackStraight:
                //Move in straight line until we collide!
                GetComponent<Collider2D>().isTrigger = false;
                RaycastHit2D hit = Physics2D.Raycast(transform.position, attackDir.Value, 900f, groundLayer);
                aIPath.destination = hit.point;
                attackLastPoint = hit.point;

                behaviourGizmo.sprite = GetBehaviourGizmo(behaviour);
                behaviourGizmo.gameObject.SetActive(true);
                break;
        }
    }

    Sprite GetBehaviourGizmo(GhostBehaviour behaviour){
        foreach (BehaviourGizmoData data in gizmoDatas)
        {
            if(data.Behaviour == behaviour) return data.Sprite;
        }

        return null;
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if(myCurrentBehaviour == GhostBehaviour.AttackStraight){
            if(other.TryGetComponent<EnemyAI>(out EnemyAI enemy)){
                Debug.Log("Being a sucide bomber!");
                Destroy(enemy.gameObject);
                Destroy(gameObject);
            }

            if(( groundLayer & (1 << other.gameObject.layer)) != 0){
                Debug.Log("WASTED!");
                Destroy(gameObject);
            }
        }
    }

    float timeToMove = 0.3f;
    private IEnumerator MovePlayer(Vector3 targetPos)
    {
        float elapsedTime = 0;

        Vector2 origPos = transform.position;
        while(elapsedTime < timeToMove)
        {
            transform.position = Vector3.Lerp(origPos, targetPos, (elapsedTime / timeToMove));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = targetPos;
    }
}
