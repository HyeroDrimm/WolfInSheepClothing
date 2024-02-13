using System;
using UnityEngine;

public class ItemsConsts
{
    public const string POWER_UP_SPEED_UP = "PowerUpSpeedUp";
    public const string POWER_UP_SPEED_DOWN = "PowerUpSpeedDown";
    public const string POWER_UP_FREEZE = "PowerUpFreeze";
    public const string POWER_UP_DOLL = "PowerUpDoll";

    public const float POWER_UP_SPEED_UP_MODIFIER = 1.2f;
    public const float POWER_UP_SPEED_UP_DURATION = 3f;

    public const float POWER_UP_SPEED_DOWN_MODIFIER = 0.7f;
    public const float POWER_UP_SPEED_DOWN_DURATION = 3f;

    public const float POWER_UP_FREEZE_DURATION = 3f;
}

[Serializable]
public class ShopItem
{
    public string type;

    public Sprite icon;
    public string name;
    public int price;
}