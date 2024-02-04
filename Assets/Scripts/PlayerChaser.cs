using System.Collections;
using System.Collections.Generic;
using Roy_T.AStar.Paths;
using Unity.Burst.CompilerServices;
using UnityEngine;

public class PlayerChaser : MonoBehaviour
{
    [SerializeField] private PathActorAnimator animator;
    [SerializeField] private PathActor chaseTarget;
    [SerializeField] private GameObject startingNode;
    [SerializeField] private float movementSpeed;
    [SerializeField] private float waitAfterMoveTime;

    private float movementSpeedProper;
    private Path path;
    private GameObject currentPosition;
    private int edgeIndex;
    private bool isMoving = false;
    private bool isWaitingAfterMove = false;

    // Animation Names
    private const string RUN_ANIMATION = "Run";
    private const string IDLE_ANIMATION = "Idle";


    private void Start()
    {
        transform.position = startingNode.transform.position;
        currentPosition = startingNode;
    }

    void Update()
    {
        if (!isMoving && !isWaitingAfterMove && chaseTarget != null)
        {
            // raycast hit this gameobject
            edgeIndex = 0;
            isMoving = true;
            isWaitingAfterMove = true;
            transform.position = currentPosition.transform.position;
            path = PathController.Singleton.GetPath(currentPosition, chaseTarget.CurrentPosition);
            currentPosition = chaseTarget.CurrentPosition;
            movementSpeedProper = movementSpeed * path.Distance.Meters;

            animator?.ChangeAnimationState(RUN_ANIMATION);

        }

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
}
