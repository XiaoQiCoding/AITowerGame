// 弹射物基类

using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 10f;
    public float lifetime = 5f;
    protected Transform target;
    protected int damage;
    protected Vector3 lastKnownPosition; // 目标的最后已知位置
    protected bool targetLost = false; // 目标丢失标志
    [HideInInspector] public bool isMagicDamage = false;

    public virtual void Initialize(Transform target, int damage)
    {
        this.target = target;
        this.damage = damage;

        // 初始化目标位置
        if (target != null)
        {
            lastKnownPosition = target.position;
        }

        Destroy(gameObject, lifetime);
    }

    protected virtual void Update()
    {
        // 如果目标为空但还没标记为丢失，记录最后位置并标记丢失
        if (target == null && !targetLost)
        {
            targetLost = true;
            // 最后位置已在Initialize或之前的更新中设置
        }

        // 确定目标点 - 如果目标存在用当前位置，否则用最后已知位置
        Vector3 targetPosition = targetLost ? lastKnownPosition : target.position;

        // 每帧更新最后已知位置（如果目标仍然存在）
        if (!targetLost)
        {
            lastKnownPosition = target.position;
        }

        // 移向目标
        Vector3 dir = targetPosition - transform.position;
        float distanceThisFrame = speed * Time.deltaTime;

        // 如果到达目标位置或者非常接近
        if (dir.magnitude <= distanceThisFrame)
        {
            // 如果目标丢失，直接摧毁自己；否则击中目标
            if (targetLost)
            {
                Destroy(gameObject);
            }
            else
            {
                HitTarget();
            }

            return;
        }

        // 移动
        transform.Translate(dir.normalized * distanceThisFrame, Space.World);

        // 2D旋转 - 计算角度并考虑默认朝上的偏移(-90度)
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90f;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }


    protected virtual void HitTarget()
    {
        Enemy enemy = target.GetComponent<Enemy>();
        if (enemy != null)
        {
            enemy.TakeDamage(damage, isMagicDamage);
        }

        Destroy(gameObject);
    }
}