using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Destructor : MonoBehaviour
{
    [SerializeField] private GameObject node;
    [SerializeField] private GameObject hitBox;
    [SerializeField] private GameObject shop;
    [SerializeField] private float decreaseRate = 1f;
    [SerializeField] private GameObject[] pickups;

    [Header("Visuals")]
    [SerializeField] private GameObject normalVisual;
    [SerializeField] private GameObject prepare1Visual;
    [SerializeField] private GameObject prepare2Visual;
    [SerializeField] private GameObject prepare3Visual;
    [SerializeField] private GameObject explodedVisuals;

    private DestructorState state = DestructorState.None;
    private float timer = 0;
    private float maxTime = 0;

    public DestructorState State => state;
    public GameObject Node => node;

    private void Awake()
    {
        SetState(DestructorState.Neutral);
    }

    private void Update()
    {
        if (state == DestructorState.On)
        {
            timer += Time.deltaTime;
            var fraction = timer / maxTime;
            if (fraction >= 1.0f)
            {
                SetState(DestructorState.Exploded);
            }
            else
            {

                if (fraction < 0.3f)
                {
                    prepare1Visual.SetActive(true);
                    prepare2Visual.SetActive(false);
                    prepare3Visual.SetActive(false);
                }
                else if (fraction < 0.6f)
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
        if (state == DestructorState.On && other.CompareTag("Player"))
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
            SetState(DestructorState.On);
        }
    }

    public void StartTimer(float maxTime)
    {
        SetState(DestructorState.On);
        this.maxTime = maxTime;
    }

    public void SetState(DestructorState destructorState)
    {
        if (destructorState != this.state)
        {
            normalVisual.SetActive(destructorState == DestructorState.Neutral);
            explodedVisuals.SetActive(destructorState == DestructorState.Exploded);
            hitBox.SetActive(destructorState != DestructorState.Exploded);
            if (shop != null)
            {
                shop.SetActive(destructorState == DestructorState.Exploded);
            }

            switch (destructorState)
            {
                case DestructorState.Neutral:
                    prepare1Visual.SetActive(false);
                    prepare2Visual.SetActive(false);
                    prepare3Visual.SetActive(false);
                    break;
                case DestructorState.On:
                    break;
                case DestructorState.Exploded:
                    prepare1Visual.SetActive(false);
                    prepare2Visual.SetActive(false);
                    prepare3Visual.SetActive(false);
                    PathController.Singleton.SetStateOfNodes(false, node);
                    foreach (var pickup in pickups)
                    {
                        Destroy(pickup);
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(destructorState), destructorState, null);
            }

            this.state = destructorState;
        }
    }


    public enum DestructorState
    {
        None,
        Neutral,
        On,
        Exploded,
    }
}
