﻿using Roy_T.AStar.Paths;
using Roy_T.AStar.Primitives;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

internal static class Helpers
{
    public static Vector3 RayTAStarPositionToVec3(Position position) => new(position.X, position.Y);
    public static Position Vec3ToPosition(Vector3 vec3) => new Position(vec3.x, vec3.y);
}

public static class LevelsConsts
{
    public static float[,] timeForLevelStars =
    { 
      // 1, 2, 3 stars
        {0f,1f,2f}, // Level 1 
        {0f,1f,2f}, // Level 2
        {0f,1f,2f}  // Level 3
    };
}
