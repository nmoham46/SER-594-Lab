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
    [SerializeField] private float minimumPlayerDistance = 3f;

    [Header("Player")]
    [SerializeField] private Transform player;

    private float spawnTimer;
    private List<GameObject> spawnedItems = new List<GameObject>();

    void Start()
    {
        ResetTimer();

        // Ensure minimum number exists immediately
        while (GetItemCount() < minimumItems)
        {
            if (!TrySpawnItem())
                break;
        }
    }

    void Update()
    {
        RemoveDestroyedItems();

        // Force spawning if below minimum
        while (GetItemCount() < minimumItems)
        {
            if (!TrySpawnItem())
                break;
        }

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

    bool TrySpawnItem()
    {
        List<Transform> validPoints = new List<Transform>();

        // Player position ignoring Y height
        Vector3 playerFlat = new Vector3(
            player.position.x,
            0f,
            player.position.z
        );

        foreach (Transform point in spawnPoints)
        {
            // Spawn point position ignoring Y height
            Vector3 spawnFlat = new Vector3(
                point.position.x,
                0f,
                point.position.z
            );

            // Don't use spawn points too close to the player
            if (Vector3.Distance(playerFlat, spawnFlat) < minimumPlayerDistance)
                continue;

            bool occupied = false;

            // Make sure another spawned item isn't already here
            foreach (GameObject item in spawnedItems)
            {
                if (item != null &&
                    Vector3.Distance(item.transform.position, point.position) < 0.5f)
                {
                    occupied = true;
                    break;
                }
            }

            if (!occupied)
                validPoints.Add(point);
        }

        // No safe spawn location available
        if (validPoints.Count == 0)
            return false;

        Transform selectedPoint =
            validPoints[Random.Range(0, validPoints.Count)];

        // Check player distance one more time immediately before spawning
        Vector3 selectedFlat = new Vector3(
            selectedPoint.position.x,
            0f,
            selectedPoint.position.z
        );

        if (Vector3.Distance(playerFlat, selectedFlat) < minimumPlayerDistance)
            return false;

        // Randomly choose Hazard or Health Pack
        GameObject selectedPrefab =
            Random.value < 0.5f ? hazardPrefab : healthPackPrefab;

        GameObject newItem = Instantiate(
            selectedPrefab,
            selectedPoint.position,
            Quaternion.identity
        );

        spawnedItems.Add(newItem);

        return true;
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