using UnityEngine;

public class SniperTower : Tower
{
    public GameObject bulletPrefab; // 子弹预制体

    protected override void SetTowerProperties()
    {
        base.SetTowerProperties();
        // 狙击塔：物理伤害，可攻击飞行目标
        dealsMagicDamage = false;
        canTargetFlying = true;
    }

    protected override void Attack()
    {
        base.Attack();

        if (target != null)
        {
            // 生成子弹
            GameObject bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
            SniperProjectile projectile = bullet.GetComponent<SniperProjectile>();

            if (projectile == null)
            {
                projectile = bullet.AddComponent<SniperProjectile>();
            }

            // 初始化子弹
            if (projectile != null)
            {
                projectile.isMagicDamage = dealsMagicDamage; // 设置伤害类型
                projectile.Initialize(target, attackDamage);
            }
        }
    }
}