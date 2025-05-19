using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using MoreMountains.Feedbacks;
using Roy_T.AStar.Paths;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using static UnityEngine.GraphicsBuffer;
using PathType = Roy_T.AStar.Paths.PathType;

public class Player : MonoBehaviour, IFollowTarget
{
    [SerializeField] private PathActorAnimator animator;
    [SerializeField] private PathNode startingNode;
    [SerializeField] private GameManager gameManager;
    [SerializeField] private float movementSpeed;
    [SerializeField] private float movementSpeedBase;
    [SerializeField] private bool skipIntro;

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
    private float movementSpeedProper => isFrozen ? 0 : (movementSpeedBase + movementSpeed) * powerUpSpeedModifier * Time.deltaTime * 1.5f;

    // Wait time after move
    private float waitAfterMoveTimeProper => waitAfterMoveTime;

    private bool useDoll = false;
    public Path Path => path;

    PathNode IFollowTarget.CurrentPosition() => currentPosition;

    private void Start()
    {
        if (!startingNode)
        {
            Debug.Log("Player has no starting position");
            return;
        }

        transform.position = startingNode.transform.position;
        currentPosition = startingNode;

        if (!skipIntro)
        {
            var duration = 2f;
            visual.transform.localPosition = new Vector3(0, -0.76f, 0);
            visual.transform.DOLocalMoveY(1.02f, duration).SetUpdate(true);


            visual.transform.localScale = Vector3.zero;
            animator?.ChangeAnimationState(RUN_ANIMATION);
            animator?.SetUnscaledUpdateMode(true);
            var scale = visual.transform.DOScale(1f, duration);
            scale.SetUpdate(true);
            scale.onComplete += () =>
            {
                animator?.ChangeAnimationState(IDLE_ANIMATION);
                animator?.SetUnscaledUpdateMode(false);
            };
        }
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
                        UpdateAnimation();

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

                UpdateAnimation();
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
                UpdateAnimation();

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
        UpdateAnimation();
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
        UpdateAnimation();
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
        UpdateStatusColor();
        UpdateAnimation();
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

    private void UpdateAnimation()
    {
        animator?.ChangeAnimationState(isMoving ? currentRunAnimation : currentIdleAnimation);
    }
}
