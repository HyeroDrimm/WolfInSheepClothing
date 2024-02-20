using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class PickUpManager : MonoBehaviour
{
    [SerializeField] private float nullRadius;
    [SerializeField] private Transform[] nullCenters;

    [SerializeField] private PickUpTypeHolder[] pickUpTypesHolders;

    private HashSet<Transform> usedPickUpPlaces = new();

    void Awake()
    {
        foreach (var pickUpTypeHolder in pickUpTypesHolders)
        {
            foreach (var pickUp in pickUpTypeHolder.pickUps)
            {
                pickUp.OnPickedUp += FreePickUpPlace;
            }
        }
    }

    void Update()
    {
        foreach (var pickUpTypeHolder in pickUpTypesHolders)
        {
            pickUpTypeHolder.counter += Time.deltaTime;
            if (pickUpTypeHolder.counter >= pickUpTypeHolder.timeBetweenSpawns)
            {
                pickUpTypeHolder.counter = 0;
                Spawn(pickUpTypeHolder);
            }
        }
    }

    private void Spawn(PickUpTypeHolder pickUpTypesHolder)
    {
        var currentlyShownPickUpsCount = pickUpTypesHolder.pickUps.Count(x => x.isShown);

        for (int i = 0; i < Mathf.Min(pickUpTypesHolder.maxSpawned - currentlyShownPickUpsCount, pickUpTypesHolder.numberSpawned); i++)
        {
            var availablePickUps = pickUpTypesHolder.pickUps.Where(x => x.isShown == false && !usedPickUpPlaces.Contains(x.PickUpPlace) && IsInNullRadius(x.gameObject)).ToList();
            if (availablePickUps.Count != 0)
            {
                var newPickUp = availablePickUps.Random();
                usedPickUpPlaces.Add(newPickUp.PickUpPlace);
                newPickUp.SetVisible(true);
            }
            else
            {
                break;
            }
        }
    }

    private bool IsInNullRadius(GameObject gameObject)
    {
        return nullCenters.All(nullCenter => nullCenter == null || Vector3.Distance(nullCenter.position, gameObject.transform.position) > nullRadius);
    }

    public void FreePickUpPlace(Transform pickupPlace)
    {
        usedPickUpPlaces.Remove(pickupPlace);
    }

    [Serializable]
    public class PickUpTypeHolder
    {
        public PickUp[] pickUps;
        public float timeBetweenSpawns;
        public int numberSpawned;
        public int maxSpawned;

        [HideInInspector] public float counter;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, nullRadius);
    }
}