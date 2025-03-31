using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyType
{
    Basic,
    DefencePhysical,
    DefencePhysicalPro,
    DefenceMagic,
    DefenceMagicPro,
    Fast,
    Fly,
    FlyPro,
    FlyMax,
    FlyFast,
}


// 敌人基类
public class Enemy : MonoBehaviour
{
    [Header("敌人配置")] public EnemyConfig config;

    public EnemyType enemyType;

    // 跟踪当前动画状态
    private string currentAnimState = "";

    private float moveSpeed = 2f;
    public int health = 100;
    public int currencyReward = 10;

    // 防御属性
    [HideInInspector] public float physicalResistance = 0f; // 物理抗性百分比
    [HideInInspector] public float magicResistance = 0f; // 魔法抗性百分比
    [HideInInspector] public bool canFly = false; // 是否为飞行单位

    // 组件引用
    public List<Transform> pathPoints;
    private int currentPathIndex = 0;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private BoxCollider2D boxCollider;
    private Rigidbody2D rb;

    // 动画方向阈值
    private const float DIRECTION_THRESHOLD = 0.5f;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        boxCollider = GetComponent<BoxCollider2D>();
        rb = GetComponent<Rigidbody2D>();

        // 确保不使用物理碰撞，由我们自己控制移动
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.gravityScale = 0;
        }
    }

    public void Initialize(List<Transform> path)
    {
        pathPoints = path;
        currentPathIndex = 0;
        currentAnimState = "";

        // 使用配置文件中的数据
        if (config != null)
        {
            enemyType = config.enemyType;
            health = config.maxHealth;
            currencyReward = config.currencyReward;
            moveSpeed = config.moveSpeed;

            // 设置防御属性
            SetResistanceByType(enemyType);
        }
        else
        {
            Debug.LogError("Enemy config is missing!");
        }

        // 确保敌人一开始就有正确的动画
        if (pathPoints != null && pathPoints.Count > 0)
        {
            UpdateAnimationDirection(transform.position, pathPoints[0].position);
        }
    }

    // 根据敌人类型设置抗性
    private void SetResistanceByType(EnemyType type)
    {
        // 首先从配置文件获取抗性值
        if (config != null)
        {
            physicalResistance = config.physicalResistance;
            magicResistance = config.magicResistance;
            canFly = config.canFly;
        }
        else
        {
            // 重置属性
            physicalResistance = 0f;
            magicResistance = 0f;
            canFly = false;
        }

        // 根据类型覆盖特殊属性（确保类型和配置一致）
        switch (type)
        {
            case EnemyType.Basic:
                // 普通小怪没有特殊属性
                break;
            case EnemyType.DefencePhysical:
                // 如果配置的抗性比默认值低，则使用默认值
                physicalResistance = Mathf.Max(physicalResistance, 0.5f); // 至少50%物理抗性
                break;
            case EnemyType.DefenceMagic:
                // 如果配置的抗性比默认值低，则使用默认值
                magicResistance = Mathf.Max(magicResistance, 0.5f); // 至少50%魔法抗性
                break;
            case EnemyType.Fly:
                canFly = true; // 强制设为飞行单位
                break;
            case EnemyType.Fast:
                // 快速小怪在config中已设置移动速度
                // 如果需要可以在这里设置最小移动速度
                moveSpeed = Mathf.Max(moveSpeed, 3f); // 确保至少3倍基础速度
                break;
        }
    }

    private void Update()
    {
        if (pathPoints == null || currentPathIndex >= pathPoints.Count)
            return;

        // 向下一个路径点移动
        Transform targetPoint = pathPoints[currentPathIndex];
        Vector3 moveDir = (targetPoint.position - transform.position).normalized;
        transform.position += moveDir * moveSpeed * Time.deltaTime;

        // 更新动画方向
        UpdateAnimationDirection(transform.position, targetPoint.position);

        // 到达路径点后前往下一点
        if (Vector3.Distance(transform.position, targetPoint.position) < 0.1f)
        {
            currentPathIndex++;

            // 到达终点
            if (currentPathIndex >= pathPoints.Count)
            {
                LevelController.Instance.OnEnemyReachedEnd();
                ReturnToPool();
            }
            // 不是终点，更新朝向下一个点的动画
            else if (currentPathIndex < pathPoints.Count)
            {
                UpdateAnimationDirection(transform.position, pathPoints[currentPathIndex].position);
            }
        }
    }

    // 根据移动方向设置正确的动画
    private void UpdateAnimationDirection(Vector3 currentPos, Vector3 targetPos)
    {
        if (animator == null)
            return;

        // 计算移动方向
        Vector2 direction = targetPos - currentPos;
        direction.Normalize();

        // 根据方向选择动画
        bool isFront = direction.y < -DIRECTION_THRESHOLD; // 向下移动
        bool isBack = direction.y > DIRECTION_THRESHOLD; // 向上移动
        bool isSide = Mathf.Abs(direction.x) > DIRECTION_THRESHOLD; // 左右移动

        // 确定应该播放哪个动画状态
        string newAnimState = "";

        if (isFront)
        {
            newAnimState = "Front";
        }
        else if (isBack)
        {
            newAnimState = "Back";
        }
        else if (isSide)
        {
            newAnimState = "Side";

            // 设置正确的朝向 (左右)
            if (direction.x > 0)
            {
                spriteRenderer.flipX = false; // 向右
            }
            else
            {
                spriteRenderer.flipX = true; // 向左
            }
        }

        // 只有当需要切换到新状态时才更新动画参数
        if (newAnimState != currentAnimState && !string.IsNullOrEmpty(newAnimState))
        {
            // 重置所有动画状态
            animator.SetBool("Front", false);
            animator.SetBool("Back", false);
            animator.SetBool("Side", false);

            // 设置新的动画状态
            animator.SetBool(newAnimState, true);

            // 记录当前动画状态
            currentAnimState = newAnimState;

            Debug.Log($"切换敌人动画状态: {newAnimState}, 方向: {direction}");
        }
    }


    public void TakeDamage(int damage, bool isMagicDamage = false)
    {
        // 应用伤害抗性
        float actualDamage = damage;

        if (isMagicDamage)
        {
            // 应用魔法抗性
            actualDamage = damage * (1 - magicResistance);
        }
        else
        {
            // 应用物理抗性
            actualDamage = damage * (1 - physicalResistance);
        }

        // 向下取整确保至少造成1点伤害
        int finalDamage = Mathf.Max(1, Mathf.FloorToInt(actualDamage));
        health -= finalDamage;

        if (health <= 0)
        {
            Die();
        }
    }


    private void Die()
    {
        GameManager.Instance.Currency += currencyReward;
        LevelController.Instance.OnEnemyDefeated();
        GameObject deathEffect = Resources.Load("Prefabs/Effect/DeathEffect") as GameObject;
        Instantiate(deathEffect, transform.position, Quaternion.identity);

        // 移除了Die触发器
        // 直接返回对象池
        ReturnToPool();
    }

    public void ReturnToPool()
    {
        gameObject.SetActive(false);
        transform.SetParent(EnemyPoolManager.Instance.transform);
    }

}