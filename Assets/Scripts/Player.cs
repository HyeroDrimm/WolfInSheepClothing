using System.Collections;
using System.Collections.Generic;
using MoreMountains.Feedbacks;
using Roy_T.AStar.Paths;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using static UnityEngine.GraphicsBuffer;

public class Player : MonoBehaviour, IFollowTarget
{
    [SerializeField] private PathActorAnimator animator;
    [SerializeField] private PathNode startingNode;
    [SerializeField] private GameManager gameManager;
    [SerializeField] private float movementSpeed;
    [SerializeField] private float movementSpeedBase;

    [SerializeField] private float waitAfterMoveTime;
    [SerializeField] private Doll dollPrefab;
    [SerializeField] private Popup useDollPopup;
    [SerializeField] private SpriteRenderer visual;
    [SerializeField] private MMF_Player speedUpFeedback;
    [SerializeField] private MMF_Player speedDownFeedback;

    private Enemy enemy;
    public Path path;
    private int edgeIndex;
    private bool isMoving = false;
    private bool isWaitingAfterMove = false;
    private PathNode currentPosition;
    private PathNode queuedPlace;
    private BoardManager boardManager;

    // Animation Names
    private const string RUN_ANIMATION = "Run";
    private const string IDLE_ANIMATION = "Idle";
    private const string RUN_SLOWED_ANIMATION = "Run Slowed";
    private const string IDLE_SLOWED_ANIMATION = "Idle Slowed";

    private string currentRunAnimation => powerUpSpeedModifier < 1 ? RUN_SLOWED_ANIMATION : RUN_ANIMATION;
    private string currentIdleAnimation => powerUpSpeedModifier < 1 ? IDLE_SLOWED_ANIMATION : IDLE_ANIMATION;

    // Speed
    private float powerUpSpeedModifier = 1;
    private bool isFrozen;
    private float movementSpeedProper => isFrozen ? 0 : (movementSpeedBase + movementSpeed) * powerUpSpeedModifier * Time.deltaTime * 1.5f;

    // Wait time after move
    private float waitAfterMoveTimeProper => waitAfterMoveTime;

    private bool useDoll = false;
    public Path Path => path;


    PathNode IFollowTarget.CurrentPosition()
    {
        return currentPosition;
    }

    private void Start()
    {
        if (!startingNode)
        {
            Debug.Log("Player has no starting position");
            return;
        }

        transform.position = startingNode.transform.position;
        currentPosition = startingNode;
    }

    private void Update()
    {
        if (useDoll && Input.GetKeyDown(KeyCode.Mouse0) && !EventSystem.current.IsPointerOverGameObject())
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit = Physics2D.GetRayIntersection(ray, Mathf.Infinity, 1 << 6);

            if (hit.collider != null && hit.collider.transform.CompareTag("PathfindingTargets") && hit.transform.parent.TryGetComponent(out PathNode target))
            {
                var doll = Instantiate(dollPrefab, hit.transform.position, Quaternion.identity);
                doll.currentPosition = target;
                enemy.Doll = doll;
                useDoll = false;
                useDollPopup.SetVisible(false);

                SoundEffectPlayer.Instance.PlaySoundClip(SoundEffectPlayer.SELECT);
            }
        }
        else if (Input.GetKeyDown(KeyCode.Mouse0) && !EventSystem.current.IsPointerOverGameObject())
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit = Physics2D.GetRayIntersection(ray, Mathf.Infinity, 1 << 6);

            if (hit.collider != null && hit.transform.CompareTag("PathfindingTargets") && hit.transform.parent.TryGetComponent(out PathNode target) && target != currentPosition)
            {
                if (!isFrozen && !isMoving && !isWaitingAfterMove)
                {
                    path = boardManager.GetPath(currentPosition, target);
                    if (path != null && path.Type == PathType.Complete)
                    {
                        edgeIndex = 0;
                        isMoving = true;
                        isWaitingAfterMove = true;
                        transform.position = currentPosition.transform.position;
                        currentPosition = target;

                        animator?.ChangeAnimationState(currentRunAnimation);
                        SoundEffectPlayer.Instance.PlaySoundClip(SoundEffectPlayer.SELECT);
                    }
                }
                else
                {
                    queuedPlace = target;
                }
            }
        }

        if (!isFrozen && !isMoving && !isWaitingAfterMove && queuedPlace != null && queuedPlace != currentPosition)
        {
            path = boardManager.GetPath(currentPosition, queuedPlace);
            if (path != null && path.Type == PathType.Complete)
            {
                edgeIndex = 0;
                isMoving = true;
                isWaitingAfterMove = true;
                transform.position = currentPosition.transform.position;
                currentPosition = queuedPlace;
                queuedPlace = null;

                animator?.ChangeAnimationState(currentRunAnimation);
                SoundEffectPlayer.Instance.PlaySoundClip(SoundEffectPlayer.SELECT);
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
                animator?.ChangeAnimationState(currentIdleAnimation);

                if (IsInvoking("WaitAfterMove"))
                {
                    CancelInvoke("WaitAfterMove");
                }
                Invoke("WaitAfterMove", waitAfterMoveTimeProper);
            }
        }
    }

    private void WaitAfterMove()
    {
        isWaitingAfterMove = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.CompareTag("Enemy"))
        {
            gameManager.EndGame();
            SoundEffectPlayer.Instance.PlaySoundClip(SoundEffectPlayer.KILL);
            Destroy(gameObject);
        }
    }

    #region PowerUp

    public void PickedUpSpeedChangePowerUp(float speedModifier, float duration)
    {
        powerUpSpeedModifier = speedModifier;
        animator?.ChangeAnimationState(currentRunAnimation);
        UpdateStatusColor();

        if (powerUpSpeedModifier < 1)
        {
            speedDownFeedback.GetFeedbackOfType<MMF_HoldingPause>().FeedbackDuration = Mathf.Max(0, duration - 2);
            speedDownFeedback.PlayFeedbacks();
        }
        else if (powerUpSpeedModifier > 1)
        {
            speedUpFeedback.GetFeedbackOfType<MMF_HoldingPause>().FeedbackDuration = Mathf.Max(0, duration - 2);
            speedUpFeedback.PlayFeedbacks();
        }

        if (IsInvoking("DeleteSpeedPowerUp"))
        {
            CancelInvoke("DeleteSpeedPowerUp");
        }
        Invoke("DeleteSpeedPowerUp", duration);
    }

    private void DeleteSpeedPowerUp()
    {
        if (powerUpSpeedModifier < 1)
        {
            SoundEffectPlayer.Instance.PlaySoundClip(SoundEffectPlayer.POWER_UP);
        }
        else if (powerUpSpeedModifier > 1)
        {
            SoundEffectPlayer.Instance.PlaySoundClip(SoundEffectPlayer.POWER_DOWN);
        }

        powerUpSpeedModifier = 1;
        animator?.ChangeAnimationState(currentRunAnimation);
        UpdateStatusColor();
    }

    public void PickedUpEnemyFreezePowerUp(float duration)
    {
        enemy.ChangeFreezeAddon(duration);
    }

    public void ChangeFreezeAddon(float duration)
    {
        isFrozen = true;
        UpdateStatusColor();
        animator?.ChangeAnimationState(currentIdleAnimation);
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
        UpdateStatusColor();
        if (isMoving)
        {
            animator?.ChangeAnimationState(currentRunAnimation);
        }
    }

    public void UseDoll()
    {
        useDollPopup.SetVisible(true);
        useDoll = true;
    }

    private void UpdateStatusColor()
    {
        if (isFrozen)
        {
            visual.color = Color.blue;
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

    public void Setup(BoardManager boardManager, Enemy enemy)
    {
        this.boardManager = boardManager;
        this.enemy = enemy;
    }
}
