using UnityEngine;

public class SpeedChangePowerUp : PickUp
{
    [SerializeField] private float speedModifier;
    [SerializeField] private float duration;

    protected override void Awake()
    {
        base.Awake();
        SetVisible(false);
    }

    public override void OnPlayerPickedUp(Player player)
    {
        if (speedModifier < 1)
        {
        }
        else if (speedModifier > 1)
        {
        }
        player.PickedUpSpeedChangePowerUp(speedModifier, duration);
    }

    public override void OnEnemyPickedUp(Enemy enemy)
    {
        enemy.PickedUpSpeedChangePowerUp(speedModifier, duration);
    }
}