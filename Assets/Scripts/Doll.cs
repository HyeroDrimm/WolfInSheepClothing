﻿using UnityEngine;

public class Doll : MonoBehaviour, IFollowTarget
{
    [HideInInspector] public PathNode currentPosition;
    public PathNode CurrentPosition()
    {
        return currentPosition;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.transform.CompareTag("Enemy"))
        {
            var enemy = other.GetComponentInParent<Enemy>();

            enemy.Doll = null;

            SoundEffectPlayer.Instance.PlaySoundClip(SoundEffectPlayer.KILL);
            Destroy(gameObject);
        }
    }
}