using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using UnityEngine.UI;
using System.Threading.Tasks;

public enum GhostBehaviour
{
    None,
    Follow,
    Camp,
    Reverse,
    MapRecon,
    AttackStraight
}

public class GhostAI : MonoBehaviour
{
    AIPath aIPath;

    public Sprite huntingSprite, huntedSprite;
    [Space(10)]
    public bool isFacingRight = false;
    private Transform target;
    public float movementSpeed;
    public GhostBehaviour myCurrentBehaviour = GhostBehaviour.None;
    public LayerMask groundLayer;
    [Space]
    public Transform visionGameobject;

    [Header("Recon Stuff")]
    public Transform rayCastOrigin;
    public float reconMoveSpeed;
    public float raycastDistance;
    public float changeDirectionTimer = 1f;
    public float turnWaitTime = 0.3f;
    public float reconVisionRadius;

    [Header("UI display")]
    public Image behaviourGizmo;
    public List<BehaviourGizmoData> gizmoDatas;

    [System.Serializable]
    public struct BehaviourGizmoData
    {
        public GhostBehaviour Behaviour;
        public Sprite Sprite;
    }

    Vector2 attackLastPoint;
    float timer;
    Transform graphics;
    private void Start()
    {
        aIPath = GetComponent<AIPath>();
        graphics = GetComponentInChildren<SpriteRenderer>().transform;

        Physics2D.IgnoreLayerCollision(7, 9); //Player and Follower Layer
        aIPath.maxSpeed = movementSpeed;
    }

    private void Update()
    {
        if(myCurrentBehaviour == GhostBehaviour.MapRecon){
            CheckForPossibleWay();
        }

        if(myCurrentBehaviour != GhostBehaviour.None){
            //Change the direction of the entity based the velocity
            if(aIPath.velocity.x < 0 && isFacingRight){
                //moveleft!
                Flip();
                if(myCurrentBehaviour == GhostBehaviour.Follow) rayCastOrigin.localRotation = Quaternion.Euler(rayCastOrigin.localRotation.x, 180, rayCastOrigin.localRotation.x);
            }
            else if(aIPath.velocity.x > 0 && isFacingRight == false){
                if(myCurrentBehaviour == GhostBehaviour.Follow) rayCastOrigin.localRotation = Quaternion.Euler(rayCastOrigin.localRotation.x, 0, rayCastOrigin.localRotation.x);
                Flip();
            }
        }
    }

    void Flip(){
        isFacingRight = !isFacingRight;
        Vector3 scale = graphics.localScale;
        scale.x *= -1;
        graphics.localScale = scale;
    }

    private void LateUpdate()
    {
        if (myCurrentBehaviour == GhostBehaviour.Follow)
        {
            transform.position = target.position;
        }

        if (myCurrentBehaviour == GhostBehaviour.AttackStraight)
        {
            if (Vector2.Distance(transform.position, attackLastPoint) <= 1f)
            {
                Destroy(gameObject);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            Debug.Log("Being a sucide bomber!");
            if(LevelManager.Instance.currentState == LevelState.BeingHunter) Destroy(other.gameObject);
            LevelManager.Instance.AllAnts.Remove(this);
            Destroy(gameObject);
        }
        if (myCurrentBehaviour == GhostBehaviour.AttackStraight)
        {

            if ((groundLayer & (1 << other.gameObject.layer)) != 0)
            {
                Debug.Log("WASTED!");
                LevelManager.Instance.AllAnts.Remove(this);
                Destroy(gameObject);
            }
        }
    }

    public void SwitchMode(LevelState state){
        if(state == LevelState.BeingHunted) GetComponentInChildren<SpriteRenderer>().sprite = huntedSprite;
        else if(state == LevelState.BeingHunter) GetComponentInChildren<SpriteRenderer>().sprite = huntingSprite;
    }

    public void ChangeBehaviour(GhostBehaviour behaviour, Transform followTarget = null, Vector2? campPosition = null, Vector2? attackDir = null)
    {
        myCurrentBehaviour = behaviour;

        switch (behaviour)
        {
            case GhostBehaviour.Follow:
                target = followTarget;
                break;

            case GhostBehaviour.Camp:
                //aIPath.destination = campPosition.Value;
                //GetComponent<Collider2D>().isTrigger = false;
                StartCoroutine(MovePlayer(campPosition.Value));
                behaviourGizmo.sprite = GetBehaviourGizmo(behaviour);
                behaviourGizmo.gameObject.SetActive(true);
                break;

            case GhostBehaviour.AttackStraight:
                //Move in straight line until we collide!
                //GetComponent<Collider2D>().isTrigger = false;
                RaycastHit2D hit = Physics2D.Raycast(transform.position, attackDir.Value, 900f, groundLayer);
                aIPath.destination = hit.point;
                attackLastPoint = hit.point;

                behaviourGizmo.sprite = GetBehaviourGizmo(behaviour);
                behaviourGizmo.gameObject.SetActive(true);
                break;

            case GhostBehaviour.MapRecon:
                aIPath.destination = transform.position;
                visionGameobject.localScale = new Vector3(reconVisionRadius, reconVisionRadius, 0);
                behaviourGizmo.sprite = GetBehaviourGizmo(behaviour);
                behaviourGizmo.gameObject.SetActive(true);
                break;

            case GhostBehaviour.Reverse:
                LevelManager.Instance.TurnTheCards(LevelState.BeingHunter);
                behaviourGizmo.sprite = GetBehaviourGizmo(behaviour);
                behaviourGizmo.gameObject.SetActive(true);
                LevelManager.Instance.AllAnts.Remove(this);
                Destroy(gameObject, 1f);
                break;
        }
    }

    Sprite GetBehaviourGizmo(GhostBehaviour behaviour)
    {
        foreach (BehaviourGizmoData data in gizmoDatas)
        {
            if (data.Behaviour == behaviour) return data.Sprite;
        }

        return null;
    }


    float timeToMove = 0.3f;
    private IEnumerator MovePlayer(Vector3 targetPos)
    {
        float elapsedTime = 0;

        Vector2 origPos = transform.position;
        while (elapsedTime < timeToMove)
        {
            transform.position = Vector3.Lerp(origPos, targetPos, (elapsedTime / timeToMove));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = targetPos;
    }

    #region AutoMovement / Recon
    private void CheckForPossibleWay()
    {
        bool hitForward = Physics2D.Raycast(rayCastOrigin.position, rayCastOrigin.right, raycastDistance, groundLayer);
        bool hitUp = Physics2D.Raycast(rayCastOrigin.position, rayCastOrigin.up, raycastDistance, groundLayer);
        bool hitDown = Physics2D.Raycast(rayCastOrigin.position, -rayCastOrigin.up, raycastDistance, groundLayer);

        Debug.DrawRay(rayCastOrigin.position, rayCastOrigin.right);
        Debug.DrawRay(rayCastOrigin.position, rayCastOrigin.up);
        Debug.DrawRay(rayCastOrigin.position, -rayCastOrigin.up);

        if (hitForward && hitUp && hitDown)
        {
            // Walls in front and both sides, perform a 180-degree turn
            Perform180DegreeTurn();
        }
        else if (hitForward)
        {
            if (!hitUp && !hitDown)
            {
                TurnAtWall();
            }
            else if (hitUp && !hitDown)
            {
                rayCastOrigin.Rotate(0, 0, -90f);
                StopMoving();
            }
            else if (hitUp == false && hitDown)
            {
                rayCastOrigin.Rotate(0, 0, 90f);
                StopMoving();
            }
        }

        else if (!hitForward && timer < 0)
        {
            if (hitUp && hitDown == false)
            {
                bool turnRight = Random.value < 0.3f;
                if (turnRight)
                {
                    Debug.Log("Turning Down");
                    rayCastOrigin.Rotate(0, 0, -90f);
                    StopMoving();
                }
            }

            else if (hitDown && hitUp == false)
            {
                bool turnLeft = Random.value < 0.3f;
                if (turnLeft)
                {
                    Debug.Log("Turning Up");

                    rayCastOrigin.Rotate(0, 0, 90f);
                    StopMoving();
                }
            }

            timer = changeDirectionTimer;
        }
        else
        {
            // No walls detected, continue moving forward along the path
            MoveForward();
        }
    }


     private void MoveForward()
    {
        // Calculate the target position in front of the player
        Vector2 targetPosition = (Vector2)rayCastOrigin.position + (Vector2)rayCastOrigin.right * raycastDistance;
        aIPath.destination = targetPosition;
    }

    private void Perform180DegreeTurn()
    {
        rayCastOrigin.Rotate(0f, 180f, 0);
        StopMoving();
        MoveForward();
    }

    private void TurnAtWall()
    {
        // Choose a random direction to turn (up or down)
        bool turnRight = Random.value < 0.5f;

        // Perform the turn by rotating 90 degrees in the chosen direction
        rayCastOrigin.Rotate(0f, 0f, turnRight ? -90f : 90f);
        StopMoving();
    }

    private async void StopMoving(){
        await Task.Delay(600);
        aIPath.maxSpeed = 0;
        await Task.Delay(Mathf.RoundToInt(turnWaitTime * 1000));
        aIPath.maxSpeed = reconMoveSpeed;
        MoveForward();
    }
    #endregion
}
