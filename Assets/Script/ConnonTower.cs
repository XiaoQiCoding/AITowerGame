// 炮塔

using UnityEngine;

public class ConnonTower : Tower
{
    public GameObject cannonballPrefab;
    public float splashRadius = 1.5f;

    protected override void SetTowerProperties()
    {
        base.SetTowerProperties();
        // 炮塔：物理伤害，不可攻击飞行目标
        dealsMagicDamage = false;
        canTargetFlying = false;
    }

    protected override void Attack()
    {
        base.Attack();

        if (target == null)
            return;

        // 生成炮弹
        GameObject cannonball = Instantiate(cannonballPrefab, transform.position, Quaternion.identity);
        SplashProjectile projectile = cannonball.GetComponent<SplashProjectile>();

        if (projectile != null)
        {
            projectile.isMagicDamage = dealsMagicDamage; // 设置伤害类型
            projectile.Initialize(target, attackDamage, splashRadius);
        }
    }
}