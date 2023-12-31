using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridMovement : MonoBehaviour
{
    public bool isFacingRight = false;

    private bool isMoving;
    private Vector3 origPos, targetPos;
    private float timeToMove = 0.2f;

    public Transform[] checkPositions;
    private bool canMoveUp, canMoveDown, canMoveLeft, canMoveRight;
    [HideInInspector] public Vector2 lastFacedDirection;
    public LayerMask groundLayer;

    private Transform graphics;

    NodeGenerator nodeGenerator;

    private void Start() {
        nodeGenerator = GetComponent<NodeGenerator>();
        graphics = GetComponentInChildren<SpriteRenderer>().transform;
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.W) && canMoveUp && !isMoving)
        {
            StartCoroutine(MovePlayer(Vector3.up));
        }
        if (Input.GetKey(KeyCode.A) && canMoveLeft && !isMoving)
        {
            if(isFacingRight) Flip();
            StartCoroutine(MovePlayer(Vector3.left));
        }
        if (Input.GetKey(KeyCode.S) && canMoveDown && !isMoving)
        {
            StartCoroutine(MovePlayer(Vector3.down));
        }
        if (Input.GetKey(KeyCode.D) && canMoveRight && !isMoving)
        {
            StartCoroutine(MovePlayer(Vector3.right));
            if(!isFacingRight) Flip();
        }

        canMoveUp = !Physics2D.Raycast(checkPositions[0].position, Vector2.up * 1, 0.1f, groundLayer);
        canMoveLeft = !Physics2D.Raycast(checkPositions[1].position, Vector2.left * 1, 0.1f, groundLayer);
        canMoveDown = !Physics2D.Raycast(checkPositions[2].position, Vector2.down * 1, 0.1f, groundLayer);
        canMoveRight = !Physics2D.Raycast(checkPositions[3].position, Vector2.right * 1, 0.1f, groundLayer);

        if(Input.GetKeyDown(KeyCode.E)){
            nodeGenerator.CreateNewNode(lastFacedDirection, groundLayer);
        }
    }

    void Flip(){
        isFacingRight = !isFacingRight;
        Vector3 scale = graphics.localScale;
        scale.x *= -1;
        graphics.localScale = scale;
    }

    private IEnumerator MovePlayer(Vector3 direction)
    {
        lastFacedDirection = direction;

        isMoving = true;

        float elapsedTime = 0;

        origPos = transform.position;
        targetPos = origPos + direction;
        nodeGenerator.MoveNodes(targetPos);

        while(elapsedTime < timeToMove)
        {
            transform.position = Vector3.Lerp(origPos, targetPos, (elapsedTime / timeToMove));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = targetPos;
        isMoving = false;
    }
}
