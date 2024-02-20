using System.Collections;
using System.Collections.Generic;
using Roy_T.AStar.Paths;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using static UnityEngine.GraphicsBuffer;

public class Player : MonoBehaviour, IFollowTarget
{
    [SerializeField] private PathActorAnimator animator;
    [SerializeField] private Enemy enemy;
    [SerializeField] private GameObject startingNode;
    [SerializeField] private GameManager gameManager;
    [SerializeField] private float movementSpeed;
    [SerializeField] private float movementSpeedBase;

    [SerializeField] private float waitAfterMoveTime;
    [SerializeField] private Doll dollPrefab;
    [SerializeField] private Popup useDollPopup;
    [SerializeField] private SpriteRenderer visual;

    private Path path;
    private int edgeIndex;
    private bool isMoving = false;
    private bool isWaitingAfterMove = false;
    private GameObject currentPosition;
    private GameObject queuedPlace;

    // Animation Names
    private const string RUN_ANIMATION = "Run";
    private const string IDLE_ANIMATION = "Idle";

    // Speed
    private float powerUpSpeedModifier = 1;
    private bool isFrozen;
    private float movementSpeedProper => isFrozen ? 0 : (movementSpeedBase + movementSpeed * path.Distance.Meters) * powerUpSpeedModifier * Time.deltaTime * 0.6f;

    // Wait time after move
    private float waitAfterMoveTimeProper => waitAfterMoveTime;

    private bool useDoll = false;

    GameObject IFollowTarget.CurrentPosition()
    {
        return currentPosition;
    }

    private void Start()
    {
        transform.position = startingNode.transform.position;
        currentPosition = startingNode;
    }

    private void Update()
    {
        if (useDoll && Input.GetKeyDown(KeyCode.Mouse0) && !EventSystem.current.IsPointerOverGameObject())
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit = Physics2D.GetRayIntersection(ray, Mathf.Infinity, 1 << 6);

            if (hit.collider != null && hit.collider.transform.CompareTag("PathfindingTargets"))
            {
                var doll = Instantiate(dollPrefab, hit.transform.position, Quaternion.identity);
                doll.currentPosition = hit.transform.parent.gameObject;
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

            if (hit.collider != null && hit.transform.CompareTag("PathfindingTargets") && hit.transform.parent.gameObject != currentPosition)
            {
                var target = hit.transform.parent.gameObject;
                if (!isFrozen && !isMoving && !isWaitingAfterMove)
                {
                    path = PathController.Singleton.GetPath(currentPosition, target);
                    if (path != null && path.Type == PathType.Complete)
                    {
                        edgeIndex = 0;
                        isMoving = true;
                        isWaitingAfterMove = true;
                        transform.position = currentPosition.transform.position;
                        currentPosition = target;

                        animator?.ChangeAnimationState(RUN_ANIMATION);
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
            path = PathController.Singleton.GetPath(currentPosition, queuedPlace);
            if (path != null && path.Type == PathType.Complete)
            {
                edgeIndex = 0;
                isMoving = true;
                isWaitingAfterMove = true;
                transform.position = currentPosition.transform.position;
                currentPosition = queuedPlace;
                queuedPlace = null;

                animator?.ChangeAnimationState(RUN_ANIMATION);
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
                animator?.ChangeAnimationState(IDLE_ANIMATION);

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
    }

    public void PickedUpEnemyFreezePowerUp(float duration)
    {
        enemy.ChangeFreezeAddon(duration);
    }

    public void ChangeFreezeAddon(float duration)
    {
        isFrozen = true;
        visual.color = Color.blue;
        animator?.ChangeAnimationState(IDLE_ANIMATION);
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
        visual.color = Color.white;
        if (isMoving)
        {
            animator?.ChangeAnimationState(RUN_ANIMATION);
        }
    }

    public void UseDoll()
    {
        useDollPopup.SetVisible(true);
        useDoll = true;
    }

    #endregion
}
