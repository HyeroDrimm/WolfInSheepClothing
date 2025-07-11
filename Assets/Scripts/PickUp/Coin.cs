using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : PickUp
{
    protected override void Awake()
    {
        base.Awake();
        SetVisible(false);
    }

    public override void OnPlayerPickedUp(Player player)
    {
        GameManager.instance.CollectCoin();
    }

    public override void OnEnemyPickedUp(Enemy enemy) { }
}
