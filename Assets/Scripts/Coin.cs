using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : PickUp
{
    public override void OnPlayerPickedUp(Player player)
    {
        GameManager.instance.CollectCoin();
        SetVisible(false);
    }

    public override void OnEnemyPickedUp(Enemy enemy) { }
}
