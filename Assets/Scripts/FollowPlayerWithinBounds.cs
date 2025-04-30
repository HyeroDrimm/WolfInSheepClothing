using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class FollowPlayerWithinBounds : MonoBehaviour
{
    [SerializeField] private Transform camera;
    [SerializeField] private Transform player;
    [SerializeField] private Vector2 offset;
    [SerializeField] private Vector2 bounds;

    private void UpdatePosition()
    {
        if (!player)
            return;

        var trackingPoint = (Vector2)player.position + offset;
        camera.position = new Vector3(Mathf.Max(transform.position.x - bounds.x,
            Mathf.Min(transform.position.x + bounds.x, trackingPoint.x)),
            Mathf.Max(transform.position.y - bounds.y,
                Mathf.Min(transform.position.y + bounds.y, trackingPoint.y)
            ), -10f);
    }

    void Update()
    {
        UpdatePosition();
    }
    
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position + new Vector3(offset.x, offset.y, 0), bounds);
    }
}
