using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D rb;
    private Vector2 movementInput;
    public Transform[] checkPositions;
    public LayerMask groundLayer;

    private bool canMoveUp, canMoveDown, canMoveLeft, canMoveRight;

    [SerializeField]
    private int speed;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        if (movementInput.y > 0 && canMoveUp == false) movementInput.y = 0;
        if (movementInput.x < 0 && canMoveLeft == false) movementInput.x = 0;
        if (movementInput.y < 0 && canMoveDown == false) movementInput.y = 0;
        if (movementInput.x > 0 && canMoveRight == false) movementInput.x = 0;

        rb.MovePosition(rb.position + movementInput * speed * Time.deltaTime);
    }

    private void Update()
    {
        canMoveUp = !Physics2D.Raycast(checkPositions[0].position, Vector2.up * 1, 0.1f, groundLayer);
        canMoveLeft = !Physics2D.Raycast(checkPositions[1].position, Vector2.left * 1, 0.1f, groundLayer);
        canMoveDown = !Physics2D.Raycast(checkPositions[2].position, Vector2.down * 1, 0.1f, groundLayer);
        canMoveRight = !Physics2D.Raycast(checkPositions[3].position, Vector2.right * 1, 0.1f, groundLayer);
    }

    private void OnMove(InputValue inputValue)
    {
        movementInput = inputValue.Get<Vector2>();
    }
}