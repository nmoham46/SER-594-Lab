using UnityEngine;
using System.Collections.Generic;

public class RandomSpawner : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] private GameObject hazardPrefab;
    [SerializeField] private GameObject healthPackPrefab;

    [Header("Spawn Points")]
    [SerializeField] private Transform[] spawnPoints;

    [Header("Spawn Settings")]
    [SerializeField] private float minSpawnTime = 5f;
    [SerializeField] private float maxSpawnTime = 10f;
    [SerializeField] private int minimumItems = 2;
    [SerializeField] private int maximumItems = 10;
    [SerializeField] private float minimumPlayerDistance = 3.5f;

    [Header("Player")]
    [SerializeField] private Transform player;

    private float spawnTimer;
    private List<GameObject> spawnedItems = new List<GameObject>();

    void Start()
    {
        if (player == null)
        {
            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
            if (playerObject != null)
                player = playerObject.transform;
        }

        if (player == null)
        {
            Debug.LogError("RandomSpawner needs a Player reference before spawning items.", this);
            enabled = false;
            return;
        }

        ResetTimer();
        SpawnUntilMinimumIsMet();
    }

    void Update()
    {
        RemoveDestroyedItems();

        // Refill only at spawn points that are still safely away from the player.
        SpawnUntilMinimumIsMet();

        spawnTimer -= Time.deltaTime;

        if (spawnTimer <= 0f)
        {
            if (GetItemCount() < maximumItems)
            {
                TrySpawnItem();
            }

            ResetTimer();
        }
    }

    void SpawnUntilMinimumIsMet()
    {
        while (GetItemCount() < minimumItems)
        {
            if (!TrySpawnItem())
                break;
        }
    }

    bool TrySpawnItem()
    {
        if (player == null || spawnPoints == null || spawnPoints.Length == 0)
            return false;

        List<Transform> validPoints = new List<Transform>();
        foreach (Transform point in spawnPoints)
        {
            if (point == null || !IsFarEnoughFromPlayer(point.position))
                continue;

            bool occupied = false;
            foreach (GameObject item in spawnedItems)
            {
                if (item != null && FlatDistance(item.transform.position, point.position) < 1.5f)
                {
                    occupied = true;
                    break;
                }
            }

            if (!occupied)
                validPoints.Add(point);
        }

        if (validPoints.Count == 0)
            return false;

        Transform selectedPoint = validPoints[Random.Range(0, validPoints.Count)];

        // Recheck against the player's latest position immediately before spawning.
        Vector3 spawnPosition = selectedPoint.position;
        if (!IsFarEnoughFromPlayer(spawnPosition))
            return false;

        GameObject selectedPrefab = Random.value < 0.5f ? hazardPrefab : healthPackPrefab;
        if (selectedPrefab == null)
            return false;

        GameObject newItem = Instantiate(selectedPrefab, spawnPosition, Quaternion.identity);
        spawnedItems.Add(newItem);
        return true;
    }

    bool IsFarEnoughFromPlayer(Vector3 position)
    {
        return player != null && FlatDistance(player.position, position) >= minimumPlayerDistance;
    }

    static float FlatDistance(Vector3 a, Vector3 b)
    {
        a.y = 0f;
        b.y = 0f;
        return Vector3.Distance(a, b);
    }

    int GetItemCount()
    {
        RemoveDestroyedItems();
        return spawnedItems.Count;
    }

    void RemoveDestroyedItems()
    {
        spawnedItems.RemoveAll(item => item == null);
    }

    void ResetTimer()
    {
        spawnTimer = Random.Range(minSpawnTime, maxSpawnTime);
    }
}