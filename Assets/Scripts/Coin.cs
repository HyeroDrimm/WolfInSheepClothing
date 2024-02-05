using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : PickUp
{
    public override void OnPlayerPickedUp(PathActor player)
    {
        GameManager.instance.CollectCoin();
        SetVisible(false);
    }

    public override void OnEnemyPickedUp(PlayerChaser enemy) { }
}
