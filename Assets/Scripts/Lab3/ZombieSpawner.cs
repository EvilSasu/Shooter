using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieSpawner : MonoBehaviour
{
    public GameObject zombiePrefab;
    public Transform spawnPoint;
    public float spawnInterval = 5f;    
    public int maxZombies = 10;
    public bool killAllZombies = false;
    public bool zombieReturnToSpwan = false;

    public List<GameObject> zombies = new List<GameObject>();

    private void Start()
    {
        InvokeRepeating("SpawnZombie", spawnInterval, spawnInterval);
    }

    private void FixedUpdate()
    {
        if (zombieReturnToSpwan)
        {
            if(zombies.Count > 0)
            {
                if (!zombies[0].GetComponent<ZombieController>().GetZombieReturningToSpawner())
                    ReturnZombiesToSpawner();
            }           
        }

        if (killAllZombies)
        {
            if (zombies.Count > 0)
                DestroyAllZombies();         
        }
    }

    private void SpawnZombie()
    {
        if (zombies.Count < maxZombies)
        {
            GameObject newZombie = Instantiate(zombiePrefab, spawnPoint.position, spawnPoint.rotation, transform);
            zombies.Add(newZombie);
            newZombie.GetComponent<ZombieController>().spawner = this;
            if(zombieReturnToSpwan) ReturnZombiesToSpawner();
        }
    }

    public void ReturnZombiesToSpawner()
    {
        foreach (GameObject zombie in zombies)
        {
            if (zombie != null)
            {
                ZombieController zombieController = zombie.GetComponent<ZombieController>();
                if (zombieController != null)
                {
                    zombieController.ReturnToSpawner(spawnPoint.position);
                }
            }
        }
    }

    public void DestroyAllZombies()
    {
        foreach (GameObject zombie in zombies)
        {
            if (zombie != null)
            {
                Destroy(zombie);
            }
        }
        zombies.Clear();
    }
}
