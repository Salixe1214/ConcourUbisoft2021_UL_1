using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransportableSpawner : MonoBehaviour
{
    [SerializeField] private GameObject[] TransportablesPrefab = null;
    [SerializeField] private Transform PointA = null;
    [SerializeField] private Transform PointB = null;
    [SerializeField] private Level1Controller Level1Controller = null;
    [SerializeField] private Vector2 DelayBetweenSpawnsInSeconds = new Vector2(0.5f, 1);

    private float lastSpawnTime = 0.0f;
    private float currentDelay = 0.0f;

    private void Update()
    {
        if (Time.time - lastSpawnTime > currentDelay)
        {
            Spawn();
            lastSpawnTime = Time.time;
            currentDelay = Random.Range(DelayBetweenSpawnsInSeconds.x, DelayBetweenSpawnsInSeconds.y);
        }
    }

    private void Spawn()
    {
        Color[] possibleColors = Level1Controller.GetColors();

        GameObject randomPrefab = TransportablesPrefab[Random.Range(0, TransportablesPrefab.Length)];
        Color randomColor = possibleColors[Random.Range(0, possibleColors.Length)];

        Vector3 randomPoint = PointA.position + Random.Range(0, 100) / 100.0f * (PointB.position - PointA.position);

        GameObject transportable = Instantiate(randomPrefab, randomPoint, Quaternion.identity);
        transportable.gameObject.GetComponent<TransportableByConveyor>().Color = randomColor;
    }
}
