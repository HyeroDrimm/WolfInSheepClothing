using UnityEngine;

public class FreezeEnemyPowerUp : PickUp
{
    [SerializeField] private float duration;

    protected override void Awake()
    {
        base.Awake();
        SetVisible(false);

    }

    public override void OnPlayerPickedUp(Player player)
    {
        player.PickedUpEnemyFreezePowerUp(duration);
    }

    public override void OnEnemyPickedUp(Enemy enemy)
    {
        enemy.PickedUpEnemyFreezePowerUp(duration);
    }
}
