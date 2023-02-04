using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    // Script to keep track of and manage enemies

    [SerializeField] int enemyCount; //max a wave can possibly have
    [SerializeField] GameObject enemyPrefab;
    [SerializeField] Transform spawnPoint; //ToDo: Random from a list of points?

    List<EnemyBase> enemyPool;
    int poolIndex = 0;
    int spawnedEnemies = 0;

    float[] spawnDelays = new float[] {10f, 6f, 4f}; //ToDo: should only spawn after previous wave is dead?
    
    bool isSpawning = true;
    float spawnTimer = 0f;
    float spawnTimerMax = 0.1f;
    
    float waveTimer = 0f;
    int waveCounter = 0;
    public int WaveCounter
    {
        get { return waveCounter; }
        set 
        {
            waveCounter = value; //Should just be ++, but still
            UIManager.Instance.UpdateWaveCounter(waveCounter);
        }
    }

    void Start()
    {
        enemyPool = new List<EnemyBase>();

        GameObject enemy;
        for (int i = 0; i < enemyCount; i++)
        {
            enemy = GameObject.Instantiate(enemyPrefab, Vector3.zero, Quaternion.identity);
            enemyPool.Add(enemy.GetComponent<EnemyBase>());
            enemyPool[enemyPool.Count - 1].manager = this; //Oh dear
        }
        waveTimer = spawnDelays[0]; //Spawn an enemy right away
    }

    public void EnemyDied()
    {
        spawnedEnemies--;

        if (spawnedEnemies <= 0)
        {
            //Wave over, spawn new one!
            waveTimer = waveTimer < 1f ? 1f : waveTimer; //If we have less than a second to go, leave it, otherwise take a second (instant felt wrong) 
            spawnTimer = spawnTimerMax; //Spawn the first one right away
        }
    }

    void Update()
    {
        if (isSpawning)
        {
            spawnTimer += Time.deltaTime;
            if (spawnTimer >= spawnTimerMax)
            {
                //spawn enemies on a timer until we have the total we need
                EnemyBase current = enemyPool[poolIndex]; //ToDo: Spawn each enemy on a very short delay, incase we use the same spawn point twice in a row
                //ToDo: Make sure enemy selected is inactive
                current.SetPosition(spawnPoint.position);
                current.IsActive = true;
                spawnedEnemies++;
                spawnTimer = 0f;
                Debug.Log("Spawn");

                if (spawnedEnemies >= waveCounter)
                    isSpawning = false;
            }
            
        }
        else
        {
            waveTimer += Time.deltaTime;
            if (waveTimer >= spawnDelays[0]) //ToDo: use player "level"
            {
                WaveCounter++;//ToDo: should the number of enemies per wave be related to the overall game timer? Similar to RoR, difficulty goes up over time- currently it gets harder as you kill stuff, so there's no reward for killing things faster

                if (poolIndex >= enemyPool.Count - 1)
                    poolIndex = 0;
                else
                    poolIndex++;
                    
                waveTimer = 0f;
            }
        }
    }
}
