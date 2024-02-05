using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PickUpManager : MonoBehaviour
{
    [SerializeField] private Transform[] pickUpPlaces;
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
            var availablePickUps = pickUpTypesHolder.pickUps.Where(x => x.isShown == false && !usedPickUpPlaces.Contains(x.PickUpPlace)).ToList();
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
}