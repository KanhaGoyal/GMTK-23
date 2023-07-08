using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

    NodeGenerator node;
    GridMovement movement;

    public Sprite huntingSprite, huntedSprite;
    public List<GhostAI> GhostBodies;
    public List<GhostAI> FollowingGhosts;

    public int maxFollowerAmount;
    public GhostAI followerPrefab;

    [Header("Buttons")]
    public Animator upKey;
    public Animator downKey, leftKey, rightKey;

    // Start is called before the first frame update
    void Start()
    {
        node = GetComponent<NodeGenerator>();
        movement = GetComponent<GridMovement>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.UpArrow)) upKey.Play("ButtonPressed");
        if(Input.GetKeyDown(KeyCode.DownArrow)) downKey.Play("ButtonPressed");
        if(Input.GetKeyDown(KeyCode.LeftArrow)) leftKey.Play("ButtonPressed");
        if(Input.GetKeyDown(KeyCode.RightArrow)) rightKey.Play("ButtonPressed");
    }

    public void SpawnFollowerGhost(){
        node.CreateNewNode(movement.lastFacedDirection, movement.groundLayer);
        GhostAI follower = Instantiate(followerPrefab, node.nodes[node.nodes.Count - 1].position, Quaternion.identity);
        follower.ChangeBehaviour(GhostBehaviour.Follow, node.nodes[node.nodes.Count - 1]);
    }
}
