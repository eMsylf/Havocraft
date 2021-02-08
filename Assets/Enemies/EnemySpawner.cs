using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public Enemy EnemyPrefab;
    [Tooltip("Spawns per second")]
    public AnimationCurve SpawnrateOverTime = new AnimationCurve();
    public bool SpawnAtStart;
    private float currentSpawnRate;
    private float timeBeforeNextSpawn;
    private float countdown = 0;

    private void Start()
    {
        if (SpawnAtStart)
            Spawn();
        countdown = UpdateSpawnTime(Time.timeSinceLevelLoad);
    }

    void Update()
    {
        if (countdown > 0)
        {
            countdown -= Time.deltaTime;
            return;
        }

        Spawn();
        countdown = UpdateSpawnTime(Time.timeSinceLevelLoad);
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
