using Roy_T.AStar.Paths;
using Roy_T.AStar.Primitives;
using System.Collections.Generic;
using UnityEngine;

internal static class Helpers
{
    public static Vector3 RayTAStarPositionToVec3(Position position) => new(position.X, position.Y);
    public static Position Vec3ToPosition(Vector3 vec3) => new Position(vec3.x, vec3.y);

    public static T Random<T>(this T[] array) => array.Length != 0 ? array[UnityEngine.Random.Range(0, array.Length)] : default;
    public static T Random<T>(this List<T> list) => list.Count != 0 ? list[UnityEngine.Random.Range(0, list.Count)] : default;
}
