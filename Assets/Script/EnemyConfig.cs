using UnityEngine;

[CreateAssetMenu(fileName = "NewEnemyConfig", menuName = "Tower Defense/Enemy Config")]
public class EnemyConfig : ScriptableObject
{
    [Header("基本属性")]
    public EnemyType enemyType;
    public string enemyName;
    public int maxHealth;
    public int currencyReward;
    public float moveSpeed;
    
    [Header("抗性属性")]
    [Range(0f, 0.9f)]
    public float physicalResistance = 0f; // 物理抗性百分比，最高90%
    [Range(0f, 0.9f)]
    public float magicResistance = 0f;    // 魔法抗性百分比，最高90%
    
    // 是否为飞行单位
    public bool canFly = false;
}
