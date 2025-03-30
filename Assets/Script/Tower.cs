using System.Collections.Generic;
using UnityEngine;

public enum TowerType
{
    Arrow,
    Cannon,
    Magic,
    Sniper
}

// 防御塔基类
public abstract class Tower : MonoBehaviour
{
    public TowerType towerType;
    public float range = 3f;
    public float attackRate = 1f;
    public int attackDamage = 20;

    [Header("塔属性")] public bool dealsMagicDamage = false; // 是否造成魔法伤害
    public bool canTargetFlying = false; // 是否能攻击飞行单位

    protected float nextAttackTime;
    protected Transform target;
    // protected Animator animator;

    private void Awake()
    {
        // animator = GetComponent<Animator>();
        SetTowerProperties();
    }

    // 根据塔类型设置属性
    protected virtual void SetTowerProperties()
    {
        switch (towerType)
        {
            case TowerType.Arrow:
                dealsMagicDamage = false;
                canTargetFlying = true;
                break;
            case TowerType.Cannon:
                dealsMagicDamage = false;
                canTargetFlying = false;
                break;
            case TowerType.Magic:
                dealsMagicDamage = true;
                canTargetFlying = true;
                break;
            case TowerType.Sniper:
                dealsMagicDamage = false;
                canTargetFlying = true;
                break;
        }
    }


    private void Update()
    {
        // 如果当前没有目标或目标不在范围内，才寻找新目标
        if (target == null || !IsInRange(target))
        {
            FindTarget();
        }
        // 如果目标存在但不活跃（被销毁或禁用），清空目标引用
        else if (target != null && !target.gameObject.activeInHierarchy)
        {
            target = null;
            FindTarget();
        }

        // 攻击目标
        if (target != null && Time.time >= nextAttackTime)
        {
            Attack();
            nextAttackTime = Time.time + (1f / attackRate);
        }
    }

    protected virtual void FindTarget()
    {
        // 如果当前已有目标且在范围内，保持锁定
        if (target != null && IsInRange(target) && target.gameObject.activeInHierarchy)
        {
            // 检查当前目标是否是飞行单位且当前塔不能攻击飞行单位
            Enemy enemy = target.GetComponent<Enemy>();
            if (enemy != null && enemy.canFly && !canTargetFlying)
            {
                // 清除目标，寻找新目标
                target = null;
            }
            else
            {
                return;
            }
        }

        // 查找范围内最近的敌人
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, range);
        float shortestDistance = Mathf.Infinity;
        Transform nearestEnemy = null;

        foreach (Collider2D collider in colliders)
        {
            if (collider.CompareTag("Enemy") && collider.gameObject.activeInHierarchy)
            {
                // 检查是否为飞行单位
                Enemy enemy = collider.GetComponent<Enemy>();
                if (enemy != null && enemy.canFly && !canTargetFlying)
                {
                    // 如果是飞行单位且当前塔不能攻击飞行单位，则跳过
                    continue;
                }

                float distanceToEnemy = Vector2.Distance(transform.position, collider.transform.position);
                if (distanceToEnemy < shortestDistance)
                {
                    shortestDistance = distanceToEnemy;
                    nearestEnemy = collider.transform;
                }
            }
        }

        target = nearestEnemy;
    }

    protected virtual void Attack()
    {
        if (target != null)
        {
            // 播放攻击动画
            // animator.SetTrigger("Attack");

            // 执行具体攻击逻辑(由子类实现)
        }
    }

    protected bool IsInRange(Transform targetTransform)
    {
        return Vector2.Distance(transform.position, targetTransform.position) <= range;
    }

    void OnDrawGizmosSelected()
    {
        // 在Scene视图中绘制攻击范围
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, range);
    }
}