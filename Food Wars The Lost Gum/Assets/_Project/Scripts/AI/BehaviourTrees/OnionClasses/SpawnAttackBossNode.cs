using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnAttackBossNode : ActionNode
{
    public float totalTime;
    private float startTime;
    private OnionBossPhase1 boss;
    private int enemiesSpawned;
    public int enemiesToSpawn;
    public float enemiesSpawnZValue;
    public float spawnHorizontalWallOffsetValue;

    public SoundPlayer m_spawnAttackSound;
    public SoundPlayer m_individualSpawnSound;
    private GameObject agentParent;

    // Start is called before the first frame update
    protected override void OnStart()
    {
        boss = Owner.GetComponent<OnionBossPhase1>();
        boss.AnimationWrapper(1);
        startTime = Time.time;
        enemiesSpawned = 0;
        agentParent = boss.agentParent;

        // Play a sound effect
        if (m_spawnAttackSound != null)
        {
            SoundPlayer sound = GameObject.Instantiate(m_spawnAttackSound, boss.transform.position, Quaternion.identity);
        }
    }

    protected override void OnStop() { }

    // Update is called once per frame
    protected override State OnUpdate()
    {
        if (Time.time > startTime + totalTime && enemiesSpawned >= enemiesToSpawn) return State.Success;

        if (Time.time > startTime + (1 + enemiesSpawned) * totalTime / enemiesToSpawn)
        { 
            enemiesSpawned++;
            GameObject enemy;
            Vector3 position = new Vector3( boss.leftWallX + enemiesSpawned*(boss.rightWallX - boss.leftWallX)/(enemiesToSpawn+1), boss.wallY - spawnHorizontalWallOffsetValue, enemiesSpawnZValue);
            
            enemy = Instantiate(boss.enemyToSpawn[Random.Range(0, boss.enemyToSpawn.Length)], position, Quaternion.identity);
            enemy.transform.SetParent(agentParent.transform);

            // Play a sound effect
            if (m_individualSpawnSound != null)
            {
                SoundPlayer sound = GameObject.Instantiate(m_individualSpawnSound, position, Quaternion.identity);
            }
        }
        return State.Running;
    }
}
