using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using System.Threading.Tasks;

public class EnemyAI : MonoBehaviour
{
    public Vector2 targetPosition;
    private AIPath aiPath;
    private Seeker seeker;

    [Header("Roaming")]
    public float raycastDistance = 1f;
    public float changeDirectionTimer =1;
    public float turnWaitTime = 0.4f;
    public LayerMask groundLayer;

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
        // Shoot raycasts in front, left, and right of the player
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
        aiPath.destination = transform.position;
        isTurning = true;
        await Task.Delay(Mathf.RoundToInt(turnWaitTime * 1000));
        MoveForward();
        isTurning = false;
    }
}