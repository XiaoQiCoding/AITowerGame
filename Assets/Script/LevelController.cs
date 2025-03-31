// 关卡控制器

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelController : MonoBehaviour
{
    public static LevelController Instance { get; private set; }

    public LevelConfig levelConfig;
    public Transform[] spawnPoints;
    public List<Transform> paths = new List<Transform>(); // 多条路径点集合

    public int currentWave = 0;
    private bool levelCompleted = false;
    private int remainingEnemies = 0;

    // 添加标识符来控制是否继续生成
    private bool shouldStopSpawning = false;
    private List<Coroutine> activeCoroutines = new List<Coroutine>();


    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        StartCoroutine(StartWaves());
    }

    private IEnumerator StartWaves()
    {
        // 重置停止标志
        shouldStopSpawning = false;

        int totalWaves = levelConfig.waves.Count;
        for (int waveIndex = 0; waveIndex < totalWaves; waveIndex++)
        {
            var wave = levelConfig.waves[waveIndex];

            // 检查是否应该停止生成
            if (shouldStopSpawning)
            {
                Debug.Log("停止波次生成");
                yield break;
            }

            yield return new WaitForSeconds(wave.startDelay);

            // 计算这一波敌人的总数
            int enemiesInThisWave = 0;
            foreach (var enemy in wave.enemies)
            {
                enemiesInThisWave += enemy.count;
            }

            Debug.Log($"开始生成第 {waveIndex + 1} 波敌人，总数 {enemiesInThisWave}");

            // 存储协程引用以便停止
            Coroutine spawnCoroutine = StartCoroutine(SpawnWave(wave));
            activeCoroutines.Add(spawnCoroutine);
            currentWave++;

            // 等待敌人生成完毕
            yield return spawnCoroutine;

            // 是否是最后一波
            bool isLastWave = (waveIndex == totalWaves - 1);

            if (isLastWave)
            {
                Debug.Log("等待最后一波敌人清理...");
                // 等待最后一波所有敌人被消灭或到达终点
                yield return StartCoroutine(WaitForAllEnemiesClear());

                // 检查是否所有敌人都被消灭，且玩家没有失败
                if (remainingEnemies <= 0 && GameManager.Instance.CurrentHealth > 0 && !shouldStopSpawning)
                {
                    Debug.Log($"所有敌人已清除(剩余: {remainingEnemies})，触发胜利条件");
                    levelCompleted = true;
                    GameManager.Instance.GameOver(true);
                }
                else
                {
                    Debug.Log(
                        $"关卡未正常结束: 剩余敌人={remainingEnemies}, 生命值={GameManager.Instance.CurrentHealth}, 停止标记={shouldStopSpawning}");
                }
            }

            // 如果被要求停止，直接跳出循环
            if (shouldStopSpawning)
            {
                Debug.Log("中断波次等待");
                yield break;
            }
        }
    }

    // 等待所有敌人清理完毕
    private IEnumerator WaitForAllEnemiesClear()
    {
        // 记录开始等待时的敌人数量
        int initialCount = remainingEnemies;
        Debug.Log($"开始等待敌人清理，当前敌人数量: {initialCount}");

        float startTime = Time.time;
        float timeLimitSeconds = 600f; // 设置一个最长等待时间，避免无限等待

        while (remainingEnemies > 0 && !shouldStopSpawning)
        {
            // 每秒打印一次当前敌人数量，便于调试
            if (Mathf.FloorToInt(Time.time - startTime) % 5 == 0)
            {
                Debug.Log($"等待中...剩余敌人: {remainingEnemies}");
            }

            // 如果等待时间过长，打印警告并退出
            if (Time.time - startTime > timeLimitSeconds)
            {
                Debug.LogWarning($"等待敌人清理超时! 初始数量: {initialCount}, 当前剩余: {remainingEnemies}");
                break;
            }

            yield return null;
        }

        Debug.Log($"敌人清理完毕或被中断，剩余: {remainingEnemies}, 用时: {Time.time - startTime:F1}秒");
    }

    private void OnDestroy()
    {
        Instance = null;
    }

// 添加停止生成的公共方法
    public void StopSpawning()
    {
        shouldStopSpawning = true;
        Debug.Log("标记停止敌人生成");

        // 停止所有活动的协程
        foreach (Coroutine coroutine in activeCoroutines)
        {
            if (coroutine != null)
            {
                StopCoroutine(coroutine);
            }
        }

        activeCoroutines.Clear();

        // 也可以直接停止所有协程
        StopAllCoroutines();

        Debug.Log("已停止所有敌人生成协程");
    }


    private IEnumerator SpawnWave(LevelConfig.WaveConfig wave)
    {
        int enemiesAddedToCount = 0;

        foreach (var enemyInfo in wave.enemies)
        {
            remainingEnemies += enemyInfo.count;
            enemiesAddedToCount += enemyInfo.count;

            for (int i = 0; i < enemyInfo.count; i++)
            {
                // 检查是否应该停止生成
                if (shouldStopSpawning)
                {
                    Debug.Log("中断敌人生成循环");
                    yield break;
                }

                SpawnEnemy(enemyInfo.enemyType, enemyInfo.pathIndex);
                yield return new WaitForSeconds(enemyInfo.spawnInterval);
            }
        }

        Debug.Log($"波次生成完成，共生成 {enemiesAddedToCount} 个敌人");
    }

    private void SpawnEnemy(EnemyType enemyType, int pathIndex)
    {
        // 根据敌人类型从对象池中获取敌人实例
        GameObject enemyObj = EnemyPoolManager.Instance.GetEnemy(enemyType);
        Enemy enemy = enemyObj.GetComponent<Enemy>();

        // 设置敌人属性并激活
        List<Transform> waypoints = new();
        foreach (Transform child in paths[pathIndex])
        {
            waypoints.Add(child);
        }

        enemy.Initialize(waypoints);
        enemy.transform.position = waypoints[0].position;
        enemy.gameObject.SetActive(true);
    }

    public void OnEnemyReachedEnd()
    {
        GameManager.Instance.LoseHealth(1);
        remainingEnemies--;
        Debug.Log($"敌人到达终点，剩余敌人: {remainingEnemies}");
    }

    public void OnEnemyDefeated()
    {
        remainingEnemies--;
        Debug.Log($"敌人被消灭，剩余敌人: {remainingEnemies}");
    }
}