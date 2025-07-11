using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodSpawner : MonoBehaviour
{
    public GameObject foodPrefab;
    public Transform snakeHead;
    public float spawnRadius = 2f;
    public float minX = -10, maxX = 10, minY = -5, maxY = 5;

    private GameObject currentFood;

    void Start()
    {
        SpawnFood();
    }

    public void SpawnFood()
    {
        if (currentFood != null)
            Destroy(currentFood);

        Vector2 spawnPosition;
        int maxAttempts = 100;
        int attempts = 0;

        // food generate near player and align with grid
        do
        {
            float offsetX = Mathf.Round(Random.Range(-spawnRadius, spawnRadius));
            float offsetY = Mathf.Round(Random.Range(-spawnRadius, spawnRadius));
            spawnPosition = new Vector2(
                Mathf.Clamp(Mathf.Round(snakeHead.position.x + offsetX), minX, maxX),
                Mathf.Clamp(Mathf.Round(snakeHead.position.y + offsetY), minY, maxY)
            );
            attempts++;
        }
        while (Vector2.Distance(spawnPosition, snakeHead.position) < 1f && attempts < maxAttempts);

        currentFood = Instantiate(foodPrefab, spawnPosition, Quaternion.identity);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<Snake>().Grow();
            FindObjectOfType<FoodSpawner>().SpawnFood();
            Destroy(gameObject);
        }
    }
}
