using System.Collections;
using System.Collections.Generic;
using Roy_T.AStar.Paths;
using Roy_T.AStar.Primitives;
using Unity.Burst.CompilerServices;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private PathActorAnimator animator;
    [SerializeField] private Player player;
    [SerializeField] private GameObject startingNode;
    [SerializeField] private float movementSpeed;
    [SerializeField] private float waitAfterMoveTime;
    [Header("Teleport")]
    [SerializeField] private GameObject teleportVisualsEnemy;
    [SerializeField] private GameObject teleportVisualsDestination;
    [SerializeField] private float timeToTeleport;

    private Path path;
    private GameObject currentPosition;
    private int edgeIndex;
    private bool isMoving = false;
    private bool isWaitingAfterMove = false;
    private GameObject placeToTeleport;

    [HideInInspector] public Doll Doll;

    // Animation Names
    private const string RUN_ANIMATION = "Run";
    private const string IDLE_ANIMATION = "Idle";

    // Speed
    private float powerUpSpeedModifier = 1;
    private float movementSpeedProper => movementSpeed * path.Distance.Meters * powerUpSpeedModifier;

    // Wait time after move
    private float waitAfterMoveTimeAddon = 0;
    private float waitAfterMoveTimeProper => waitAfterMoveTime + waitAfterMoveTimeAddon;

    private void Start()
    {
        transform.position = startingNode.transform.position;
        currentPosition = startingNode;
    }

    void Update()
    {
        if (!isMoving && !isWaitingAfterMove && player != null)
        {
            GetPathToPlayer();
        }

        if (path != null && path.Type == PathType.Complete)
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
                animator?.ChangeAnimationState(IDLE_ANIMATION);

                if (IsInvoking("WaitAfterMove"))
                {
                    CancelInvoke("WaitAfterMove");
                }
                Invoke("WaitAfterMove", waitAfterMoveTimeProper);
            }
        }
    }

    private void GetPathToPlayer()
    {
        IFollowTarget followTarget = Doll == null ? player : Doll;
        // raycast hit this gameobject
        edgeIndex = 0;
        isMoving = true;
        isWaitingAfterMove = true;
        transform.position = currentPosition.transform.position;
        path = PathController.Singleton.GetPath(currentPosition, followTarget.CurrentPosition());
        if (path != null && path.Type == PathType.Complete)
        {
            currentPosition = followTarget.CurrentPosition();

            animator?.ChangeAnimationState(RUN_ANIMATION);
        }
        else
        {
            WaitAndTeleportToEmptyPlace();
        }
    }

    public void WaitAndTeleportToEmptyPlace()
    {
        IFollowTarget followTarget = Doll == null ? player : Doll;
        placeToTeleport = followTarget.CurrentPosition();
        teleportVisualsEnemy.SetActive(true);
        teleportVisualsDestination.SetActive(true);
        teleportVisualsDestination.transform.position = placeToTeleport.transform.position;
        Invoke("TeleportToPlace", timeToTeleport);
    }

    private void TeleportToPlace()
    {
        teleportVisualsEnemy.SetActive(false);
        teleportVisualsDestination.SetActive(false);
        transform.position = placeToTeleport.transform.position;
        currentPosition = placeToTeleport;
        isMoving = false;


        if (IsInvoking("WaitAfterMove"))
        {
            CancelInvoke("WaitAfterMove");
        }
        Invoke("WaitAfterMove", waitAfterMoveTimeProper);
        SoundEffectPlayer.Instance.PlaySoundClip(SoundEffectPlayer.TELEPORT);
        //GetPathToPlayer();
    }

    private void WaitAfterMove()
    {
        isWaitingAfterMove = false;
        waitAfterMoveTimeAddon = 0;
    }

    #region PowerUps

    public void PickedUpSpeedChangePowerUp(float speedModifier, float duration)
    {
        powerUpSpeedModifier = speedModifier;

        if (IsInvoking("DeleteSpeedPowerUp"))
        {
            CancelInvoke("DeleteSpeedPowerUp");
        }
        Invoke("DeleteSpeedPowerUp", duration);
    }

    private void DeleteSpeedPowerUp()
    {
        powerUpSpeedModifier = 1;
    }

    public void PickedUpEnemyFreezePowerUp(float duration)
    {
        player.ChangeWaitAfterMoveAddon(duration);
    }

    public void ChangeWaitAfterMoveAddon(float duration)
    {
        waitAfterMoveTimeAddon = duration;

        if (IsInvoking("WaitAfterMove"))
        {
            CancelInvoke("WaitAfterMove");
            Invoke("WaitAfterMove", waitAfterMoveTimeProper);
        }
    }

    #endregion
}
