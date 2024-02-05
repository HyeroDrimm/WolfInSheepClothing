using UnityEngine;

public class SpeedChangePowerUp : PickUp
{
    [SerializeField] private float speedModifier;
    [SerializeField] private float duration;
    public override void OnPlayerPickedUp(Player player)
    {
        if (speedModifier < 1)
        {
            SoundEffectPlayer.Instance.PlaySoundClip(SoundEffectPlayer.POWER_DOWN);
        }
        else if (speedModifier > 1)
        {
            SoundEffectPlayer.Instance.PlaySoundClip(SoundEffectPlayer.POWER_UP);
        }
        player.PickedUpSpeedChangePowerUp(speedModifier, duration);
        SetVisible(false);
    }

    public override void OnEnemyPickedUp(Enemy enemy)
    {
        enemy.PickedUpSpeedChangePowerUp(speedModifier, duration);
        SetVisible(false);
    }
}