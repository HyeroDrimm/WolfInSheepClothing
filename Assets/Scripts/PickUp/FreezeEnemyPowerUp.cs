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
        SoundEffectPlayer.Instance.PlaySoundClip(SoundEffectPlayer.POWER_UP);

        player.PickedUpEnemyFreezePowerUp(duration);
    }

    public override void OnEnemyPickedUp(Enemy enemy)
    {
        SoundEffectPlayer.Instance.PlaySoundClip(SoundEffectPlayer.POWER_DOWN);

        enemy.PickedUpEnemyFreezePowerUp(duration);
    }
}
