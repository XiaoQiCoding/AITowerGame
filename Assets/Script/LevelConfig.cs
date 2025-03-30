// 关卡配置

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelConfig", menuName = "Tower Defense/Level Config")]
public class LevelConfig : ScriptableObject
{
    public int levelID;
    public string levelName;
    public int initialCurrency = 100;
    public int initialHealth = 15;
    
    [System.Serializable]
    public class WaveConfig
    {
        public float startDelay;
        public List<EnemySpawnInfo> enemies = new List<EnemySpawnInfo>();
    }
    
    [System.Serializable]
    public class EnemySpawnInfo
    {
        public EnemyType enemyType;
        public int count;
        public float spawnInterval;
        public int pathIndex; // 使用哪条路径
    }
    
    public List<WaveConfig> waves = new List<WaveConfig>();
}

