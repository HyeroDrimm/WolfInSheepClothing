using UnityEngine;

public class SpeedChangePowerUp : PickUp
{
    [SerializeField] private float speedDownPercentage;
    [SerializeField] private float speedUpDuration;
    public override void OnPlayerPickedUp(Player player)
    {
        player.PickedDownSpeedChange(speedDownPercentage, speedUpDuration);
        SetVisible(false);
    }

    public override void OnEnemyPickedUp(Enemy enemy)
    {
        enemy.PickedDownSpeedChange(speedDownPercentage, speedUpDuration);
        SetVisible(false);
    }
}