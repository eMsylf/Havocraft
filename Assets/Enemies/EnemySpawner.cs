using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Tooltip("Spawns per second")]
    public float SpawnRate = 1;
    public AnimationCurve SpawnrateMultiplierOverTime = new AnimationCurve();
    private float MultipliedSpawnRate;
    private float TimeBeforeNextSpawn;

    // Start is called before the first frame update
    void Start()
    {
        MultipliedSpawnRate = SpawnrateMultiplierOverTime.Evaluate(Time.time);
        TimeBeforeNextSpawn = 1 / MultipliedSpawnRate;
    }

    // Update is called once per frame
    void Update()
    {
        MultipliedSpawnRate = SpawnrateMultiplierOverTime.Evaluate(Time.time);
        //if (MultipliedSpawnRate
    }

    void Spawn()
    {
        
    }
}
