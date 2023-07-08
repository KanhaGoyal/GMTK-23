using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeGenerator : MonoBehaviour
{
    public Transform player;
    public GameObject nodePrefab;

    private List<Transform> nodes = new List<Transform>();
    public Vector3 previousPlayerPosition;

    // Start is called before the first frame update
    void Start()
    {
        previousPlayerPosition = player.position;

        CreateAINode(transform.position);
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 direction = (player.position - transform.position).normalized;
    }

     public bool CheckIfPlayerFlips(Vector3 previousPosition, Vector3 currentPosition, Vector3 direction)
    {
        float dotProduct = Vector3.Dot(previousPosition - currentPosition, direction);
        Debug.Log(dotProduct < 0);
        return dotProduct < 0;
    }

    public void CreateAINode(Vector3 position)
    {
        GameObject node = Instantiate(nodePrefab, position, Quaternion.identity);
        nodes.Add(node.transform);
    }

    public void MoveNodes(Vector2 targetPos)
    {
        if (nodes.Count == 0)
            return;

        List<Vector2> previousNodePositions = new List<Vector2>();
        foreach (Transform node in nodes)
        {
            previousNodePositions.Add(node.position);
        }

        for (int i = 1; i < nodes.Count; i++)
        {
            nodes[i].position = previousNodePositions[i - 1];
        }

        nodes[0].position = targetPos;
    }

    public void CreateNewNode(Vector2 lastFacedDirection ,LayerMask groundLayer){
        Vector2 lastNodePosition = nodes[nodes.Count - 1].position;

        bool canMoveUp = !Physics2D.Raycast(lastNodePosition, Vector2.up * 1, 1, groundLayer);
        bool canMoveLeft = !Physics2D.Raycast(lastNodePosition, Vector2.left * 1, 1, groundLayer);
        bool canMoveDown = !Physics2D.Raycast(lastNodePosition, Vector2.down * 1, 1, groundLayer);
        bool canMoveRight = !Physics2D.Raycast(lastNodePosition, Vector2.right * 1, 1, groundLayer);

        if(Input.GetKeyDown(KeyCode.E)){
            Vector2 nodeDirection = transform.position;

            if(canMoveUp && lastFacedDirection == Vector2.down) nodeDirection = Vector2.up;
            else if(canMoveLeft && lastFacedDirection == Vector2.right) nodeDirection = Vector2.left;
            else if(canMoveDown && lastFacedDirection == Vector2.up) nodeDirection = Vector2.down;
            else if(canMoveRight && lastFacedDirection == Vector2.left) nodeDirection = Vector2.right;
            else{
                if(canMoveUp && !IsThereAnyNodeInDirection(Vector2.up)) nodeDirection = Vector2.up;
                else if(canMoveLeft && !IsThereAnyNodeInDirection(Vector2.left)) nodeDirection = Vector2.left;
                else if(canMoveDown && !IsThereAnyNodeInDirection(Vector2.down)) nodeDirection = Vector2.down;
                else if(canMoveRight && !IsThereAnyNodeInDirection(Vector2.right)) nodeDirection = Vector2.right;
            }

            FindObjectOfType<NodeGenerator>().CreateAINode((Vector2)lastNodePosition + nodeDirection);
        }
    }

    private bool IsThereAnyNodeInDirection(Vector2 direction){
        Vector2 nodeDirection = (Vector2)nodes[nodes.Count - 1].position + direction;
        foreach (Transform node in nodes)
        {
            if((Vector2)node.position == nodeDirection){
                return true;
            }
        }

        return false;
    }
}
