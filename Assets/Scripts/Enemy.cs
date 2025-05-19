using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Roy_T.AStar.Paths;
using Roy_T.AStar.Primitives;
using UnityEngine;
using PathType = Roy_T.AStar.Paths.PathType;

public class Enemy : MonoBehaviour
{
    [SerializeField] private PathActorAnimator animator;
    [SerializeField] private PathNode startingNode;
    [SerializeField] private float movementSpeed;
    [SerializeField] private float movementSpeedBaseStarting;
    [SerializeField] private float movementSpeedBaseMax;
    [SerializeField] private float movementSpeedBaseMaxTime;
    [SerializeField] private float waitAfterMoveTime;
    [SerializeField] private SpriteRenderer visual;
    [SerializeField] private Collider2D enemyCollider;
    [SerializeField] private bool skipIntro;

    [Header("Teleport")]
    [SerializeField] private GameObject teleportVisualsEnemy;
    [SerializeField] private GameObject teleportVisualsDestination;
    [SerializeField] private float timeToTeleport;

    private Player player;
    private Path path;
    private PathNode currentPosition;
    private int edgeIndex;
    private bool isMoving = false;
    private bool isWaitingAfterMove = false;
    private PathNode placeToTeleport;
    private BoardManager boardManager;

    [HideInInspector] public Doll Doll;

    // Animation Names
    private const string RUN_ANIMATION = "Run";
    private const string RUN_SLOWED_ANIMATION = "Run Slowed";
    private const string RUN_FAST_ANIMATION = "Run Fast";

    private const string IDLE_ANIMATION = "Idle";
    private const string IDLE_SLOWED_ANIMATION = "Idle Slowed";
    private const string IDLE_FASR_ANIMATION = "Idle Fast";

    private const string FROZEN_ANIMATION = "Frozen";

    private string currentRunAnimation
    {
        get
        {
            if (isFrozen)
                return FROZEN_ANIMATION;
            else if (powerUpSpeedModifier < 1)
                return RUN_SLOWED_ANIMATION;
            else if (powerUpSpeedModifier > 1)
                return RUN_FAST_ANIMATION;
            else
                return RUN_ANIMATION;
        }
    }

    private string currentIdleAnimation
    {
        get
        {
            if (isFrozen)
                return FROZEN_ANIMATION;
            else if (powerUpSpeedModifier < 1)
                return IDLE_SLOWED_ANIMATION;
            else if (powerUpSpeedModifier > 1)
                return IDLE_FASR_ANIMATION;
            else
                return IDLE_ANIMATION;
        }
    }

    // Speed
    private float powerUpSpeedModifier = 1;
    private bool isFrozen;
    private float movementSpeedProper => isFrozen ? 0 : (movementSpeedBaseStarting + (movementSpeedBaseMax - movementSpeedBaseStarting) * Mathf.Min(Time.timeSinceLevelLoad / movementSpeedBaseMaxTime, 1) + movementSpeed) * powerUpSpeedModifier * Time.deltaTime * 1.5f;



    // Wait time after move
    private float waitAfterMoveTimeProper => waitAfterMoveTime;

    public Path Path => path;

    private void Start()
    {
        if (!startingNode)
        {
            Debug.Log("Enemy has no starting position");
            return;
        }

        transform.position = startingNode.transform.position;
        currentPosition = startingNode;

        isWaitingAfterMove = true;
        Invoke("WaitAfterMove", waitAfterMoveTimeProper);


        if (!skipIntro)
        {
            var duration = 2f;
            visual.transform.localPosition = new Vector3(0, -0.93f, 0);
            visual.transform.DOLocalMoveY(1.02f, duration).SetUpdate(true);


            visual.transform.localScale = Vector3.zero;
            animator?.ChangeAnimationState(RUN_ANIMATION);
            animator?.SetUnscaledUpdateMode(true);
            var scale = visual.transform.DOScale(0.79f, duration);
            scale.SetUpdate(true);
            scale.onComplete += () =>
            {
                animator?.ChangeAnimationState(IDLE_ANIMATION);
                animator?.SetUnscaledUpdateMode(false);
            };
        }
    }

    void Update()
    {
        if (!isMoving && !isWaitingAfterMove && player != null)
        {
            IFollowTarget followTarget = Doll == null ? player : Doll;
            // raycast hit this gameobject
            path = boardManager.GetPath(currentPosition, followTarget.CurrentPosition());
            if (path != null && path.Type == PathType.Complete)
            {
                edgeIndex = 0;
                isMoving = true;
                isWaitingAfterMove = true;
                transform.position = currentPosition.transform.position;
                currentPosition = followTarget.CurrentPosition();

                UpdateAnimation();
            }
            else
            {
                WaitAndTeleportToEmptyPlace();
            }
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
                        movementSpeedProper);
                    animator?.Flip((Helpers.RayTAStarPositionToVec3(currentEdge.End.Position) - transform.position).x < 0);
                }
            }
            else
            {
                path = null;
                isMoving = false;

                UpdateAnimation();

                if (IsInvoking("WaitAfterMove"))
                {
                    CancelInvoke("WaitAfterMove");
                }
                Invoke("WaitAfterMove", waitAfterMoveTimeProper);
            }
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
    }

    private void WaitAfterMove()
    {
        isWaitingAfterMove = false;
    }
    public void Setup(BoardManager boardManager, Player player)
    {
        this.boardManager = boardManager;
        this.player = player;
    }

    #region PowerUps

    public void PickedUpSpeedChangePowerUp(float speedModifier, float duration)
    {
        powerUpSpeedModifier = speedModifier;
        UpdateStatusColor();
        UpdateAnimation();

        if (IsInvoking("DeleteSpeedPowerUp"))
        {
            CancelInvoke("DeleteSpeedPowerUp");
        }
        Invoke("DeleteSpeedPowerUp", duration);
    }

    private void DeleteSpeedPowerUp()
    {
        powerUpSpeedModifier = 1;
        UpdateStatusColor();
        UpdateAnimation();
    }

    public void PickedUpEnemyFreezePowerUp(float duration)
    {
        player.ChangeFreezeAddon(duration);
    }

    public void ChangeFreezeAddon(float duration)
    {
        isFrozen = true;
        enemyCollider.enabled = false;
        UpdateStatusColor();

        UpdateAnimation();

        SoundEffectPlayer.Instance.PlaySoundClip(SoundEffectPlayer.FREEZE);

        if (IsInvoking("RemoveFreezeAddon"))
        {
            CancelInvoke("RemoveFreezeAddon");
        }
        Invoke("RemoveFreezeAddon", duration);
    }

    private void RemoveFreezeAddon()
    {
        isFrozen = false;
        enemyCollider.enabled = true;
        UpdateStatusColor();
        UpdateAnimation();
    }

    private void UpdateStatusColor()
    {
        if (isFrozen)
        {
            visual.color = Color.blue;
        }
        else if (powerUpSpeedModifier < 1)
        {
            visual.color = Color.red;
        }
        else if (powerUpSpeedModifier > 1)
        {
            visual.color = Color.green;
        }
        else
        {
            visual.color = Color.white;
        }
    }

    #endregion

    private void UpdateAnimation()
    {
        animator?.ChangeAnimationState(isMoving ? currentRunAnimation : currentIdleAnimation);
    }
}
