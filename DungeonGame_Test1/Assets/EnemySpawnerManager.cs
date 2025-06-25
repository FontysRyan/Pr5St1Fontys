using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnerManager : MonoBehaviour
{
    public Player playerScript;
    public UI_handler uI_Handler;
    public GameObject enemyPrefab;
    public GameObject Enemy_Parent;
    public int baseEnemiesPerWave = 3;
    public Transform spawnPointParent;
    public GameObject player; // optional, for boosting
    public int score;
    private List<Transform> spawnPoints = new List<Transform>();
    private List<GameObject> activeEnemies = new List<GameObject>();
    internal int currentWave = 1;

    private void Start()
    {
        // Collect all spawn points
        foreach (Transform child in spawnPointParent)
        {
            spawnPoints.Add(child);
        }

        StartCoroutine(HandleWaves());
    }

    private IEnumerator HandleWaves()
    {
        while (true)
        {
            yield return StartCoroutine(StartNewWave());
            yield return StartCoroutine(WaitForEnemiesDefeated());
            yield return StartCoroutine(ApplyPlayerReward());

            currentWave++;
            yield return new WaitForSeconds(2f); // small delay before next wave
        }
    }

    private IEnumerator StartNewWave()
    {
        // Clear previous list
        activeEnemies.Clear();

        // Shuffle spawn points
        List<Transform> shuffledSpawns = new List<Transform>(spawnPoints);
        for (int i = 0; i < shuffledSpawns.Count; i++)
        {
            Transform temp = shuffledSpawns[i];
            int randomIndex = Random.Range(i, shuffledSpawns.Count);
            shuffledSpawns[i] = shuffledSpawns[randomIndex];
            shuffledSpawns[randomIndex] = temp;
        }

        int enemiesToSpawn = baseEnemiesPerWave + currentWave + Random.Range(3, 8); // adds 0 to 2 extra


        for (int i = 0; i < enemiesToSpawn && i < shuffledSpawns.Count; i++)
        {
            GameObject enemy = Instantiate(enemyPrefab, shuffledSpawns[i].position, Quaternion.identity, Enemy_Parent.transform);

            // Set difficulty amplifier
            Enemy enemyScript = enemy.GetComponent<Enemy>();
            if (enemyScript != null)
            {
                enemyScript.DifficultyDMGAmplifier = Mathf.Max(1, currentWave);
            }

            activeEnemies.Add(enemy);
        }
        if (uI_Handler != null)
        {
            uI_Handler.UpdateLevelStats(currentWave, activeEnemies.Count);
        }
        yield return null;
    }

    private IEnumerator WaitForEnemiesDefeated()
    {
        while (activeEnemies.Exists(e => e != null))
        {
            yield return null; // wait until all enemies are gone
        }
    }

    private IEnumerator ApplyPlayerReward()
    {
        playerScript.BoostPlayer();
        //if (player != null && player.TryGetComponent(out Character character))
        //{
        //    int roll = Random.Range(0, 2);
        //    if (roll == 0)
        //    {
        //        character.BoostHealth();
        //        Debug.Log("Player gained health!");
        //    }
        //    else
        //    {
        //        character.BoostDamage();
        //        Debug.Log("Player gained damage!");
        //    }
        //}

        yield return null;
    }

    internal void AddScore(int IncomingScore)
    {
        score = score + IncomingScore;
    }

    private void Update()
    {
        uI_Handler.UpdateLevelStats(currentWave, Enemy_Parent.transform.childCount);
        uI_Handler.UpdateScore(score);
    }

    internal void PlayerDiedResetGame()
    {
        foreach (Transform child in Enemy_Parent.transform)
        {
            Destroy(child.gameObject);
        }
    }
}
