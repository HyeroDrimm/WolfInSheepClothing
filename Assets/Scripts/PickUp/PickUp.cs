using System;
using MoreMountains.Feedbacks;
using UnityEngine;

public abstract class PickUp : MonoBehaviour
{
    [HideInInspector] public Transform PickUpPlace;
    [HideInInspector] public bool isShown;
    [SerializeField] private MMF_Player appearPlayer;
    private PathNode pathNode;

    protected virtual void Awake()
    {
        PickUpPlace = transform.parent;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            var player = other.GetComponentInParent<Player>();
            OnPlayerPickedUp(player);
            pathNode.HidePickup();
        }
        else if (other.CompareTag("Enemy"))
        {
            var enemy = other.GetComponentInParent<Enemy>();
            OnEnemyPickedUp(enemy);
            pathNode.HidePickup();
        }
    }

    public void Setup(PathNode pathNode)
    {
        this.pathNode = pathNode;
    }

    public void SetVisible(bool state)
    {
        isShown = state;
        gameObject.SetActive(state);
        appearPlayer?.PlayFeedbacks();
    }

    public abstract void OnPlayerPickedUp(Player player);
    public abstract void OnEnemyPickedUp(Enemy enemy);
}
