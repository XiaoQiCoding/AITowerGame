using UnityEngine;

public class MagicProjectile : Projectile
{
    public float speed = 10f;
    public float lifetime = 3f;
    public Color trailColor = new Color(0.5f, 0.2f, 1f); // 紫色魔法轨迹
    public GameObject hitEffectPrefab; // 可选的命中效果
    
    private Transform target;
    private int damage;
    private Vector3 targetPosition;
    private bool targetLost = false;
    [HideInInspector] public bool isMagicDamage = true;
    
    public override void Initialize(Transform target, int damage)
    {
        this.target = target;
        this.damage = damage;
        
        if (target != null)
        {
            targetPosition = target.position;
        }
        else
        {
            targetLost = true;
            Destroy(gameObject);
            return;
        }
        
        // 设置轨迹效果
        TrailRenderer trailRenderer = GetComponent<TrailRenderer>();
        if (trailRenderer == null)
        {
            trailRenderer = gameObject.AddComponent<TrailRenderer>();
        }
        
        // 配置轨迹渲染器以获得漂亮的魔法效果
        trailRenderer.startWidth = 0.15f;
        trailRenderer.endWidth = 0.05f;
        trailRenderer.time = 0.5f; // 轨迹持续时间更长
        trailRenderer.startColor = trailColor;
        trailRenderer.endColor = new Color(trailColor.r, trailColor.g, trailColor.b, 0);
        trailRenderer.material = new Material(Shader.Find("Sprites/Default"));
        
        // 添加一点点随机旋转，使其看起来更生动
        transform.rotation = Quaternion.Euler(0, 0, Random.Range(0f, 360f));
        
        Destroy(gameObject, lifetime); // 安全机制，确保最终被销毁
    }

    protected override void Update()
    {
        if (target == null && !targetLost)
        {
            targetLost = true;
        }
        else if (!targetLost)
        {
            // 更新目标位置
            targetPosition = target.position;
        }

        // 计算方向
        Vector3 dir = (targetPosition - transform.position).normalized;
        
        // 移动魔法弹
        transform.position += dir * speed * Time.deltaTime;
        
        // 旋转朝向运动方向
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
        
        // 检查是否到达目标
        float distanceThisFrame = speed * Time.deltaTime;
        if (Vector3.Distance(transform.position, targetPosition) <= distanceThisFrame)
        {
            HitTarget();
        }
        
        // 添加微小的随机偏移，使魔法弹看起来有点"魔法颤动"效果
        transform.position += new Vector3(
            Random.Range(-0.02f, 0.02f), 
            Random.Range(-0.02f, 0.02f), 
            0) * Time.deltaTime;
    }
    
    void OnTriggerEnter2D(Collider2D collider)
    {
        // 碰撞检测，当接触敌人时触发
        if (collider.CompareTag("Enemy"))
        {
            Enemy enemy = collider.GetComponent<Enemy>();
            if (enemy != null)
            {
                HitTarget();
            }
        }
    }

    protected override void HitTarget()
    {
        // 命中目标造成伤害
        if (target != null)
        {
            Enemy enemy = target.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage, isMagicDamage);
                
                // 创建命中特效
                if (hitEffectPrefab != null)
                {
                    GameObject hitEffect = Instantiate(hitEffectPrefab, target.position, Quaternion.identity);
                    Destroy(hitEffect, 1f);
                }
            }
        }
        
        // 销毁魔法弹
        Destroy(gameObject);
    }
}
