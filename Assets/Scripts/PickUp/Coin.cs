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
        SoundEffectPlayer.Instance.PlaySoundClip(SoundEffectPlayer.COIN);
        GameManager.instance.CollectCoin();
        SetVisible(false);
    }

    public override void OnEnemyPickedUp(Enemy enemy) { }
}
