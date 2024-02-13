using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shop : PickUp
{
    [SerializeField] private GameManager shopUi;

    public override void OnPlayerPickedUp(Player player)
    {
        shopUi.ShowShop(true);
    }

    public override void OnEnemyPickedUp(Enemy enemy) { }
}
