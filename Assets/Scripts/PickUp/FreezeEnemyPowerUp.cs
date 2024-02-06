using UnityEngine;

public class FreezeEnemyPowerUp : PickUp
{
    [SerializeField] private float duration;
    public override void OnPlayerPickedUp(Player player)
    {
        SoundEffectPlayer.Instance.PlaySoundClip(SoundEffectPlayer.POWER_UP);

        player.PickedUpEnemyFreezePowerUp(duration);
        SetVisible(false);
    }

    public override void OnEnemyPickedUp(Enemy enemy)
    {
        SoundEffectPlayer.Instance.PlaySoundClip(SoundEffectPlayer.POWER_DOWN);

        enemy.PickedUpEnemyFreezePowerUp(duration);
        SetVisible(false);
    }
}
