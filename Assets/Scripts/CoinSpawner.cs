using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CoinSpawner : MonoBehaviour
{
    [SerializeField] private Coin[] coins;
    [SerializeField] private float timeBetweenSpawns;
    [SerializeField] private int numberOfCoinsSpawned;
    [SerializeField] private int maxSpawnedCoins;

    // Start is called before the first frame update
    void Awake()
    {
        StartCoroutine(WaitAndSpawnCoins());
    }

    private IEnumerator WaitAndSpawnCoins()
    {
        yield return new WaitForSeconds(timeBetweenSpawns);

        var hiddenCoins = coins.Where(x => x.isShown == false).ToList();
        var currentlyShownCoinCount = coins.Count(x => x.isShown == true);

        var spawnIndexes = new List<int>();
        for (int i = 0; i < Mathf.Min(maxSpawnedCoins - currentlyShownCoinCount, numberOfCoinsSpawned); i++)
        {
            int coinIndex;
            do
            {
                coinIndex = Random.Range(0, hiddenCoins.Count);

            } while (spawnIndexes.Contains(coinIndex));
            spawnIndexes.Add(coinIndex);
            var choosenCoin = hiddenCoins[coinIndex];
            choosenCoin.SetVisible(true);
        }

        StartCoroutine(WaitAndSpawnCoins());
    }
}
