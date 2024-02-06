using System;
using UnityEngine;

public abstract class PickUp : MonoBehaviour
{
    [HideInInspector] public Transform PickUpPlace;
    [HideInInspector] public bool isShown;
    public Action<Transform> OnPickedUp;

    protected virtual void Awake()
    {
        PickUpPlace = transform.parent;
        SetVisible(false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            var player = other.GetComponentInParent<Player>();
            OnPlayerPickedUp(player);
        }
        else if (other.CompareTag("Enemy"))
        {
            var enemy = other.GetComponentInParent<Enemy>();
            OnEnemyPickedUp(enemy);
        }
    }

    public void SetVisible(bool state)
    {
        isShown = state;
        gameObject.SetActive(state);
        
        if (!state)
        {
            OnPickedUp?.Invoke(PickUpPlace);
        }
    }

    public abstract void OnPlayerPickedUp(Player player);
    public abstract void OnEnemyPickedUp(Enemy enemy);
}
