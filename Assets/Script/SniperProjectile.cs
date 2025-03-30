using Unity.VisualScripting;
using UnityEngine;

public class SniperProjectile : Projectile
{
    public float speed = 20f; // 高速移动
    public float lifetime = 3f;

    private Transform target;
    private int damage;
    [HideInInspector] public bool isMagicDamage = false;

    public override void Initialize(Transform target, int damage)
    {
        this.target = target;
        this.damage = damage;
        Destroy(gameObject, lifetime);
    }

    protected override void Update()
    {
        if (target == null)
        {
            Destroy(gameObject);
            return;
        }

        // 直线高速移动到目标
        Vector3 dir = target.position - transform.position;
        float distanceThisFrame = speed * Time.deltaTime;

        if (dir.magnitude <= distanceThisFrame)
        {
            HitTarget();
            return;
        }

        transform.Translate(dir.normalized * distanceThisFrame, Space.World);

        // 朝向目标
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    protected override void HitTarget()
    {
        // 命中敌人并造成伤害
        if (target != null)
        {
            Enemy enemy = target.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage, isMagicDamage);
            }
        }

        // 销毁子弹
        Destroy(gameObject);
    }
}