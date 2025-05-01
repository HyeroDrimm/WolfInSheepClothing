using System;
using System.Collections;
using System.Collections.Generic;
using Codice.Client.BaseCommands;
using Codice.CM.Common;
using UnityEngine;
using UnityEngine.VFX;

public class PathNode : MonoBehaviour
{
    [Header("Pickups")]
    [SerializeField] private Coin coinPickup;
    [SerializeField] private SpeedChangePowerUp speedUpPickup;
    [SerializeField] private SpeedChangePowerUp speedDownPickup;
    [SerializeField] private FreezeEnemyPowerUp freezePickup;
    [Header("Destructor")]
    [SerializeField] private GameObject hitBox;
    [SerializeField] private float decreaseRate = 1f;

    [SerializeField] private GameObject normalVisual;
    [SerializeField] private GameObject prepare1Visual;
    [SerializeField] private GameObject prepare2Visual;
    [SerializeField] private GameObject prepare3Visual;
    [SerializeField] private GameObject explodedVisuals;


    private Player player;
    private Enemy enemy;
    private BoardManager boardManager;
    private PickUp currentPickup = null;

    private DestructorState state = DestructorState.None;
    private float timer = 0;
    private float maxTime = 0;
    private float corruptionFraction = 0;

    public DestructorState State => state;

    public float CorruptionFraction => corruptionFraction;

    private void Awake()
    {
        SetState(DestructorState.Neutral);
        coinPickup.Setup(this);
        speedUpPickup.Setup(this);
        speedDownPickup.Setup(this);
        freezePickup.Setup(this);
    }

    private void Update()
    {
        if (state == DestructorState.InProcess)
        {
            if (!boardManager.IsNodeInAnyPaths(this))
            {
                timer += Time.deltaTime;
                corruptionFraction = timer / maxTime;
            }
            if (corruptionFraction >= 1.0f)
            {
                SetState(DestructorState.Exploded);
            }
            else
            {

                if (corruptionFraction < 0.333f)
                {
                    prepare1Visual.SetActive(true);
                    prepare2Visual.SetActive(false);
                    prepare3Visual.SetActive(false);
                }
                else if (corruptionFraction < 0.666f)
                {
                    prepare1Visual.SetActive(false);
                    prepare2Visual.SetActive(true);
                    prepare3Visual.SetActive(false);
                }
                else
                {
                    prepare1Visual.SetActive(false);
                    prepare2Visual.SetActive(false);
                    prepare3Visual.SetActive(true);
                }
            }
        }
    }
    private void OnTriggerStay2D(Collider2D other)
    {
        if (state == DestructorState.InProcess && other.CompareTag("Player"))
        {
            timer -= decreaseRate * Time.deltaTime;
            if (timer <= 0)
            {
                SetState(DestructorState.Neutral);
            }
        }

        if (state == DestructorState.Exploded && other.CompareTag("Enemy"))
        {
            var enemy = other.GetComponentInParent<Enemy>();
            enemy.WaitAndTeleportToEmptyPlace();
        }

        if (state == DestructorState.Exploded && other.CompareTag("Player"))
        {
            SetState(DestructorState.InProcess);
            timer = maxTime;
        }
    }

    public void StartTimer(float maxTime)
    {
        SetState(DestructorState.InProcess);
        this.maxTime = maxTime;
    }

    [NaughtyAttributes.Button]
    private void Explode()
    {
        SetState(DestructorState.Exploded);
    }

    public void SetState(DestructorState destructorState)
    {
        if (destructorState != this.state)
        {
            normalVisual.SetActive(destructorState == DestructorState.Neutral);
            explodedVisuals.SetActive(destructorState == DestructorState.Exploded);
            hitBox.SetActive(destructorState != DestructorState.Exploded);


            switch (destructorState)
            {
                case DestructorState.Neutral:
                    prepare1Visual.SetActive(false);
                    prepare2Visual.SetActive(false);
                    prepare3Visual.SetActive(false);
                    break;
                case DestructorState.InProcess:
                    if (state == DestructorState.Exploded)
                    {
                        boardManager.SetStateOfNodes(true, this);
                    }
                    break;
                case DestructorState.Exploded:
                    prepare1Visual.SetActive(false);
                    prepare2Visual.SetActive(false);
                    prepare3Visual.SetActive(false);
                    boardManager.SetStateOfNodes(false, this);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(destructorState), destructorState, null);
            }

            this.state = destructorState;
        }
    }

    public void ShowPickup(PickupType pickupType)
    {
        switch (pickupType)
        {
            case PickupType.SpeedUp:
                currentPickup = speedUpPickup;
                break;
            case PickupType.SpeedDown:
                currentPickup = speedDownPickup;
                break;
            case PickupType.Freeze:
                currentPickup = freezePickup;
                break;
            case PickupType.Coin:
                currentPickup = coinPickup;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(pickupType), pickupType, null);
        }

        currentPickup.SetVisible(true);
    }

    public void HidePickup()
    {
        currentPickup.SetVisible(false);
        currentPickup = null;
    }


    public enum DestructorState
    {
        None,
        Neutral,
        InProcess,
        Exploded,
    }

    public enum PickupType
    {
        SpeedUp,
        SpeedDown,
        Freeze,
        Coin,
    }

    // Setup

    public void Setup(Player player, Enemy enemy, BoardManager boardManager)
    {
        this.player = player;
        this.enemy = enemy;
        this.boardManager = boardManager;
    }

}
