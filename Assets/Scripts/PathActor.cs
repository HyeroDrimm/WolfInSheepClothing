using System.Collections;
using System.Collections.Generic;
using Roy_T.AStar.Paths;
using Unity.VisualScripting;
using UnityEngine;

public class PathActor : MonoBehaviour
{
    [SerializeField] private PathActorAnimator animator;
    [SerializeField] private GameObject startingNode;
    [SerializeField] private GameManager gameManager;
    [SerializeField] private float movementSpeed;
    [SerializeField] private float waitAfterMoveTime;

    private Path path;
    private float movementSpeedProper;

    public GameObject CurrentPosition
    {
        get => currentPosition;
        private set => currentPosition = value;
    }

    private int edgeIndex;
    private bool isMoving = false;
    private bool isWaitingAfterMove = false;
    private GameObject currentPosition;

    // Animation Names
    private const string RUN_ANIMATION = "Run";
    private const string IDLE_ANIMATION = "Idle";

    private void Start()
    {
        transform.position = startingNode.transform.position;
        currentPosition = startingNode;
    }

    private void Update()
    {
        if (!isMoving && !isWaitingAfterMove && Input.GetKeyDown(KeyCode.Mouse0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit = Physics2D.GetRayIntersection(ray, Mathf.Infinity, 1 << 6);

            if (hit.collider != null && hit.collider.transform.CompareTag("PathfindingTargets"))
            {
                // raycast hit this gameobject
                edgeIndex = 0;
                isMoving = true;
                isWaitingAfterMove = true;
                transform.position = currentPosition.transform.position;
                path = PathController.Singleton.GetPath(currentPosition, hit.transform.gameObject);
                currentPosition = hit.transform.gameObject;
                movementSpeedProper = movementSpeed * path.Distance.Meters;

                animator?.ChangeAnimationState(RUN_ANIMATION);
            }
        }



        /*if (Input.GetKeyDown(KeyCode.Space))
        {
            path = PathController.Singleton.GetPath(startingNode, endNode);
            transform.position = Helpers.RayTAStarPositionToVec3(path.Edges[0].Start.Position);
            edgeIndex = 0;
            currentPosition = endNode;
        }*/

        if (path != null)
        {
            if (path.Edges.Count > edgeIndex)
            {
                var currentEdge = path.Edges[edgeIndex];
                if (currentEdge != null &&
                    Vector3.Distance(transform.position, Helpers.RayTAStarPositionToVec3(currentEdge.End.Position)) < 0.01f)
                {
                    edgeIndex++;
                }
                else
                {
                    transform.position = Vector3.MoveTowards(transform.position, Helpers.RayTAStarPositionToVec3(currentEdge.End.Position),
                        movementSpeedProper * Time.deltaTime * 0.06f);
                }
            }
            else
            {
                path = null;
                isMoving = false;
                StartCoroutine(WaitAfterMove());
                animator?.ChangeAnimationState(IDLE_ANIMATION);
            }
        }
    }

    private IEnumerator WaitAfterMove()
    {
        yield return new WaitForSeconds(waitAfterMoveTime);
        isWaitingAfterMove = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.CompareTag("Enemy"))
        {
            gameManager.EndGame();
            Destroy(gameObject);
        }
    }
}
