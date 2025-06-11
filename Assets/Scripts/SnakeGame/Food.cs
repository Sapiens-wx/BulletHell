using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Food : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<Snake>().Grow();

            //f_i
            other.GetComponent<Snake>().foodCollected++;

            FindObjectOfType<FoodSpawner>().SpawnFood(); // generate new food
            Destroy(gameObject); // destroy current food
        }
    }
}

