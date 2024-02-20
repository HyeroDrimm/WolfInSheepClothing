using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Dependencies.Sqlite;
using UnityEngine;
using UnityEngine.UI;

public class Destructor : MonoBehaviour
{
    [SerializeField] private GameObject node;
    [SerializeField] private Slider onSlider;
    [SerializeField] private GameObject explodedVisuals;
    [SerializeField] private GameObject onVisuals;
    [SerializeField] private GameObject hitBox;
    [SerializeField] private float decreaseRate = 1f;

    private DestructorState state = DestructorState.None;
    private float timer = 0;
    private float maxTime = 0;

    public DestructorState State => state;

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
                onSlider.value = fraction;
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
            var player = other.GetComponentInParent<Player>();
            Destroy(player.gameObject);
            GameManager.instance.EndGame();
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
            explodedVisuals.SetActive(destructorState == DestructorState.Exploded);
            onVisuals.SetActive(destructorState == DestructorState.On);
            onSlider.gameObject.SetActive(destructorState == DestructorState.On);
            hitBox.SetActive(destructorState != DestructorState.Exploded);

            switch (destructorState)
            {
                case DestructorState.Neutral:
                    break;
                case DestructorState.On:
                    break;
                case DestructorState.Exploded:
                    PathController.Singleton.SetStateOfNodes(false, node);
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
