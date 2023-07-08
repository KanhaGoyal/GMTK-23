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
        //if(FollowingGhosts.Count <= 0) return;

        if(Input.GetKeyDown(KeyCode.LeftArrow)) leftKey.Play("ButtonPressed");
        if(Input.GetKeyDown(KeyCode.DownArrow)) downKey.Play("ButtonPressed");
        if(Input.GetKeyDown(KeyCode.UpArrow)) upKey.Play("ButtonPressed");
        if(Input.GetKeyDown(KeyCode.RightArrow)) rightKey.Play("ButtonPressed");

        if (Input.GetKeyUp(KeyCode.UpArrow))
        {
            upKey.Play("ButtonReleased");
        }
        if (Input.GetKeyUp(KeyCode.DownArrow))
        {
            downKey.Play("ButtonReleased");
            Debug.Log("Camping mode on!");
            FollowingGhosts[0].ChangeBehaviour(GhostBehaviour.Camp, null, transform.position);
            FollowingGhosts.RemoveAt(0);
            node.nodes.RemoveAt(0);
        }
        if (Input.GetKeyUp(KeyCode.LeftArrow))
        {
            leftKey.Play("ButtonReleased");
            Debug.Log("Baiting an ally!");
            FollowingGhosts[0].ChangeBehaviour(GhostBehaviour.AttackStraight, null, null, movement.lastFacedDirection);
            FollowingGhosts.RemoveAt(0);
            node.nodes.RemoveAt(0);
        }
        if (Input.GetKeyUp(KeyCode.RightArrow))
        {
            rightKey.Play("ButtonReleased");
        }
    }

    public void SpawnFollowerGhost(){
        node.CreateNewNode(movement.lastFacedDirection, movement.groundLayer);
        GhostAI follower = Instantiate(followerPrefab, node.nodes[node.nodes.Count - 1].position, Quaternion.identity);
        follower.ChangeBehaviour(GhostBehaviour.Follow, node.nodes[node.nodes.Count - 1]);
        FollowingGhosts.Add(follower);
        GhostBodies.Add(follower);
    }
}
