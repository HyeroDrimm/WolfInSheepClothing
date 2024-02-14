using System.Collections;
using System.Collections.Generic;
using Roy_T.AStar.Paths;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class Player : MonoBehaviour, IFollowTarget
{
    [SerializeField] private PathActorAnimator animator;
    [SerializeField] private Enemy enemy;
    [SerializeField] private GameObject startingNode;
    [SerializeField] private GameManager gameManager;
    [SerializeField] private float movementSpeed;
    [SerializeField] private float waitAfterMoveTime;
    [SerializeField] private Doll dollPrefab;
    [SerializeField] private Popup useDollPopup;

    private Path path;
    private int edgeIndex;
    private bool isMoving = false;
    private bool isWaitingAfterMove = false;
    private GameObject currentPosition;

    // Animation Names
    private const string RUN_ANIMATION = "Run";
    private const string IDLE_ANIMATION = "Idle";

    // Speed
    private float powerUpSpeedModifier = 1;
    private float movementSpeedProper => movementSpeed * path.Distance.Meters * powerUpSpeedModifier;

    // Wait time after move
    private float waitAfterMoveTimeAddon = 0;
    private float waitAfterMoveTimeProper => waitAfterMoveTime + waitAfterMoveTimeAddon;

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
                doll.currentPosition = hit.transform.gameObject;
                enemy.Doll = doll;
                useDoll = false;
                useDollPopup.SetVisible(false);

                SoundEffectPlayer.Instance.PlaySoundClip(SoundEffectPlayer.SELECT);
            }
        }
        else if (!isMoving && !isWaitingAfterMove && Input.GetKeyDown(KeyCode.Mouse0) && !EventSystem.current.IsPointerOverGameObject())
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

                animator?.ChangeAnimationState(RUN_ANIMATION);
                SoundEffectPlayer.Instance.PlaySoundClip(SoundEffectPlayer.SELECT);
            }
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
                animator?.ChangeAnimationState(IDLE_ANIMATION);

                if (useDoll)
                {
                    var doll = Instantiate(dollPrefab, transform.position, Quaternion.identity);
                    doll.currentPosition = currentPosition;
                    enemy.Doll = doll;
                    useDoll = false;
                }

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
        waitAfterMoveTimeAddon = 0;
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
        enemy.ChangeWaitAfterMoveAddon(duration);
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

    public void UseDoll()
    {
        useDollPopup.SetVisible(true);
        useDoll = true;
    }

    #endregion
}
