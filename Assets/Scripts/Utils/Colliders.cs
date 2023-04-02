// created by Maximiliam Rosén
using UnityEngine;

internal static class ColliderExtension
{
    public static Vector3 GetExitPosition(this Collider collider, Vector3 entryPoint, Vector3 direction, float checkDistance) => GetExitPosition(collider, new Ray(entryPoint, direction), checkDistance);
    public static Vector3 GetExitPosition(this Collider collider, Ray ray, float checkDistance)
    {
        // Get a point x distance from the entryPoint    
        ray.origin = ray.GetPoint(checkDistance);
        // Reverse the ray direction
        ray.direction = -ray.direction;
        // Call the raycast from the collider
        collider.Raycast(ray, out var hit, float.MaxValue);
        return hit.point;
    }
}