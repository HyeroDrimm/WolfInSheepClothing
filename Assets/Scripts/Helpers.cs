using Roy_T.AStar.Paths;
using Roy_T.AStar.Primitives;
using UnityEngine;

internal static class Helpers
{
    public static Vector3 RayTAStarPositionToVec3(Position position) => new(position.X, position.Y);
    public static Position Vec3ToPosition(Vector3 vec3) => new Position(vec3.x, vec3.y);
}
