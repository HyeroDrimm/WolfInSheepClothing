using UnityEngine;

public class Doll : MonoBehaviour, IFollowTarget
{
    [HideInInspector] public GameObject currentPosition;
    public GameObject CurrentPosition()
    {
        return currentPosition;
    }
}