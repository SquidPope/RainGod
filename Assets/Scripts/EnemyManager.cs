using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class NewWaveEvent : UnityEvent<int> {}
public class EnemyManager : MonoBehaviour
{
    // Script to keep track of and manage enemies

    [SerializeField] int enemyCount; //max a wave can possibly have
    [SerializeField] GameObject[] enemyPrefabs;
    [SerializeField] Transform[] spawnPoints; //ToDo: Random from a list of points?

    List<EnemyBase> enemyPool;
    int poolIndex = 0;
    int spawnedEnemies = 0;

    float[] spawnDelays = new float[] {10f, 6f, 4f}; //ToDo: should only spawn after previous wave is dead?
    
    bool isSpawning = true;
    float spawnTimer = 0f;
    float spawnTimerMax = 0.1f;
    
    float waveTimer = 0f;
    int waveCounter = 0;
    float waveDelay = 4f; //Amount of time the player gets between waves if they finish a wave with more time than this remaining.

    bool isPlaying = true;

    NewWaveEvent newWave = new NewWaveEvent();
    public NewWaveEvent NewWave { get { return newWave; } }

    public int WaveCounter
    {
        get { return waveCounter; }
        set 
        {
            //ToDo: Partially heal player at the end of each wave, amount based on quantity 
            waveCounter = value; //Should just be ++, but still
            isSpawning = true;
            NewWave.Invoke(waveCounter);
            Debug.Log("New Wave");
        }
    }

    void Start()
    {
        enemyPool = new List<EnemyBase>();

        GameObject enemy;
        for (int i = 0; i < enemyCount; i++)
        {
            int rand = Random.Range(0, enemyPrefabs.Length);
            enemy = GameObject.Instantiate(enemyPrefabs[rand], Vector3.zero, Quaternion.identity); //ToDo: Randomize on activate? select enemies for the wave?
            enemyPool.Add(enemy.GetComponent<EnemyBase>());
            enemyPool[enemyPool.Count - 1].manager = this; //Oh dear
        }

        WaveCounter++; //Spawn enemies right away

        GameController.Instance.StateChange.AddListener(StateChange);
    }

    void StateChange(GameState state) { isPlaying = state == GameState.Playing; }

    public void EnemyDied()
    {
        spawnedEnemies--;

        if (spawnedEnemies <= 0)
        {
            //Wave over, spawn new one!
            //ToDo: If player defeats all of a wave before the next spawns, give them a bonus to worship points (when we implement those)
            waveTimer = waveTimer < waveDelay ? waveDelay : waveTimer; //If we have less than a second to go, leave it, otherwise take a second (instant felt wrong)
            spawnTimer = spawnTimerMax; //Spawn the first one right away
        }
    }

    void Update()
    {
        if (!isPlaying)
            return;
            
        if (isSpawning)
        {
            spawnTimer += Time.deltaTime;
            if (spawnTimer >= spawnTimerMax)
            {
                //spawn enemies on a timer until we have the total we need
                EnemyBase current = enemyPool[poolIndex];
                //ToDo: Make sure enemy selected is inactive
                int rand = Random.Range(0, spawnPoints.Length - 1);
                current.SetPosition(spawnPoints[rand].position);
                current.IsActive = true;
                spawnedEnemies++;
                spawnTimer = 0f;
                Debug.Log("Spawn");

                if (poolIndex >= enemyPool.Count - 1)
                    poolIndex = 0;
                else
                    poolIndex++;

                if (spawnedEnemies >= WaveCounter)
                    isSpawning = false;
            }
        }
        else
        {
            waveTimer += Time.deltaTime;
            if (waveTimer >= spawnDelays[0]) //ToDo: use player "level"
            {
                WaveCounter++;//ToDo: should the number of enemies per wave be related to the overall game timer? Similar to RoR, difficulty goes up over time- currently it gets harder as you kill stuff, so there's no reward for killing things faster

                waveTimer = 0f;
            }
        }
    }
}
