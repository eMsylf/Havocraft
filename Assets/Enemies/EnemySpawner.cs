using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public Enemy EnemyPrefab;
    [Tooltip("Spawns per second")]
    public AnimationCurve SpawnrateOverTime = new AnimationCurve();
    private float currentSpawnRate;
    private float timeBeforeNextSpawn;
    private float countdown = 0;

    private void Start()
    {
        countdown = UpdateSpawnTime(Time.time);
    }

    void Update()
    {
        if (countdown > 0)
        {
            countdown -= Time.deltaTime;
            return;
        }

        Spawn();
        countdown = UpdateSpawnTime(Time.time);
    }

    float UpdateSpawnTime(float time)
    {
        currentSpawnRate = SpawnrateOverTime.Evaluate(time);
        if (currentSpawnRate != 0f)
            timeBeforeNextSpawn = 1 / currentSpawnRate;
        else
            timeBeforeNextSpawn = 1f;
        return timeBeforeNextSpawn;
    }

    void Spawn()
    {
        if (EnemyPrefab == null)
            Debug.LogError("Enemy Spawner has no enemy prefab assigned", this);
        Instantiate(EnemyPrefab, transform);
    }
}
