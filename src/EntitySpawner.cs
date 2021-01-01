using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntitySpawner : MonoBehaviour
{
    [SerializeField]
    private GameObject entityPrefab;

    public float spawnStartTime = 5, spawnRepeatTime = 3;

    private LevelManager levelManager;

    // Start is called before the first frame update
    void Start()
    {
        levelManager = FindObjectOfType<LevelManager>();
        InvokeRepeating("SpawnEntity", spawnStartTime, spawnRepeatTime);
    }

    void SpawnEntity()
    {
        if (CanSpawn())
        {
            Vector3 entityPosition;
            entityPosition.x = Random.Range(levelManager.GetXMin(), levelManager.GetXMax());
            entityPosition.y = Random.Range(levelManager.GetYMin(), levelManager.GetYMax());
            entityPosition.z = Random.Range(levelManager.GetZMin(), levelManager.GetZMax());

            GameObject spawnedEntity = Instantiate(entityPrefab, entityPosition, transform.rotation)
                as GameObject;

            spawnedEntity.transform.parent = gameObject.transform;
        }
    }

    private bool CanSpawn()
    {
        // Checks if it is a paladin or enemy. If it is a barrel, automatically return true.
        if (entityPrefab.tag == "Paladin")
        {
            int currentEnemies = GameObject.FindGameObjectsWithTag("Paladin").Length +
                GameObject.FindGameObjectsWithTag("Enemy").Length;
            // Checks if another enemy can be spawned.
            if (currentEnemies + 1 >= FindObjectOfType<LevelManager>().GetEnemiesRemaining())
                return false;
        }
        if (entityPrefab.tag == "Enemy")
        {
            int currentEnemies = GameObject.FindGameObjectsWithTag("Paladin").Length +
                GameObject.FindGameObjectsWithTag("Enemy").Length;
            // Checks if another enemy can be spawned.
            if (currentEnemies >= FindObjectOfType<LevelManager>().GetEnemiesRemaining())
                return false;
        }

        return true;
    }
}
