using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridMovement : MonoBehaviour
{
    private bool isMoving;
    private Vector3 origPos, targetPos;
    private float timeToMove = 0.2f;

    public Transform[] checkPositions;
    private bool canMoveUp, canMoveDown, canMoveLeft, canMoveRight;
    private Vector2 lastFacedDirection;
    public LayerMask groundLayer;

<<<<<<< Updated upstream
    public GameObject[] addedGhosts;
=======
    NodeGenerator nodeGenerator;

    private void Start() {
        nodeGenerator = GetComponent<NodeGenerator>();
    }
>>>>>>> Stashed changes

    void Update()
    {
        if (Input.GetKey(KeyCode.W) && canMoveUp && !isMoving)
        {
            StartCoroutine(MovePlayer(Vector3.up));
        }
        if (Input.GetKey(KeyCode.A) && canMoveLeft && !isMoving)
        {
            StartCoroutine(MovePlayer(Vector3.left));
        }
        if (Input.GetKey(KeyCode.S) && canMoveDown && !isMoving)
        {
            StartCoroutine(MovePlayer(Vector3.down));
        }
        if (Input.GetKey(KeyCode.D) && canMoveRight && !isMoving)
        {
            StartCoroutine(MovePlayer(Vector3.right));
        }

        canMoveUp = !Physics2D.Raycast(checkPositions[0].position, Vector2.up * 1, 0.1f, groundLayer);
        canMoveLeft = !Physics2D.Raycast(checkPositions[1].position, Vector2.left * 1, 0.1f, groundLayer);
        canMoveDown = !Physics2D.Raycast(checkPositions[2].position, Vector2.down * 1, 0.1f, groundLayer);
        canMoveRight = !Physics2D.Raycast(checkPositions[3].position, Vector2.right * 1, 0.1f, groundLayer);

        if(Input.GetKeyDown(KeyCode.E)){
            nodeGenerator.CreateNewNode(lastFacedDirection, groundLayer);
        }
    }

    private IEnumerator MovePlayer(Vector3 direction)
    {
        lastFacedDirection = direction;

        isMoving = true;

        float elapsedTime = 0;

        origPos = transform.position;
        targetPos = origPos + direction;

        while(elapsedTime < timeToMove)
        {
            transform.position = Vector3.Lerp(origPos, targetPos, (elapsedTime / timeToMove));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        nodeGenerator.previousPlayerPosition = origPos;
        nodeGenerator.CheckIfPlayerFlips(origPos, targetPos, direction);
        nodeGenerator.MoveNodes(targetPos);
        transform.position = targetPos;

        isMoving = false;
    }
}
