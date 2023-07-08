using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using System.Threading.Tasks;

public enum EnemyState{
    Patrol,
    PlayerSighted, //I want to use a raycast in front of the pacman!
    Hunter
}

public class EnemyAI : MonoBehaviour
{
    public Vector2 targetPosition;
    private AIPath aiPath;
    private Seeker seeker;

    public EnemyState myState;
    [Header("Roaming")]
    public float roamSpeed;
    public float raycastDistance = 1f;
    public float changeDirectionTimer =1;
    public float turnWaitTime = 0.4f;
    public LayerMask groundLayer;

    [Header("Player Search")]
    public float playerDetectionRaycastDistance = 3f;
    public float playerRunSpeed;
    public float playerSearchTime = 3f;
    public LayerMask playerLayer;
    public bool travelToOrb;
    private List<Vector2> powerupLocations = new List<Vector2>(); //Remember its location, if player is spotted then go to any nearest orb and pick it up!

    private float timer;
    private bool isTurning;
    private void Start()
    {
        // Set the initial target position
        targetPosition = transform.position;

        seeker = GetComponent<Seeker>();
        aiPath = GetComponent<AIPath>();
    }


    private void Update()
    {
        timer -= Time.deltaTime;

        if(myState != EnemyState.PlayerSighted){
            if(Physics2D.Raycast(transform.position, transform.right, playerDetectionRaycastDistance, playerLayer)){
                Debug.DrawRay(transform.position, transform.right * playerDetectionRaycastDistance);
                OnStateChange(EnemyState.PlayerSighted);
            }
        }

        if(travelToOrb) return;

        CheckForPossibleWay();
    }

    public void OnStateChange(EnemyState state){
        myState = state;

        switch (state)
        {
            case EnemyState.Patrol:
                targetPosition = transform.position;
                aiPath.maxSpeed = roamSpeed;
                travelToOrb = false;
                break;
            
            case EnemyState.PlayerSighted:
                Debug.Log("Found my hunter, need to find to orb to start my HUNT");
                GoToNearestOrb();
                Invoke(nameof(ReturnToPatrol), playerSearchTime);
                break;
                
            case EnemyState.Hunter:
                break;
        }
    }

    void ReturnToPatrol(){
        OnStateChange(EnemyState.Patrol);
    }

    void GoToNearestOrb(){
        float nearestDistance = Mathf.Infinity;
        Vector2? target = null;

        foreach (Vector2 dist in powerupLocations)
        {
            float myDistance = Vector2.Distance(transform.position, dist);
            if(myDistance < nearestDistance){
                nearestDistance = myDistance;
                target = dist;
            }
        }

        if(target.HasValue){
            travelToOrb = true;
            Debug.Log($"Running to {target.Value}");
            aiPath.destination = target.Value;
        }else{
            Debug.Log("There is no orb i have founD!! Time to 180!");
            Perform180DegreeTurn();
        }
        aiPath.maxSpeed = playerRunSpeed;
    }

    #region FreeRoam
    private void CheckForPossibleWay(){
        bool hitForward = Physics2D.Raycast(transform.position, transform.right, raycastDistance, groundLayer);
        bool hitUp = Physics2D.Raycast(transform.position, transform.up, raycastDistance, groundLayer);
        bool hitDown = Physics2D.Raycast(transform.position, -transform.up, raycastDistance, groundLayer);

        Debug.DrawRay(transform.position, transform.right);
        Debug.DrawRay(transform.position, transform.up);
        Debug.DrawRay(transform.position, -transform.up);

        if (hitForward && hitUp && hitDown)
        {
            // Walls in front and both sides, perform a 180-degree turn
            Perform180DegreeTurn();
        }
        else if(hitForward){
            if(!hitUp && !hitDown){
                TurnAtWall();
            }
            else if(hitUp && !hitDown){
                transform.Rotate(0, 0, -90f);
                StopMoving();
            }
            else if(hitUp == false && hitDown){
                transform.Rotate(0, 0, 90f);
                StopMoving();
            }
        }

        else if(!hitForward && timer < 0){
            if(hitUp && hitDown == false){
                bool turnRight = Random.value < 0.3f;
                if(turnRight){
                    Debug.Log("Turning Down");
                    transform.Rotate(0, 0, -90f);
                    StopMoving();
                } 
            }

            else if(hitDown && hitUp == false){
                bool turnLeft = Random.value < 0.3f;
                if(turnLeft){
                    Debug.Log("Turning Up");

                    transform.Rotate(0, 0, 90f);
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
        targetPosition = (Vector2)transform.position + (Vector2)transform.right * raycastDistance;
        aiPath.destination = targetPosition;
    }

    private void Perform180DegreeTurn()
    {
        transform.Rotate(0f, 180f, 0);
        StopMoving();
        MoveForward();
    }

    private void TurnAtWall()
    {
        // Choose a random direction to turn (up or down)
        bool turnRight = Random.value < 0.5f;

        // Perform the turn by rotating 90 degrees in the chosen direction
        transform.Rotate(0f, 0f, turnRight ? -90f : 90f);
        StopMoving();
    }

    private async void StopMoving(){
        await Task.Delay(600);
        aiPath.maxSpeed = 0;
        isTurning = true;
        await Task.Delay(Mathf.RoundToInt(turnWaitTime * 1000));
        aiPath.maxSpeed = roamSpeed;
        MoveForward();
        isTurning = false;
    }
    #endregion

    private void OnTriggerEnter2D(Collider2D other) {
        if(other.TryGetComponent<CommonOrbScript>(out CommonOrbScript orb)){
            powerupLocations.Add(orb.transform.position);
        }
    }
}