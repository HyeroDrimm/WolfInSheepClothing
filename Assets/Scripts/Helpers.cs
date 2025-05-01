using Roy_T.AStar.Paths;
using Roy_T.AStar.Primitives;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

internal static class Helpers
{
    public static Vector3 RayTAStarPositionToVec3(Position position) => new(position.X, position.Y);
    public static Position Vec3ToPosition(Vector3 vec3) => new Position(vec3.x, vec3.y);

    public static T Random<T>(this T[] array) => array.Length != 0 ? array[UnityEngine.Random.Range(0, array.Length)] : default;
    public static T Random<T>(this List<T> list) => list.Count != 0 ? list[UnityEngine.Random.Range(0, list.Count)] : default;

    public static void Shuffle<T>(this IList<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = UnityEngine.Random.Range(0, n + 1); // you can use other function that returns a random number between 0 and n+1 as integer
            (list[k], list[n]) = (list[n], list[k]);
        }
    }

    public static void Shuffle<T>(this T[] array)
    {
        int n = array.Length;
        while (n > 1)
        {
            n--;
            int k = UnityEngine.Random.Range(0, n + 1); // you can use other function that returns a random number between 0 and n+1 as integer
            (array[k], array[n]) = (array[n], array[k]);
        }
    }

    public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> enumerable)
    {
        var list = enumerable.ToList();
        list.Shuffle();
        return list;
    }
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
