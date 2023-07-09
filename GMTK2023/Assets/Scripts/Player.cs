using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

    NodeGenerator node;
    GridMovement movement;

    public Sprite huntingSprite, huntedSprite;
    [HideInInspector] public List<GhostAI> FollowingGhosts;

    [Space]
    public Transform visionTransform;
    public float normalVisionRadius, huntedVisionRadius;
    public int maxFollowerAmount;
    public GhostAI followerPrefab;

    [Header("Buttons")]
    public Animator upKey;
    public Animator downKey, leftKey, rightKey;

    public bool IsHunter = true;

    // Start is called before the first frame update
    void Start()
    {
        node = GetComponent<NodeGenerator>();
        movement = GetComponent<GridMovement>();
    }

    // Update is called once per frame
    void Update()
    {
        #region Input Handling
        //if(FollowingGhosts.Count <= 0) return;

        if(Input.GetKeyDown(KeyCode.LeftArrow)) leftKey.Play("ButtonPressed");
        if(Input.GetKeyDown(KeyCode.DownArrow)) downKey.Play("ButtonPressed");
        if(Input.GetKeyDown(KeyCode.UpArrow)) upKey.Play("ButtonPressed");
        if(Input.GetKeyDown(KeyCode.RightArrow)) rightKey.Play("ButtonPressed");

        if (Input.GetKeyUp(KeyCode.UpArrow))
        {
            upKey.Play("ButtonReleased");

            if(FollowingGhosts.Count <= 0) return;
            Debug.Log("Take my shadow as sacrifice!");
            FollowingGhosts[0].ChangeBehaviour(GhostBehaviour.Reverse);
            FollowingGhosts.RemoveAt(0);
            node.nodes.RemoveAt(0);
        }

        if (Input.GetKeyUp(KeyCode.DownArrow))
        {
            downKey.Play("ButtonReleased");

            if(FollowingGhosts.Count <= 0) return;
            Debug.Log("Camping mode on!");
            FollowingGhosts[0].ChangeBehaviour(GhostBehaviour.Camp, null, transform.position);
            FollowingGhosts.RemoveAt(0);
            node.nodes.RemoveAt(0);
        }

        if (Input.GetKeyUp(KeyCode.LeftArrow))
        {
            leftKey.Play("ButtonReleased");

            
            if(FollowingGhosts.Count <= 0) return;
            Debug.Log("Baiting an ally!");
            FollowingGhosts[0].ChangeBehaviour(GhostBehaviour.AttackStraight, null, null, movement.lastFacedDirection);
            FollowingGhosts.RemoveAt(0);
            node.nodes.RemoveAt(0);
        }

        if (Input.GetKeyUp(KeyCode.RightArrow))
        {
            rightKey.Play("ButtonReleased");
            
            if(FollowingGhosts.Count <= 0) return;
            Debug.Log("Sending a vulnarable scout!!");
            FollowingGhosts[0].ChangeBehaviour(GhostBehaviour.MapRecon);
            FollowingGhosts.RemoveAt(0);
            node.nodes.RemoveAt(0);
        }
        #endregion
    }

    private void OnCollisionEnter2D(Collision2D other) {
        if(other.gameObject.CompareTag("Enemy")){
            if(IsHunter)
            {
                EnemyAI enemy;
                other.gameObject.TryGetComponent<EnemyAI>(out enemy);
                other.transform.parent.TryGetComponent<EnemyAI>(out enemy);
                LevelManager.Instance.CheckForWin(enemy);
                Destroy(other.gameObject);
            } 
            else Destroy(gameObject);
        }
    }

    public void SpawnFollowerGhost(){
        node.CreateNewNode(movement.lastFacedDirection, movement.groundLayer);
        GhostAI follower = Instantiate(followerPrefab, node.nodes[node.nodes.Count - 1].position, Quaternion.identity);
        follower.SwitchMode(LevelManager.Instance.currentState);
        follower.ChangeBehaviour(GhostBehaviour.Follow, node.nodes[node.nodes.Count - 1]);
        FollowingGhosts.Add(follower);
        LevelManager.Instance.AllAnts.Add(follower);
    }


    public void SwitchMode(LevelState state){
        if(state == LevelState.BeingHunted && IsHunter){
            IsHunter = false;
            StartCoroutine(ChangeVisionRadiusLerp(new Vector3(huntedVisionRadius, huntedVisionRadius, 0)));
            GetComponentInChildren<SpriteRenderer>().sprite = huntedSprite;
        }

        else if(state == LevelState.BeingHunter && !IsHunter){
            IsHunter = true;
            StartCoroutine(ChangeVisionRadiusLerp(new Vector3(normalVisionRadius, normalVisionRadius, 0)));
            GetComponentInChildren<SpriteRenderer>().sprite = huntingSprite;
        }
    }

    private IEnumerator ChangeVisionRadiusLerp(Vector3 target)
    {
        float elapsedTime = 0;

        Vector2 originalScale = visionTransform.localScale;
        while (elapsedTime < 0.75f)
        {
            visionTransform.localScale = Vector3.Lerp(originalScale, target, (elapsedTime / 0.75f));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        visionTransform.localScale = target;
    }
}
